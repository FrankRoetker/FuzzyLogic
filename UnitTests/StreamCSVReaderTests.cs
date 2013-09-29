using System;
using System.IO;
using System.Text;
using FuzzyLogic.TGMCProject.Core;
using NUnit.Framework;

namespace FuzzyLogic.UnitTests
{
    [TestFixture]
    class StreamCSVReaderTests
    {
        [Test]
        public void TestReadingNormalCSV()
        {
            var reader = new StreamCSVReader(new MemoryStream(Encoding.ASCII.GetBytes("1,2,-123.0,-1.7\n2,2,-1.0,-1.73\n3,2,-1.04,-1.65")));

            int[] expectedInts = {1, 2, 2, 2, 3, 2};
            double[] expectedDoubles = {-123.0, -1.7, -1.0, -1.73, -1.04, -1.65};

            for (var row = 0; row < 3; row++)
            {
                Assert.IsTrue(reader.NextRecord());
                for (var column = 0; column < 4; column++)
                {
                    Assert.IsTrue(reader.HasChunkInRecord());
                    if (column < 2)
                        Assert.AreEqual(expectedInts[(row * 2) + column], reader.ReadChunk<int>());
                    else
                        Assert.AreEqual(expectedDoubles[(row * 2) + (column - 2)], reader.ReadChunk<double>());
                }

                Assert.IsFalse(reader.HasChunkInRecord());
            }

            Assert.IsFalse(reader.NextRecord());
        }

        [Test]
        public void TestReadingWithQuotes()
        {
            var reader = new StreamCSVReader(new MemoryStream(Encoding.ASCII.GetBytes("'a','b','c'")));

            String[] expectedStrings = {"a", "b", "c"};
            Assert.IsTrue(reader.NextRecord());

            for (var column = 0; column < 3; column++)
            {
                Assert.AreEqual(reader.ReadChunk<String>(), expectedStrings[column]);
            }
        }

        [Test]
        public void TestReadingWithQuotedNewlines()
        {
            var reader = new StreamCSVReader(new MemoryStream(Encoding.ASCII.GetBytes("'a\n\n','b\n','c\n'")));

            String[] expectedStrings = { "a\n\n", "b\n", "c\n" };
            Assert.IsTrue(reader.NextRecord());

            for (var column = 0; column < 3; column++)
            {
                Assert.AreEqual(reader.ReadChunk<String>(), expectedStrings[column]);
            }

            Assert.IsFalse(reader.HasChunkInRecord());
            Assert.IsFalse(reader.NextRecord());
        }

        [Test]
        public void TestReadingWithEscapedCharacters()
        {
            var reader = new StreamCSVReader(new MemoryStream(Encoding.ASCII.GetBytes("a,b,c\\,d,e")));

            String[] expectedStrings = { "a", "b", "c,d", "e" };
            Assert.IsTrue(reader.NextRecord());

            for (var column = 0; column < 4; column++)
            {
                Assert.AreEqual(reader.ReadChunk<String>(), expectedStrings[column]);
            }

            Assert.IsFalse(reader.HasChunkInRecord());
            Assert.IsFalse(reader.NextRecord());
        }
    }
}
