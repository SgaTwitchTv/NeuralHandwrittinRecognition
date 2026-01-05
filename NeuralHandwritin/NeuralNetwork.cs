using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NeuralHandwritin.Core;

public class NeuralNetwork
{
    private double[][] hiddenWeight; // [hidden neuron][input]
    private double[] hiddenBias;
    private double[][] outputWeight; // [output neuron][hidden neuron]
    private double[] outputBias;

    private Random random;

    public NeuralNetwork(int inpSize, int hiddenSize, int otpSize)
    {
        random = new Random();

        //random weights and biases initialization
        hiddenWeight = InitMatrix(hiddenSize, inpSize);
        hiddenBias = new double[hiddenSize];
        Randomize(hiddenBias);

        outputWeight = InitMatrix(otpSize, hiddenSize);
        outputBias = new double[otpSize];
        Randomize(outputBias);
    }

    //initialize a matrix with random numbers between -1 and 1
    private double[][] InitMatrix(int rows, int cols)
    {
        double[][] matrix = new double[rows][];
        for (int i = 0; i < rows; i++)
        {
            matrix[i] = new double[cols];
            Randomize(matrix[i]);
        }

        return matrix;
    }

    private void Randomize(double[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = random.NextDouble() * 2 - 1; // Random values between -1 and 1
        }
    }

    //the sigmoid activation function
    private double Sigmoid(double x)
    {
        return 1.0 / (1.0 + Math.Exp(-x));
    }

    private double SigmoidDerivative(double x)
    {
        return x * (1 - x);
    }

    //forward propagation (to move throught the network's layers)
    public double[] Forward(double[] inp)
    {
        //from input to hidden layer
        double[] hidden = new double[hiddenWeight.Length];
        for (int i = 0; i < hidden.Length; i++)             //we loop over hidden neurons
        {
            double sum = hiddenBias[i];                     //bias of ith neuron
            for (int j = 0; j < inp.Length; j++)
            {
                sum += hiddenWeight[i][j] * inp[j];         //weight of ith neuron * input
            }
            hidden[i] = Sigmoid(sum);                       //output of ith hidden neuron
        }

        //from hidden to output layer
        double[] output = new double[outputWeight.Length];
        for (int i = 0; i < output.Length; i++)
        {
            double sum = outputBias[i];
            for (int j = 0; j < hidden.Length; j++)
            {
                sum += outputWeight[i][j] * hidden[j];
            }
            output[i] = Sigmoid(sum);
        }

        return output;
    }

    //training the network with backpropagation
    public void Train(double[] inp, double[] targets, double learningRate)
    {
        //forward propagation
        double[] hidden = new double[hiddenWeight.Length];
        for (int i = 0; i < hidden.Length; i++)
        {
            double sum = hiddenBias[i];
            for (int j = 0; j < inp.Length; j++)
            {
                sum += hiddenWeight[i][j] * inp[j];
            }
            hidden[i] = Sigmoid(sum);
        }

        double[] output = new double[outputWeight.Length];
        for (int i = 0; i < output.Length; i++)
        {
            double sum = outputBias[i];
            for (int j = 0; j < hidden.Length; j++)
            {
                sum += outputWeight[i][j] * hidden[j];
            }
            output[i] = Sigmoid(sum);
        }

        //backpropagation
        //output layer error and delta
        double[] outputErrors = new double[output.Length];
        for (int i = 0; i < outputErrors.Length; i++)
        {
            double error = targets[i] - output[i];
            outputErrors[i] = error * SigmoidDerivative(output[i]);
        }

        //hidden layer error and delta
        double[] hiddenErrors = new double[hidden.Length];
        for (int i = 0; i < hidden.Length; i++)
        {
            double error = 0;
            for (int j = 0; j < output.Length; j++)
            {
                error += outputErrors[j] * outputWeight[j][i];
            }

            hiddenErrors[i] = error * SigmoidDerivative(hidden[i]);
        }

        //update output weights and biases
        //output weights
        for (int i = 0; i < outputWeight.Length; i++)
        {
            for (int j = 0; j < hidden.Length; j++)
            {
                outputWeight[i][j] += learningRate * outputErrors[i] * hidden[j];
            }

            outputBias[i] += learningRate * outputErrors[i];
        }

        //hidden weights
        for (int i = 0; i < hiddenWeight.Length; i++)
        {
            for (int j = 0; j < inp.Length; j++)
            {
                hiddenWeight[i][j] += learningRate * hiddenErrors[i] * inp[j];
            }

            hiddenBias[i] += learningRate * hiddenErrors[i];
        }
    }

    public static double[] OneHot(int label, int size = 10)
    {
        double[] target = new double[size];
        target[label] = 1.0;
        return target;
    }

    public int PredictDigit(double[] output)
    {
        int maxIndex = 0;
        for (int i = 1; i < output.Length; i++)
        {
            if (output[i] > output[maxIndex])
            {
                maxIndex = i;
            }
        }

        return maxIndex;
    }
}