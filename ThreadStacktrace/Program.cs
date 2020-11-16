using System;
using System.Threading;

namespace ThreadStacktrace
{
    public static class Program
    {
        public static void Main()
        {
            Threads();

            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }

        private static void Tasks()
        {
            Console.WriteLine($"Thread #{Thread.CurrentThread.ManagedThreadId}: running {nameof(Tasks)}");
            using var taskGroup = new TaskExecutor();
            taskGroup.Execute(state => FirstLongOperation(), (object)null);
        }

        private static void Threads()
        {
            Console.WriteLine($"Thread #{Thread.CurrentThread.ManagedThreadId}: running {nameof(Threads)}");

            var t1 = ParentThreadAwareThreadFactory.Create(FirstLongOperation, new ThreadCreationOptions
            {
                IsBackground = true,
                Name = "T1"
            });

            t1.Start();
            t1.Join();
        }

        private static void FirstLongOperation()
        {
            Console.WriteLine($"Thread #{Thread.CurrentThread.ManagedThreadId}: running {nameof(FirstLongOperation)}");

            var t2 = ParentThreadAwareThreadFactory.Create(SecondLongOperation, new ThreadCreationOptions
            {
                IsBackground = true,
                Name = "T2"
            });

            t2.Start();
            t2.Join();
        }

        private static void SecondLongOperation()
        {
            Console.WriteLine($"Thread #{Thread.CurrentThread.ManagedThreadId}: running {nameof(SecondLongOperation)}");

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
                throw new ParentThreadAwareException(ex.Message, ex);
            }
        }
    }
}