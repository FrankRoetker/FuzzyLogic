using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyLogic.TGMCProject.Core
{
    public interface IClassifier
    {
        void TrainClassifier(StreamCSVReader reader);
    }
}
