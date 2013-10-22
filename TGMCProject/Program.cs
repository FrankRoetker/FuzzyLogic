using System;
using FuzzyLogic.TGMCProject.Core;
using FuzzyLogic.TGMCProject.LogisticRegression;

namespace FuzzyLogic.TGMCProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var classifier = new LogisticRegressionClassifier();

            var reader = new StreamCSVReader(new System.IO.FileStream("..\\..\\..\\Datasets\\tgmctrain.csv", System.IO.FileMode.Open), enableQuotes:false);

            classifier.TrainClassifier(reader);

            //Console.WriteLine(classifier);
            
            // Wait for user input before closing the console window...
            //Console.ReadLine();
        }
    }
}
