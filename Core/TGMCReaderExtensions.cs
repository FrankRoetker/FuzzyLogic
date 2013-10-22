using System;
using System.Collections.Generic;

namespace FuzzyLogic.TGMCProject.Core
{
    // Provides specialized methods for parsing TGMC data, allowing for fixed row length
    public static class TGMCReaderExtensions
    {
        public static bool GetTGMCRow(this StreamCSVReader reader, bool isTraining, out float questionId, out float answerId, out IList<float> features, out bool isCorrect) 
        {
            questionId = -1;
            answerId = -1;
            features = new List<float>(reader.NumberColumns);
            isCorrect = false;

            if (!reader.NextRecord()) return false;

            questionId = reader.ReadChunkFloat();
            reader.HasChunkInRecord();
            answerId = reader.ReadChunkFloat();
            reader.HasChunkInRecord();

            // Now, read until the end of the features
            for (var i = 2; (isTraining ? reader.NumberColumns - 1 : reader.NumberColumns) > i && reader.HasChunkInRecord(); i++)
            {
                features.Add(reader.ReadChunkFloat());
            }

            if (isTraining && reader.HasChunkInRecord())
            {
                isCorrect = reader.ReadChunkBoolean();
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
