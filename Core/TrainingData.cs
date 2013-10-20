using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyLogic.TGMCProject.Core
{
    class TrainingData
    {
        // TODO: Make an internal representation of training data. Include support for 
        // getting counts of a particular value in a column

        // Maps column index to a map of <value, count>
        private List<Dictionary<IConvertible, int>> _counts;

        public TrainingData()
        {
            _counts = new List<Dictionary<IConvertible, int>>();
        }

        public void AddDataRow(List<float> row)
        {
            // Include the row ID and question ID (the first two elements in the list)
            // Can exclude them later in analysis

            bool firstRowAdded = _counts.Count() <= 0;

            for(int i = 0; i < row.Count(); i++)
            {
                // If this is the first row to be added, add a dictionary to counts for each row element
                if(firstRowAdded) _counts.Add(new Dictionary<IConvertible,int>());
                
                if(!_counts[i].ContainsKey(row[i]))
                {
                    // Add the value of the column as a key if it has not been seen yet
                    _counts[i][row[i]] = 0;
                }
                _counts[i][row[i]]++;
            }
        }

        public int GetCount(int columnIndex, IConvertible value)
        {
            return _counts[columnIndex][value];
        }
    }
}
