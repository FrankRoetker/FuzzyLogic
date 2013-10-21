using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuzzyLogic.TGMCProject.Core;

namespace FuzzyLogic.LogisticRegression
{
    class LogisticRegressionClassifier : IClassifier
    {
        private TrainingDataContainer _trainingData;

        public LogisticRegressionClassifier()
        {
            _trainingData = new TrainingDataContainer();
        }

        public void TrainClassifier(StreamCSVReader reader)
        {
            while (reader.NextRecord())
            {
                float questionId, answerId;
                IList<float> features;
                bool isCorrect;

                if (!reader.GetTGMCRow(true, out questionId, out answerId, out features, 
                    out isCorrect))
                {
                    Console.Out.WriteLine("Didn't read row correctly! :(");
                }

                _trainingData.AddDataRow(features, isCorrect);
            }

            // TODO Based on data in _trainingData, find the odds of the row being true for each value of each column
            // Then, populate a MultivariateSample (Metanumerics class) with each row of features against the log odds of the row being true.
            // Solve it using the LinearRegression method (Metanumerics) specifying the log odds column as the dependent variable.

            // Problem: need to store the entire data set in memory, then append the log odds on each row



        }

        public override string ToString()
        {
            return _trainingData.ToString();
        }
    }
}
