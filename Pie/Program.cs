using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi
{
    class Program
    {
        public static void SerialFunction()
        {
            long num_steps = 100000000;
            Stopwatch timer = Stopwatch.StartNew();
            double step=1.0/num_steps;
            double x, pi, sum = 0.0;
            for (double i = 0; i <= num_steps; i++)
            {
                x = (i - 0.5) * step;
                sum = sum + 4.0 / (1.0 + x * x);
            }
            pi = step * sum;
            timer.Stop();
            Console.WriteLine("\n SerialFunction Pi with {0} steps is {1} in {2} miliseconds", num_steps, pi, timer.ElapsedMilliseconds);
        }

        public static void ParallelFunction()
        {
            object lockObject = new object();
            long num_steps = 100000000;
            Stopwatch timer = Stopwatch.StartNew();
            double step = 1.0 / num_steps;
            double sum = 0;
            Parallel.For(1, num_steps + 1, () => 0.0, (i, loopState, partialResult) =>
              {
                  var x = (i - 0.5) * step;
                  return partialResult + 4.0 / (1.0 + x * x);
              },
            localPartialSum =>
            {
                lock (lockObject)
                {
                    sum += localPartialSum;
                }
            });
            var pi = step * sum;
            timer.Stop();
            Console.WriteLine("\n ParallelFunction Pi with {0} steps is {1} in {2} miliseconds", num_steps, pi, timer.ElapsedMilliseconds);
        }

        private static double fun(long start, long end)
        {
            long num_steps = 100000000;
            double step = 1.0 / num_steps;
            double sum = 0.0;
            for (int i = (int)start;  i<=end; i++)
            {
                double x = (i - 0.5) * step;
                sum = sum + 4.0 / (1.0 + x * x);
            }
            return sum;
        }
        public static void TaskFunction()
        {
            long num_steps = 100000000;
            double step = 1.0 / num_steps;
            double pi, sum = 0.0;
            Stopwatch timer = Stopwatch.StartNew();
            Task<double> task1 = new Task<double>(() => fun(0, num_steps / 4));
            Task<double> task2 = new Task<double>(() => fun(num_steps / 4, num_steps / 2));
            Task<double> task3 = new Task<double>(() => fun(num_steps / 2, num_steps / 2+num_steps / 4));
            Task<double> task4 = new Task<double>(() => fun(num_steps / 2 + num_steps / 4, num_steps));
            task1.Start();
            task2.Start();
            task3.Start();
            task4.Start();
            double sum1 = task1.Result;
            double sum2 = task2.Result;
            double sum3 = task3.Result;
            double sum4 = task4.Result;
            sum += sum1 + sum2 + sum3 + sum4;
            pi = step * sum;
            timer.Stop();
            Console.WriteLine("\n TaskFunction Pi with {0} steps is {1} in {2} miliseconds", num_steps, pi, timer.ElapsedMilliseconds);
        }

        static void Main(string[] args)
        {
            TaskFunction();
            ParallelFunction();
            SerialFunction();
        }

    }
}
