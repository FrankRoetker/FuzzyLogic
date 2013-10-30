using System;
using System.Collections.Generic;
using System.Linq;
using AForge.Neuro;
using AForge.Neuro.Learning;
using FuzzyLogic.TGMCProject.Core;

namespace FuzzyLogic.TGMCProject.NeuralNetworks
{
    /// <summary>
    /// A Neural Network Classifier
    /// </summary>
    /// Very very very VERY VERY VERY slow
    public class NeuralNetworkClassifier : IClassifier
    {
        public void TrainClassifier(StreamCSVReader reader)
        {
            Console.WriteLine("reading training data...");
            IList<double[]> inputClassifier = new List<double[]>();
            IList<double[]> outputResults = new List<double[]>();

            while (reader.NextRecord())
            {
                float questionId, answerId;
                IList<double> features;
                bool isCorrect;

                if (!reader.GetTGMCRow(true, out questionId, out answerId, out features,
                    out isCorrect))
                {
                    Console.Out.WriteLine("Didn't read row correctly! :(");
                    continue;
                }

                inputClassifier.Add(features.ToArray());
                outputResults.Add(new[] {isCorrect ? 1.0 : 0.0});
            }

            var inputArray = inputClassifier.ToArray();
            var outputArray = outputResults.ToArray();

            Console.WriteLine("Finished reading features.\nBegin training...");

            var network = new ActivationNetwork(new BipolarSigmoidFunction(), reader.NumberColumns - 3, reader.NumberColumns, reader.NumberColumns / 4, 10, 5, 1);

            var teacher = new BackPropagationLearning(network);
            teacher.LearningRate = 0.5;
            teacher.Momentum = 0.2;

            //var teacher = new EvolutionaryLearning(network, 100);

            double runningError = 1;
            double delta = 1;

            while (delta > 0.05)
            {
                // Do training
                double error = teacher.RunEpoch(inputArray, outputArray);
                delta = Math.Abs(runningError - error);
                Console.Out.WriteLine("Error is now {0} (delta: {1})", error, delta);

                runningError = error;
            }
        }

        public bool ClassifyRow(double[] row, out double confidence)
        {
            confidence = 0.0;
            return false; 
        }
    }
}