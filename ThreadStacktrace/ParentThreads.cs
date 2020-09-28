using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace ThreadStacktrace
{
    public static class ParentThreads
    {
        private static readonly ConcurrentDictionary<int, StackedStackTrace> ParentStackTraces = new ConcurrentDictionary<int, StackedStackTrace>();

        public static Thread Create(ThreadStart start, ThreadCreationOptions options)
        {
            var thread = new Thread(WrapWithParentStackTrace(start))
            {
                IsBackground = options.IsBackground
            };

            if (!string.IsNullOrEmpty(options.Name))
            {
                thread.Name = options.Name;
            }

            return thread;
        }

        private static ThreadStart WrapWithParentStackTrace(ThreadStart start)
        {
            var parentStackTrace = GetParentStackTrace();

            void Wrapper()
            {
                var currentThreadId = Thread.CurrentThread.ManagedThreadId;
                ParentStackTraces.TryAdd(currentThreadId, parentStackTrace);

                try
                {
                    start();
                }
                finally
                {
                    ParentStackTraces.TryRemove(currentThreadId, out _);
                }
            }

            return Wrapper;
        }

        private static StackedStackTrace GetParentStackTrace()
        {
            var stackTrace = ParentStackTraces.TryGetValue(Thread.CurrentThread.ManagedThreadId, out var parentStackTrace)
                ? parentStackTrace.Clone()
                : new StackedStackTrace();

            stackTrace.Add(new StackTrace(2, true));

            return stackTrace;
        }

        public static bool TryGetParentStackTrace(out StackedStackTrace parentStackTrace)
        {
            if (ParentStackTraces.TryGetValue(Thread.CurrentThread.ManagedThreadId, out var stackTrace))
            {
                parentStackTrace = stackTrace;
                return true;
            }

            parentStackTrace = null;
            return false;
        }
    }
}