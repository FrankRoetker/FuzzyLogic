using System;
using FuzzySVM;
using libsvm;

namespace FuzzyLogic.TGMCProject.FuzzySVM
{
    public class SVM
    {
        public const double FEATURES = 300.0;

        private svm_problem prob;
        private svm_problem test;

        public SVM(string trainingFile, string testFile)
        {
            prob = FuzzyProblemHelper.ReadAndScaleProblem(trainingFile);
            test = FuzzyProblemHelper.ReadAndScaleProblem(testFile, testing: false);
        }

        public void SvmSolver()
        {
            //double gamma = width parameter
            //double C = Cost parameter
            //int nr_fold = ??
            double gamma = 0.5;
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
