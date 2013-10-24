using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FuzzyLogic.TGMCProject.AccordLogisticRegression;
using FuzzyLogic.TGMCProject.Core;
using FuzzyLogic.TGMCProject.LogisticRegression;
using FuzzyLogic.TGMCProject.NeuralNetworks;

namespace FuzzyLogic.TGMCProject
{
    public struct EvaluationData
    {
        public double AnswerId;
        public double[] Features;
        public bool IsCorrect;
        public double Confidence;
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            // Train the classifier
            var classifier = new AccordLogisticRegressionClassifier();

            var trainRead = new StreamCSVReader(new System.IO.FileStream("..\\..\\..\\Datasets\\tgmctrain.csv", System.IO.FileMode.Open), enableQuotes:false);

            classifier.TrainClassifier(trainRead);


            // Now, use the training data to classify the evaluation set
            var evaluationRead = new StreamCSVReader(new System.IO.FileStream("..\\..\\..\\Datasets\\tgmcevaluation.csv", System.IO.FileMode.Open), enableQuotes: false);


            // Read in the evaluation set
            IList<EvaluationData> evaluationData = new List<EvaluationData>();
            while (evaluationRead.NextRecord())
            {
                float questionId, answerId;
                IList<double> questionFeatures;
                bool isCorrect;

                if (!evaluationRead.GetTGMCRow(false, out questionId, out answerId, out questionFeatures,
                    out isCorrect))
                {
                    Console.WriteLine("Error reading row");
                    continue;
                }

                var evData = new EvaluationData {AnswerId = answerId, Features = questionFeatures.ToArray()};

                evaluationData.Add(evData);
            }

            // Now, evaluate each feature
            for (int i = 0; i < evaluationData.Count; i++)
            {
                var data = evaluationData[i];
                data.IsCorrect = classifier.ClassifyRow(data.Features, out data.Confidence);
            }
        }
    }
}
