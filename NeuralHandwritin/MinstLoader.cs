using System;
using System.IO;
using System.Threading.Tasks;

namespace NeuralHandwritin.Data
{
    public static class MnistLoader
    {
        private const int ImageMagicNumber = 2051;
        private const int LabelMagicNumber = 2049;
        private const int MaxParallelism = 8; // adjust to your CPU cores

        public static async Task<(double[][] images, int[] labels)> LoadAsync(string folderPath, bool isTrain = true)
        {
            string imageFile = Path.Combine(folderPath, isTrain ? "train-images-idx3-ubyte" : "t10k-images-idx3-ubyte");

            string labelFile = Path.Combine(folderPath, isTrain ? "train-labels-idx1-ubyte" : "t10k-labels-idx1-ubyte");

            if (!File.Exists(imageFile))
            {
                throw new FileNotFoundException($"Image file not found: {imageFile}");
            }

            if (!File.Exists(labelFile))
            {
                throw new FileNotFoundException($"Label file not found: {labelFile}");
            }

            // 1. Async read whole files
            byte[] imageBytes = await File.ReadAllBytesAsync(imageFile);
            byte[] labelBytes = await File.ReadAllBytesAsync(labelFile);

            // 2. Parse headers (sync - very fast)
            int imageMagic = ReadBigInt32(imageBytes, 0);
            int numImages = ReadBigInt32(imageBytes, 4);
            int rows = ReadBigInt32(imageBytes, 8);
            int cols = ReadBigInt32(imageBytes, 12);

            int labelMagic = ReadBigInt32(labelBytes, 0);
            int numLabels = ReadBigInt32(labelBytes, 4);

            if (imageMagic != ImageMagicNumber)
            {
                throw new InvalidDataException("Invalid MNIST image file magic number");
            }

            if (labelMagic != LabelMagicNumber)
            {
                throw new InvalidDataException("Invalid MNIST label file magic number");
            }

            if (numImages != numLabels)
            {
                throw new InvalidDataException("Image and label count mismatch");
            }

            int pixelCount = rows * cols; // 784

            // Pre-allocate arrays
            var images = new double[numImages][];
            var labels = new int[numLabels];

            // 3. Parallel image parsing
            ParallelOptions options = new() { MaxDegreeOfParallelism = MaxParallelism };

            Parallel.For(0, numImages, options, i =>
            {
                var image = new double[pixelCount];
                int baseOffset = 16 + i * pixelCount;

                for (int p = 0; p < pixelCount; p++)
                {
                    image[p] = imageBytes[baseOffset + p] / 255.0;
                }

                images[i] = image;
            });

            // 4. Parallel label parsing (fast, but consistent)
            Parallel.For(0, numLabels, options, i =>
            {
                labels[i] = labelBytes[8 + i];
            });

            Console.WriteLine($"Loaded {numImages:N0} {(isTrain ? "training" : "test")} images " + $"({rows}×{cols}) asynchronously & in parallel.");

            Console.WriteLine($"First 10 labels: {string.Join(", ", labels[..10])}");

            return (images, labels);
        }

        private static int ReadBigInt32(byte[] bytes, int offset)
        {
            return (bytes[offset] << 24) | (bytes[offset + 1] << 16) | (bytes[offset + 2] << 8) | bytes[offset + 3];
        }
    }
}