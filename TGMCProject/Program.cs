using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FuzzyLogic.TGMCProject.AccordLogisticRegression;
using FuzzyLogic.TGMCProject.AccordSVM;
using FuzzyLogic.TGMCProject.Core;
using FuzzyLogic.TGMCProject.LogisticRegression;
using FuzzyLogic.TGMCProject.FuzzySVM;
using FuzzyLogic.TGMCProject.NeuralNetworks;

namespace FuzzyLogic.TGMCProject
{
    public struct EvaluationData
    {
        public double QuestionId;
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

            var trainRead = new StreamCSVReader(new FileStream("..\\..\\..\\Datasets\\tgmctrain.csv", FileMode.Open), enableQuotes:false);

            classifier.TrainClassifier(trainRead);


            // Now, use the training data to classify the evaluation set
            var evaluationRead = new StreamCSVReader(new FileStream("..\\..\\..\\Datasets\\tgmcevaluation.csv", FileMode.Open), enableQuotes: false);


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

                var evData = new EvaluationData {AnswerId = answerId, Features = questionFeatures.ToArray(), QuestionId = questionId};

                evaluationData.Add(evData);
            }

            using (
                StreamWriter outputFile = new StreamWriter(String.Format(@"..\..\..\Results\{0:MM-dd-hh-mm-ss}.txt", DateTime.Now)),
                               dataFile = new StreamWriter(String.Format(@"..\..\..\Results\RAW-{0:MM-dd-hh-mm-ss}.txt", DateTime.Now)))
            {
                // Now, evaluate each feature
                for (var i = 0; i < evaluationData.Count; i++)
                {
                    var data = evaluationData[i];
                    data.IsCorrect = classifier.ClassifyRow(data.Features, data.QuestionId, data.AnswerId, dataFile, out data.Confidence);

                    if (data.IsCorrect)
                    {
                       Console.Out.WriteLine("CORRECT: {0} ({1})", data.AnswerId, data.Confidence);
                       outputFile.WriteLine(data.AnswerId);
                    }
                }
            }

            //var svm = new SVM("..\\..\\..\\Datasets\\q1train.csv", "..\\..\\..\\Datasets\\tgmcevaluation.csv");
            //svm.SvmSolver();

            //System.Threading.Thread.Sleep(100000000);

            //Console.WriteLine(classifier);

            // Wait for user input before closing the console window...
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
        }
    }
}
