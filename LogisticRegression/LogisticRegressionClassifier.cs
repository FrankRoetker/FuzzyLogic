using System;
using System.Collections.Generic;
using FuzzyLogic.TGMCProject.Core;
using Meta.Numerics.Statistics;

namespace FuzzyLogic.TGMCProject.LogisticRegression
{
    public class LogisticRegressionClassifier : IClassifier
    {
        private readonly TrainingDataContainer _trainingData;
        private FitResult _model;

        public LogisticRegressionClassifier()
        {
            _trainingData = new TrainingDataContainer();
        }

        public void TrainClassifier(StreamCSVReader reader)
        {
            Console.WriteLine("Begin training...");
            // Dispense with all the training data after training, keep only the model
            Console.WriteLine("Creating a MultivariateSample with " + (reader.NumberColumns - 2) + " entries per row.");
            var sample = new MultivariateSample(reader.NumberColumns - 2);

            Console.WriteLine("Begin reading data...");
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

                _trainingData.AddDataRow(features, isCorrect);

                features.Add(0); // add an extra space for the probability for later
                if (features.Count != reader.NumberColumns - 2) Console.WriteLine("Warning: row only has " + features.Count + " entries per row.");

                sample.Add(features);

            }
            Console.WriteLine("Finished reading data.");

            // Populate the multivariatesample with the log odds in the last column of each row
            Console.WriteLine("Begin calculating log odds...");
            foreach (var currentRow in sample)
            {
                double combinedProb = 1;
                
                // Get the odds for the row by multiplying the odds for each column together. Skip the last column
                for (var i = 0; i < sample.Dimension - 2; i++)
                {
                    var val = currentRow[i];

                    //combinedOdds *= _trainingData.GetOddsOfTrue(i + 2, val);
                    var prob =  _trainingData.GetProbabilityOfTrue(i, val) + 0.5;
                    //if (prob <= 0.001 || combinedProb <= 0.00000000000000000001)
                    //{
                     //   Console.WriteLine("Odds are 0");
                    //}
                    combinedProb *= prob;
                }

                // Set the last column to the log odds
                System.Console.WriteLine("Combined prob: {0}, log odds: {1}", combinedProb, Math.Log((combinedProb) / (1 - combinedProb)));
                currentRow[sample.Dimension - 1] = Math.Log((combinedProb) / (1 - combinedProb));
            }
            Console.WriteLine("Finished calculating log odds.");

            Console.WriteLine("sample: {0}, dimension: {1}", sample, sample.Dimension);

            // Solve it specifying the log odds column as the dependent variable

            
            Console.WriteLine("Begin linear regression on log odds...");
            _model = sample.LinearRegression(sample.Dimension - 1);
            Console.WriteLine("Finished linear regression on log odds.");
        }

        
        public override string ToString()
        {
            return "";
            //return _trainingData.ToString();
        }
        
    }
}
