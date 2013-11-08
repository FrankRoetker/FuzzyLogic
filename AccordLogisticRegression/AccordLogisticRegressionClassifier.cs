using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;
using FuzzyLogic.TGMCProject.Core;
using System.IO;
using System.Threading;

namespace FuzzyLogic.TGMCProject.AccordLogisticRegression
{
    public class AccordLogisticRegressionClassifier : IClassifier
    {
        private static int ORACLE_SIZE = 270000; // # of records in each oracle
        private static int MAX_ORACLES = 1; // # of oracles

        private static double DELTA_THRESHOLD = 0.001;
        private static int THRESHOLD_COUNT = 5; // How long the model should stabilize before calling it done

        public IDictionary<int, int> _oracleStatus = new Dictionary<int, int>();

        public Task<LogisticRegression> StartOracle(int oracle, int numberColumns, IList<double[]> inputs, IList<double[]> outputs)
        {
            var input = inputs.ToArray();
            var output = outputs.ToArray();

            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Starting Oracle {0} with {1} rows", oracle, inputs.Count);

                var regression = new LogisticRegression(numberColumns);
                var teacher = new IterativeReweightedLeastSquares(regression);

                int thresholdCount = 0;
                double delta;
                do
                {
                    // Perform an iteration
                    delta = teacher.Run(input, output);

                    Console.Out.WriteLine("Oracle {1} Delta: {0} (count: {2})", delta, oracle, thresholdCount);
                } while (ContinueTraining(oracle, delta));

                Console.WriteLine("Oracle {0} done", oracle);

                return regression;
            });
        }

        /// <summary>
        /// Returns true if should continue training
        /// </summary>
        public bool ContinueTraining(int oracleId, double delta)
        {
            lock (_oracleStatus)
            {
                bool isAbove = delta > DELTA_THRESHOLD;

                if (!_oracleStatus.ContainsKey(oracleId))
                    _oracleStatus[oracleId] = 0;

                if (isAbove)
                    _oracleStatus[oracleId]++;

                return isAbove || _oracleStatus.Any(pair => pair.Value < THRESHOLD_COUNT);
            }
        }

        public void TrainClassifier(StreamCSVReader reader)
        {
            Console.WriteLine("reading training data...");

            IList<double[]> correct = new List<double[]>();
            IList<double[]> incorrect = new List<double[]>();

            IList<IList<double[]>> inputs = new List<IList<double[]>>();
            IList<IList<double[]>> outputs = new List<IList<double[]>>();
            IList<Task<LogisticRegression>> tasks = new List<Task<LogisticRegression>>();

            inputs.Add(new List<double[]>(ORACLE_SIZE));
            outputs.Add(new List<double[]>(ORACLE_SIZE));


            // Separate all the correct and incorrect answers in the training set
            while (reader.NextRecord())
            {
                float questionId, answerId;
                IList<double> features;
                bool isCorrect;

                if (!reader.GetTGMCRow(true, out questionId, out answerId, out features, out isCorrect))
                {
                    Console.Out.WriteLine("Didn't read row Correctly! :(");
                    continue;
                }

                if (isCorrect)
                {
                    correct.Add(features.ToArray());
                }
                else
                {
                    incorrect.Add(features.ToArray());
                }
            }

            // Uncomment the following to randomize
            correct.Shuffle();
            incorrect.Shuffle();

            int numOracles = (int)Math.Ceiling((double)(correct.Count + incorrect.Count) / ORACLE_SIZE);
            if (numOracles > MAX_ORACLES)
            {
                numOracles = MAX_ORACLES;
            }

            int numCorrectPerOracle = (int)Math.Ceiling(((double)correct.Count) / numOracles);
            int numIncorrectPerOracle = (int)Math.Ceiling(((double)incorrect.Count) / numOracles);

            if (numIncorrectPerOracle > ORACLE_SIZE)
            {
                numIncorrectPerOracle = ORACLE_SIZE - numCorrectPerOracle;
            }

            int currentOracle = 0;
            int count = 0;

            // Evenly distribute the correct answers
            foreach (double[] row in correct)
            {
                if (count++ > numCorrectPerOracle)
                {
                    currentOracle++;
                    count = 0;

                    if (currentOracle >= numOracles)
                    {
                        break;
                    }

                    inputs.Add(new List<double[]>(ORACLE_SIZE));
                    outputs.Add(new List<double[]>(ORACLE_SIZE));
                }

                inputs[currentOracle].Add(row);
                outputs[currentOracle].Add(new[] { 1.0 });
            }

            currentOracle = 0;
            count = 0;

            // Evenly distribute incorrect answers
            foreach (double[] row in incorrect)
            {
                if (count++ > numIncorrectPerOracle)
                {
                    currentOracle++;
                    count = 0;

                    if (currentOracle >= numOracles)
                    {
                        break;
                    }
                }

                inputs[currentOracle].Add(row);
                outputs[currentOracle].Add(new[] { 0.0 });
            }

            // Start the Oracles
            for (var i = 0; i < numOracles; i++)
            {
                tasks.Add(StartOracle(i, reader.NumberColumns - 3, inputs[i], outputs[i]));
            }

            // Wait for all the tasks
            Task.WaitAll(tasks.ToArray());

            // Collect the models
            Models = tasks.Select(x => x.Result).ToList();
        }

        public bool ClassifyRow(double[] row, double questionId, double answerId, StreamWriter dataFile, out double confidence)
        {
            var results = Models.Select(x => x.Compute(row)).ToList();
            results.Sort();

            confidence = (double)results.Count(x => Math.Abs(x - 1.0) < 0.005) / results.Count();

            dataFile.Write("{0},{1}", questionId, answerId);
            foreach (var result in results)
            {
                dataFile.Write(",{0}", result);
            }
            dataFile.Write("\n");

            return results.Max() > 0.50;
        }

        public IList<LogisticRegression> Models { get; set; }
    }

    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }

    static class MyExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}