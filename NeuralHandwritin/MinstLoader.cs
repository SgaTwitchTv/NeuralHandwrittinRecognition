using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NeuralHandwritin.Data;

public static class MnistLoader
{
    private const int ImageMagicNumber = 2051;
    private const int LabelMagicNumber = 2049;

    public static async Task<(double[][] images, int[] labels)> LoadAsync(string folderPath, bool isTrain)
    {
        //files with handwritten digits and labels with 10 numbers from 0 to 9
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

        //we read file as a byte array
        byte[] imageBytes = await File.ReadAllBytesAsync(imageFile);
        byte[] labelBytes = await File.ReadAllBytesAsync(labelFile);

        //here we extract the first 4 integers (4 bytes each)
        int imageMagic = ReadBigInt32(imageBytes, 0);        //2051 confirms an image file
        int numImages = ReadBigInt32(imageBytes, 4);        //60000 digits
        int rows = ReadBigInt32(imageBytes, 8);            //28 - image height
        int cols = ReadBigInt32(imageBytes, 12);          //28 - image width

        int labelMagic = ReadBigInt32(labelBytes, 0);   //2049 confirms a label file
        int numLabels = ReadBigInt32(labelBytes, 4);   //60000 numlmages
        //the headers tell us the file's structure.
        //Without them, we wouldn't know how many images there are or their size (28x28 = 784 pixels)
        //ReadBigInt32 handles the byte order(big-endian = most significant byte first, common in old formats).

        if (imageMagic != ImageMagicNumber)
        {
            throw new Exception("Invalid MNIST image file magic number");
        }
        if (labelMagic != LabelMagicNumber)
        {
            throw new Exception("Invalid MNIST label file magic number");
        }
        if (numImages != numLabels)
        {
            throw new Exception("Mismatch between number of images and labels");
        }

        //now after haveing validated the files we can extract the images and labels
        int pixelCount = rows * cols;
        var images = new double[numImages][];
        var labels = new int[numLabels];

        for (int i = 0; i < numImages; i++)
        {
            //create a new (type double) image of 784 pixels
            var image = new double[pixelCount];
            for (int j = 0; j < pixelCount; j++)
            {                                             //skip the 16-byte header
                int bytePos = 16 + i * pixelCount + j;   //and calculate the position of the pixel in the byte array
                image[j] = imageBytes[bytePos] / 255.0; //normalize pixel value to [0, 1]
            }
            images[i] = image;                        //store the image in the images array

            int labelPos = 8 + i;                   //skip the 8-byte header for labels
            labels[i] = labelBytes[labelPos];      //store the label
        }

        Console.WriteLine($"Loaded {numImages} MNIST images ({rows}x{cols}) and labels asynchronously.");
        Console.WriteLine($"First 10 labels: {string.Join(", ", labels[..10])}");

        return (images, labels);
    }

    private static int ReadBigInt32(byte[] bytes, int offset)
    {
        return (bytes[offset] << 24) | (bytes[offset + 1] << 16) | (bytes[offset + 2] << 8) | bytes[offset + 3];
    }
}