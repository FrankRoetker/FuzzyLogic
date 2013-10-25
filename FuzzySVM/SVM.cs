using System;
using FuzzySVM;
using libsvm;
using System.Linq;
using System.Collections.Generic;

namespace FuzzyLogic.TGMCProject.FuzzySVM
{
    public class SVM
    {
        public const double FEATURES = 300.0;

        private PersonalProblem prob;
        private PersonalProblem test;

        public SVM(string trainingFile, string testFile)
        {
            var throwaway = new List<FuzzyProblemHelper.temp>();
            var Data = new List<FuzzyProblemHelper.temp>();

            prob = FuzzyProblemHelper.ReadAndScaleProblem(trainingFile, out throwaway);
            test = FuzzyProblemHelper.ReadAndScaleProblem(testFile, out Data, testing: false);
        }

        public List<int> SvmSolver()
        {
            //double gamma = width parameter
            //double C = Cost parameter
            //int nr_fold = ??
            double gamma = 0.7;
            double C = 0.3; //1.0/FEATURES;

            var stuff = new List<int>();


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
                    //we found an answer
                    stuff.Add(test.answerId[i]);
                }
            }

            return stuff.ToList();
        }
    }
}
