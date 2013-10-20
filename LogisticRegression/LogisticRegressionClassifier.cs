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
                int questionId, answerId;
                IList<float> features;
                bool isCorrect;

                reader.GetTGMCRow(true, out questionId, out answerId, out features, 
                    out isCorrect);

                _trainingData.AddDataRow(features, isCorrect);
            }
        }
    }
}
