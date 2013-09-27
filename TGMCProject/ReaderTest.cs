using System;
using System.IO;
using System.Text;
using FuzzyLogic.TGMCProject.Core;

namespace FuzzyLogic.TGMCProject
{
    public static class ReaderTest
    {
        public static void Main()
        {
            var reader = new StreamCSVReader(new MemoryStream(Encoding.ASCII.GetBytes("1,2,-123.0,-1.7\n2,2,-1.0,-1.73\n3,2,-1.04,-1.65")), false);
            Console.Out.WriteLine("created reader...");
            while (reader.NextRecord())
            {
                // Read the question ID
                var rID = reader.ReadChunk<int>();
                var qID = reader.ReadChunk<int>();

                Console.Out.Write("Question ID: {0} Row: {1} Features:", qID, rID);

                // Read the features
                while (reader.HasChunkInRecord())
                {
                    var chunk = reader.ReadChunk<float>();
                    Console.Out.Write("{0}, ", chunk);
                }

                Console.Out.WriteLine();
            }

            while (true)
            {
            }
        }
    }
}
