using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuzzyLogic.TGMCProject.Core;
using libsvm;

namespace FuzzyLogic.TGMCProject.FuzzySVM
{
    class SVM
    {
        public const double FEATURES = 300.0;

        public void SvmSolver(string trainingFile, string testFile)
        {
            var prob = ProblemHelper.ReadAndScaleProblem(trainingFile);
            var test = ProblemHelper.ReadAndScaleProblem(testFile);

            //double gamma = width parameter
            //double C = Cost parameter
            //int nr_fold = ??
            double gamma = 1;
            double C = 1.0/FEATURES;


            var svm = new C_SVC(prob, KernelHelper.RadialBasisFunctionKernel(gamma), C); 
            //var accuracy = svm.GetCrossValidationAccuracy(nr_fold);
            for (var i = 0; i < test.l; i++)
            {
                var x = test.x[i];
                var y = test.y[i];
                var predict = svm.Predict(x); // returns the predicted value 'y'
                var probabilities = svm.PredictProbabilities(x);  // returns the probabilities for each 'y' value

                if (predict.CompareTo(1.0) == 0)
                {
                    //we found an answer!
                    Console.WriteLine(test.y[i]);
                }
            }
        }

    }
}
