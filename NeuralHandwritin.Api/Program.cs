using NeuralHandwritin.Core;
using NeuralHandwritin.Data;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:5080");
builder.Services.AddSingleton<NetworkSession>();
var app = builder.Build();
app.MapGet("/api/status", (NetworkSession session) => session.Status);
app.MapPost("/api/train", (TrainRequest request, NetworkSession session) =>
{
    if (string.IsNullOrWhiteSpace(request.Folder) || !Directory.Exists(request.Folder))
        return Results.BadRequest(new { error = "Enter an existing local MNIST folder." });
    if (request.Steps is < 1000 or > 2000000 || !double.IsFinite(request.LearningRate) || request.LearningRate is <= 0 or > 1)
        return Results.BadRequest(new { error = "Use 1,000–2,000,000 steps and a learning rate above 0 and at most 1." });
    return session.Start(request) ? Results.Accepted("/api/status") : Results.Conflict(new { error = "Training is already running." });
});
app.MapPost("/api/predict", (PredictRequest request, NetworkSession session) =>
{
    if (request.Pixels is not { Length: 784 } || request.Pixels.Any(p => !double.IsFinite(p) || p < 0 || p > 1))
        return Results.BadRequest(new { error = "Expected 784 pixel values between 0 and 1." });
    var scores = session.Predict(request.Pixels);
    return scores is null ? Results.Conflict(new { error = "Train the network before recognizing a digit." })
        : Results.Ok(new { digit = Array.IndexOf(scores, scores.Max()), scores });
});
app.Run();

record TrainRequest(string Folder, int Steps = 200000, double LearningRate = 0.3);
record PredictRequest(double[] Pixels);
record SessionStatus(string Phase, int Completed, int Total, double? Accuracy, string? Error);

sealed class NetworkSession
{
    private readonly object gate = new();
    private NeuralNetwork? network;
    private SessionStatus status = new("idle", 0, 0, null, null);
    public SessionStatus Status { get { lock (gate) return status; } }
    public double[]? Predict(double[] pixels) { lock (gate) return network?.Forward(pixels); }
    public bool Start(TrainRequest request)
    {
        lock (gate)
        {
            if (status.Phase is "loading" or "training" or "evaluating") return false;
            status = new("loading", 0, request.Steps, null, null);
        }
        _ = Task.Run(async () =>
        {
            try
            {
                var (images, labels) = await MnistLoader.LoadAsync(request.Folder);
                var (testImages, testLabels) = await MnistLoader.LoadAsync(request.Folder, false);
                if (images.Length == 0 || testImages.Length == 0 || images.Concat(testImages).Any(x => x.Length != 784) || labels.Concat(testLabels).Any(x => x is < 0 or > 9))
                    throw new InvalidDataException("Use nonempty 28 × 28 MNIST digit datasets with labels 0–9.");
                var candidate = new NeuralNetwork(784, 128, 10);
                var random = new Random(0);
                for (int step = 0; step < request.Steps; step++)
                {
                    int index = random.Next(images.Length);
                    candidate.Train(images[index], NeuralNetwork.OneHot(labels[index]), request.LearningRate);
                    if (step % 1000 == 0) lock (gate) status = status with { Phase = "training", Completed = step };
                }
                lock (gate) status = status with { Phase = "evaluating", Completed = request.Steps };
                int correct = 0;
                for (int i = 0; i < testImages.Length; i++)
                    if (candidate.PredictDigit(candidate.Forward(testImages[i])) == testLabels[i]) correct++;
                lock (gate)
                {
                    network = candidate;
                    status = status with { Phase = "ready", Accuracy = 100.0 * correct / testImages.Length };
                }
            }
            catch (Exception error) { lock (gate) status = status with { Phase = "error", Error = error.Message }; }
        });
        return true;
    }
}
