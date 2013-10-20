using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyLogic.TGMCProject.Core
{
    // Provides specialized methods for parsing TGMC data, allowing for fixed row length
    public static class TGMCReaderExtensions
    {
        private static IList<StreamCSVReader> _readers = new List<StreamCSVReader>(1);
        private static int _readerLength = -1;
        public static bool GetTGMCRow(this StreamCSVReader reader, bool isTraining, out float questionId, out float answerId, out IList<float> features, out bool isCorrect) 
        {
            if (!_readers.Contains(reader))
            {
                // Read the length
                _readerLength = reader.NumberColumns;
            }

            questionId = -1;
            answerId = -1;
            features = new List<float>();
            isCorrect = false;

            if (!reader.NextRecord()) return false;

            questionId = reader.ReadChunk<float>();
            reader.HasChunkInRecord();
            answerId = reader.ReadChunk<float>();
            reader.HasChunkInRecord();

            // Now, read until the end of the features
            for (int i = 2; (isTraining ? _readerLength - 1 : _readerLength) > i && reader.HasChunkInRecord(); i++)
            {
                features.Add(reader.ReadChunk<float>());
            }

            if (isTraining && reader.HasChunkInRecord())
            {
                isCorrect = reader.ReadChunk<Boolean>();
            }
            else
            {
                if (isTraining)
                {
                    Console.Out.WriteLine("requested training data but we didn't have a chunk for the row?");
                }
            }

            return true;
        }
    }
}
