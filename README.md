# Ink Lab

The existing Windows Forms app lives in `NeuralHandwritin/`. The separate `ui/` app uses TypeScript and Vite. `NeuralHandwritin.Api/` exposes the existing C# network and MNIST loader over a loopback-only HTTP API; it compiles the original source files directly.

## Run the web interface

Prerequisites: .NET 10 SDK and Node.js 22.12+ (or 24 LTS).

Start the engine from the repository root:

```powershell
dotnet run --project NeuralHandwritin.Api
```

In a second terminal:

```powershell
cd ui
npm install
npm run dev
```

Open the local URL printed by Vite. Its development proxy forwards `/api` to `http://127.0.0.1:5080`.

Enter an absolute folder path containing these extracted MNIST files:

- `train-images-idx3-ubyte`
- `train-labels-idx1-ubyte`
- `t10k-images-idx3-ubyte`
- `t10k-labels-idx1-ubyte`

Start training, wait for evaluation to finish, then draw a centered digit and select **Recognize digit**. Training progress and test accuracy come from the C# engine. Predictions use white-on-black 28 × 28 inputs, matching the original drawing app. Scores are raw sigmoid activations, not calibrated probabilities. Models remain in memory and must be trained again after restarting the API. Training steps are individual sampled examples, not full dataset epochs.

## Build

```powershell
dotnet build NeuralHandwritin.Api
cd ui
npm run build
```

The frontend build outputs to `ui/dist`. For deployment, serve those files with a same-origin reverse proxy for `/api`; the included proxy is for the Vite development server only. Keep the API local: it accepts dataset paths from the UI and has no authentication.
