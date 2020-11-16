using System.Threading;

namespace ThreadStacktrace
{
    public static class ParentThreadAwareThreadFactory
    {
        public static Thread Create(ThreadStart start, ThreadCreationOptions options)
        {
            var thread = new Thread(WrapWithParentStackTraceRemoval(start))
            {
                IsBackground = options.IsBackground,
                Name = options.Name
            };

            var parentThreadStackTrace = ThreadStackTrace.GetCurrentThreadStackTrace();
            ThreadStackTrace.RegisterParentThreadStackTrace(thread.ManagedThreadId, parentThreadStackTrace);

            return thread;
        }

        private static ThreadStart WrapWithParentStackTraceRemoval(ThreadStart start)
        {
            void Wrapper()
            {
                try
                {
                    start();
                }
                finally
                {
                    ThreadStackTrace.UnregisterParentThreadStackTrace(Thread.CurrentThread.ManagedThreadId);
                }
            }

            return Wrapper;
        }
    }
}