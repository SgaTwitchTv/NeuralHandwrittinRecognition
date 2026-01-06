using NeuralHandwritin.Core;
using NeuralHandwritin.Data;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace NeuralHandwritin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private NeuralNetwork nn;
        private string dataFolder;
        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select folder with MNIST data files";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                lblFolderPath.Text = dialog.SelectedPath;
                btnTrain.Enabled = true;
                lblStatus.Text = "Folder selected. Ready to train";
            }
        }
        private void lblFolderPath_Click(object sender, EventArgs e)
        {

        }

        private async void btnTrain_Click(object sender, EventArgs e)
        {
            btnTrain.Enabled = false;
            proggressBarTraining.Visible = true;
            lblStatus.Text = "Loading data...";

            dataFolder = lblFolderPath.Text;

            try
            {
                var (trainImages, trainLabels) = await MnistLoader.LoadAsync(dataFolder, isTrain: true);
                var (testImages, testLabels) = await MnistLoader.LoadAsync(dataFolder, isTrain: false);

                nn = new NeuralNetwork(784, 128, 10);
                int trainCount = trainImages.Length; // full 60k for real training

                lblStatus.Text = "Training started...";

                // Train in background
                await Task.Run(() =>
                {
                    Random rand = new Random(0);
                    double learningRate = 0.3;
                    int totalEpochs = 200000; // adjust as needed

                    for (int epoch = 0; epoch < totalEpochs; epoch++)
                    {
                        int idx = rand.Next(trainCount);
                        double[] input = trainImages[idx];
                        double[] target = NeuralNetwork.OneHot(trainLabels[idx]);

                        nn.Train(input, target, learningRate);

                        if (epoch % 1000 == 0)
                        {
                            // Update UI safely
                            this.Invoke((MethodInvoker)delegate
                            {
                                proggressBarTraining.Value = (int)((epoch / (double)totalEpochs) * 100);
                                lblStatus.Text = $"Epoch {epoch}: Training...";
                            });
                        }
                    }
                });

                // After training, evaluate on test set
                int correct = 0;
                for (int i = 0; i < testImages.Length; i++)
                {
                    double[] output = nn.Forward(testImages[i]);
                    if (nn.PredictDigit(output) == testLabels[i]) correct++;
                }
                double accuracy = (double)correct / testImages.Length * 100;

                proggressBarTraining.Visible = false;
                lblStatus.Text = $"Training complete! Test accuracy: {accuracy:F2}%";
                btnDraw.Enabled = true;
                btnDraw.BackColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                btnTrain.Enabled = true;
            }
        }

        private void proggressBarTraining_Click(object sender, EventArgs e)
        {

        }

        private void lblStatus_Click(object sender, EventArgs e)
        {

        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            if(nn == null)
            {
                MessageBox.Show("Network not trained yet.");
                return;
            }

            DrawingForm drawForm = new DrawingForm(nn);
            drawForm.ShowDialog();
        }
    }
}
