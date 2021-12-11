using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StowawayPS
{
    public class Action
    {
        public const int COUNT = 4;

        public const int LEFT = 0;
        public const int UP = 1;
        public const int RIGHT = 2;
        public const int DOWN = 3;
    }
    public class Utils
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
        
        public static double calculateMean(List<int> data)
        {
            double total = 0.0;
            for (int i = 0; i < data.Count; i++)
            {
                total += data[i];
            }

            return total / data.Count;
        }

        public static double calculateMean(List<double> data)
        {
            double total = 0.0;
            for (int i = 0; i < data.Count; i++)
            {
                total += data[i];
            }

            return Math.Round(total / data.Count, 2);
        }

        public static double calculateCov(List<int> data)
        {
            double mean = calculateMean(data);
            double total = 0.0;
            for (int i = 0; i < data.Count; i++)
            {
                total += Math.Pow((double)data[i] - mean, 2);
            }

            return total / data.Count;
        }
        public static double calculateCov(List<double> data)
        {
            double mean = calculateMean(data);
            double total = 0.0;
            for (int i = 0; i < data.Count; i++)
            {
                total += Math.Pow(data[i] - mean, 2);
            }

            return Math.Round(total / data.Count, 2);
        }

        internal static void testVariance()
        {
            List<int> test = new List<int>();
            for (int j = 0; j < 9; j++)
            {
                test.Add(30);
            }
            test.Add(32);
            double covTest = Utils.calculateCov(test);
        }
    }
}
