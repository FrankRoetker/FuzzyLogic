using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libsvm;

namespace FuzzySVM
{
    class FuzzyProblemHelper
    {
        public struct temp
        {
            public int answerId;
            public double type;
            public svm_node[] nodes;
        }

        private const int SAMPLE = 30;

        public static PersonalProblem ReadProblem(string input_file_name, out List<temp> nodes, bool testing = false)
        {
            var stuff = new List<temp>();

            var vy = new List<double>();
            var vx = new List<svm_node[]>();
            using (StreamReader sr = new StreamReader(input_file_name))
            {
                while (true)
                {
                    var line = sr.ReadLine();
                    if (line == null) break;

                    var st = line.Split(',').Where(c => c != String.Empty).ToArray();

                    temp tmp = new temp();
                    //vy.Add(testing ? atob(st[st.Length - 1]) : 0.0);
                    tmp.type = testing ? atob(st[st.Length - 1]) : 0.0;
                    tmp.answerId = atoi(st[0]);

                    var m = (st.Count() - (testing ? 3 : 2));
                    var x = new List<svm_node>();
                    for (var i = 0; i < m; i++)
                    {
                        var value = double.Parse(st[i + 2].Trim());

                        //we only want to add the values that are non-zero
                        if (!value.Equals(0.0))
                            x.Add(new svm_node
                            {
                                index = i + 1,
                                value = value,
                            });
                    }
                    //vx.Add(x.ToArray());
                    tmp.nodes = x.ToArray();
                    stuff.Add(tmp);
                }

            }
            if (testing)
            {
                var rnd = new Random();
                nodes = stuff.Where(e => e.type.Equals(1.0)).ToList();
                var negNodes = stuff.Where(e => e.type.Equals(0.0)).OrderBy(e => rnd.Next()).Take(SAMPLE).ToList();
                nodes.AddRange(negNodes);
            }
            else
            {
                nodes = stuff;
            }

            vy = nodes.Select(e => e.type).ToList();
            vx = nodes.Select(e => e.nodes).ToList();
            var answers = nodes.Select(e => e.answerId).ToList();

            var prob = new PersonalProblem {l = vy.Count, x = vx.ToArray(), y = vy.ToArray(), answerId = answers.ToArray()};

            return prob;
        }

        public static PersonalProblem ScaleProblem(PersonalProblem prob, double lower = -1.0, double upper = 1.0)
        {
            var index_max = prob.x.Max(X => X.Max(e => e.index));
            var feature_max = new double[(index_max + 1)];
            var feature_min = new double[(index_max + 1)];
            int n = prob.l;

            for (int i = 0; i <= index_max; i++)
            {
                feature_max[i] = -Double.MaxValue;
                feature_min[i] = Double.MaxValue;
            }

            for (int i = 0; i < n; i++)
            {
                var m = prob.x[i].Count();
                for (int j = 0; j < m; j++)
                {
                    var index = prob.x[i][j].index;
                    feature_max[index - 1] = Math.Max(feature_max[index - 1], prob.x[i][j].value);
                    feature_min[index - 1] = Math.Min(feature_min[index - 1], prob.x[i][j].value);
                }
            }

            var scaledProb = new PersonalProblem();
            scaledProb.l = n;
            scaledProb.y = prob.y.ToArray();
            scaledProb.x = new svm_node[n][];
            scaledProb.answerId = new int[n];
            for (int i = 0; i < n; i++)
            {
                var m = prob.x[i].Count();
                scaledProb.x[i] = new svm_node[m];
                scaledProb.answerId[i] = prob.answerId[i];
                for (int j = 0; j < m; j++)
                {
                    var index = prob.x[i][j].index;
                    var value = prob.x[i][j].value;
                    var max = feature_max[index - 1];
                    var min = feature_min[index - 1];

                    scaledProb.x[i][j] = new svm_node() { index = index };

                    if (min == max)
                        scaledProb.x[i][j].value = 0;
                    else
                        scaledProb.x[i][j].value = lower + (upper - lower) * (value - min) / (max - min);
                }
            }
            return scaledProb;
        }

        public static PersonalProblem ReadAndScaleProblem(string input_file_name, out List<temp> nodes, double lower = -1.0, double upper = 1.0, bool testing = true)
        {
            return ScaleProblem(ReadProblem(input_file_name, out nodes, testing), lower, upper);
        }

        public static void WriteProblem(string output_file_name, PersonalProblem problem)
        {
            using (StreamWriter sw = new StreamWriter(output_file_name))
            {
                for (int i = 0; i < problem.l; i++)
                {
                    var sb = new StringBuilder();
                    sb.AppendFormat("{0} ", problem.y[i]);
                    for (int j = 0; j < problem.x[i].Count(); j++)
                    {
                        var node = problem.x[i][j];
                        sb.AppendFormat("{0}:{1} ", node.index, node.value);
                    }
                    sw.WriteLine(sb.ToString().Trim());
                }
                sw.Close();
            }
        }

        public static double atof(String s)
        {
            double d = Double.Parse(s, usCulture);
            if (Double.IsNaN(d) || Double.IsInfinity(d))
            {
                throw new FormatException(String.Format("'{0}' is not a valid Double value", s));
            }
            return (d);
        }
        public static int atoi(String s)
        {
            return Int32.Parse(s, usCulture);
        }
        public static double atod(String s)
        {
            return Double.Parse(s, usCulture);
        }
        public static double atob(String s)
        {
            return Boolean.Parse(s) ? 1.0 : 0.0;
        }
        private static CultureInfo usCulture = new CultureInfo("en-US");
    }

    public class PersonalProblem : svm_problem
    {
        public int[] answerId;
    }
}
