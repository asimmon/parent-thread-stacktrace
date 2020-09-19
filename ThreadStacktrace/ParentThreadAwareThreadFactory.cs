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

            ParentThreads.Register(thread);

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
                    ParentThreads.UnregisterCurrentThread();
                }
            }

            return Wrapper;
        }
    }
}