using System;
using System.Collections.Generic;
using FuzzyLogic.TGMCProject.Core;
using Meta.Numerics.Statistics;

namespace FuzzyLogic.TGMCProject.LogisticRegression
{
    public class LogisticRegressionClassifier : IClassifier
    {
        private readonly TrainingDataContainer _trainingData;
        private FitResult model;

        public LogisticRegressionClassifier()
        {
            _trainingData = new TrainingDataContainer();
            FitResult model = null; // Only have a fitresult after training
        }

        public void TrainClassifier(StreamCSVReader reader)
        {
            // Dispense with all the training data after training, keep only the model
            var sample = new MultivariateSample(reader.NumberColumns - 2);

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

                if (features.Count != 318)
                {
                    //
                    Console.WriteLine("Beep");
                }

                _trainingData.AddDataRow(features, isCorrect);

                features.Add(0); // add an extra space for the probability for later

                sample.Add(features);

            }

            // Populate the multivariatesample with the log odds in the last column of each row
            foreach (Double[] currentRow in sample)
            {
                Double combinedOdds = 1;

                // Get the odds for the row by multiplying the odds for each column together. Skip the last column
                for (var i = 0; i < sample.Dimension - 2; i++)
                {
                    var val = currentRow[i];

                    //combinedOdds *= _trainingData.GetOddsOfTrue(i + 2, val);
                    combinedOdds *= _trainingData.GetOddsOfTrue(i, val);
                }

                // Set the last column to the log odds
                currentRow[sample.Dimension -1] = Math.Log(combinedOdds);
            }

            // Solve it specifying the log odds column as the dependent variable
            sample.LinearRegression(sample.Dimension - 1);

        }

        public override string ToString()
        {
            return _trainingData.ToString();
        }
    }
}
