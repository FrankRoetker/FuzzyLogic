using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuzzyLogic.TGMCProject.Core;

namespace FuzzyLogic.LogisticRegression
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LogisticRegressionClassifier classifier = new LogisticRegressionClassifier();

            StreamCSVReader reader = new StreamCSVReader(new System.IO.FileStream("..\\..\\..\\Datasets\\tinytrain.csv", System.IO.FileMode.Open));

            classifier.TrainClassifier(reader);

            System.Console.WriteLine(classifier);

            Console.ReadLine();
        }
    }
}
