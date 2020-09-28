using System;
using System.Threading;

namespace ThreadStacktrace
{
    public static class Program
    {
        public static void Main()
        {
            var t1 = ParentThreads.Create(FirstLongOperation, new ThreadCreationOptions
            {
                IsBackground = true,
                Name = "T1"
            });

            t1.Start();
            t1.Join();
        }

        private static void FirstLongOperation()
        {
            var t2 = ParentThreads.Create(SecondLongOperation, new ThreadCreationOptions
            {
                IsBackground = true,
                Name = "T2"
            });

            t2.Start();
            t2.Join();
        }

        private static void SecondLongOperation()
        {
            try
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                ThrowFatalError();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void ThrowFatalError()
        {
            try
            {
                throw new Exception("Something bad happened");
            }
            catch (Exception ex)
            {
                throw new ParentThreadException(ex.Message, ex);
            }
        }
    }
}