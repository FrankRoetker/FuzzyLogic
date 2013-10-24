using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using FuzzyLogic.TGMCProject.Core;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;

namespace DecisionTrees
{
    public class DecisionTreeClassifier : IClassifier
    {
        private DecisionTree _decisionTree;

        public Func<double[], int> classify;

        public void TrainClassifier(StreamCSVReader reader)
        {
            Console.WriteLine("reading training data...");

            //IList<double[]> inputClassifier = new List<double[]>();
            //IList<double[]> outputResults = new List<double[]>();

            
            DataTable dataTable = new DataTable();
            for(int i = 0; i < reader.NumberColumns - 2; i++)
            {
                dataTable.Columns.Add();
            }

            dataTable.Columns.Add("output");

            // dataTable is supposed to mimic the output of codebook.Apply()
            
            
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

                features.Add(isCorrect ? 1.0 : 0.0);

                dataTable.Rows.Add(features);
            }
            // double[] inputs = feature columns toArray?
            // int[] outputs = true/false values

            // Attributes: array of DecisionVariables. No idea how to construct these. Need DataTable?


            // Doesn't compile yet.
            /*
            _decisionTree = new DecisionTree(attributes, outputClasses: 2);

            Console.WriteLine("Finished reading features.\nBegin training...");
            // Create the C45 algorithm
            C45Learning c45 = new C45Learning(_decisionTree);
            // Learn the tree
            double error = c45.Run(inputs, outputs);
            Console.WriteLine("End Training.");

            classify = _decisionTree.ToExpression().Compile();
             */
        }
    }
}
