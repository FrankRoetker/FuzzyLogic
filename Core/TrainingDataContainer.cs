using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLogic.TGMCProject.Core
{
    public class TrainingDataContainer
    {
        // TODO: Make an internal representation of training data. Include support for 
        // getting counts of a particular value in a column

        // Maps column index to a map of <value, <countFalse, countTrue>>
        private readonly IList<Dictionary<IConvertible, int[]>> _counts;

        public TrainingDataContainer()
        {
            _counts = new List<Dictionary<IConvertible, int[]>>();
        }

        public void AddDataRow(IList<double> row, bool rowClassification)
        {
            // Includes all the data it's given - strip the row ID, question ID, and classification yourself

            var firstRowAdded = !_counts.Any();

            for(var i = 0; i < row.Count; i++)
            {
                // If this is the first row to be added, add a dictionary to counts for each row element
                if (firstRowAdded)
                {
                    _counts.Add(new Dictionary<IConvertible, int[]>());
                }

                var dict = _counts[i];
                var r = row[i];

                if(!dict.ContainsKey(r))
                {
                    // Add the value of the column as a key if it has not been seen yet
                    dict[r] = new[] {0, 0};

                }

                dict[r][rowClassification ? 1 : 0]++;
            }
        }

        public int GetCount(int columnIndex, IConvertible value, bool rowClassification)
        {
            return _counts[columnIndex][value][Convert.ToInt32(rowClassification)];
        }

        public float GetOddsOfTrue(int columnIndex, IConvertible value)
        {
            if (_counts.Count >= columnIndex && _counts[columnIndex].ContainsKey(value))
            {
                var FTcounts = _counts[columnIndex][value];
                return (float) FTcounts[1]/FTcounts[0];
            }

            return 0.001f;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(6*_counts.Count);
            
            Console.WriteLine("Number of columns: " + _counts.Count);

            for (var i = 0; i < _counts.Count; i++ )
            {
                builder.AppendFormat("Column {0}:\n[", i);

                Console.WriteLine("Keys in the dictionary for line " + i + ": " + _counts[i].Keys.Count);

                foreach (var key in _counts[i].Keys)
                {
                    var val = _counts[i][key];
                    builder.AppendFormat("\t[{0} [{1}, {2}]]\n", key, val[0], val[1]);
                }

                builder.Append("]\n");
            }

            return builder.ToString();
        }
    }
}
