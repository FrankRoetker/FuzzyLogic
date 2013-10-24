using System;
using FuzzyLogic.TGMCProject.Core;
using FuzzyLogic.TGMCProject.LogisticRegression;
using FuzzyLogic.TGMCProject.FuzzySVM;

namespace FuzzyLogic.TGMCProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var classifier = new LogisticRegressionClassifier();

            var reader = new StreamCSVReader(new System.IO.FileStream("..\\..\\..\\Datasets\\q1repeatedtrain.csv", System.IO.FileMode.Open), enableQuotes: false);

            classifier.TrainClassifier(reader);

            var svm = new SVM("..\\..\\..\\Datasets\\q1train.csv", "..\\..\\..\\Datasets\\tgmcevaluation.csv");
            svm.SvmSolver();

            System.Threading.Thread.Sleep(100000000);

            //Console.WriteLine(classifier);

            // Wait for user input before closing the console window...
            //Console.ReadLine();
        }
    }
}
