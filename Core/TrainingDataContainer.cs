using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyLogic.TGMCProject.Core
{
    public class TrainingDataContainer
    {
        // TODO: Make an internal representation of training data. Include support for 
        // getting counts of a particular value in a column

        // Maps column index to a map of <value, <countFalse, countTrue>>
        private IList<Dictionary<IConvertible, int[]>> _counts;

        public TrainingDataContainer()
        {
            _counts = new List<Dictionary<IConvertible, int[]>>();
        }

        public void AddDataRow(IList<float> row, bool rowClassification)
        {
            // Includes all the data it's given - strip the row ID, question ID, and classification yourself

            bool firstRowAdded = _counts.Count() <= 0;

            for(int i = 0; i < row.Count(); i++)
            {
                // If this is the first row to be added, add a dictionary to counts for each row element
                if(firstRowAdded) _counts.Add(new Dictionary<IConvertible, int[]>());
                
                if(!_counts[i].ContainsKey(row[i]))
                {
                    // Add the value of the column as a key if it has not been seen yet
                    _counts[i][row[i]] = new int[2] {0, 0};

                }

                _counts[i][row[i]][rowClassification ? 1 : 0]++;
            }
        }

        public int GetCount(int columnIndex, IConvertible value, bool rowClassification)
        {
            return _counts[columnIndex][value][Convert.ToInt32(rowClassification)];
        }

        public override string ToString()
        {
            string toReturn = "";

            System.Console.WriteLine("Number of columns: " + _counts.Count());

            for (int i = 0; i < _counts.Count(); i++ )
            {
                toReturn += "Column " + i + ":\n[";

                System.Console.WriteLine("Keys in the dictionary for line " + i + ": " + _counts[i].Keys.Count());

                foreach (IConvertible key in _counts[i].Keys)
                {
                    int[] val = _counts[i][key];
                    toReturn += "\t[" + key + " [" + val[0] + ", " + val[1] +"]]\n";
                }

                toReturn += "]\n";
            }

            return toReturn;
        }
    }
}
