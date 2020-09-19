using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace ThreadStacktrace
{
    public static class ParentThreads
    {
        private static readonly ConcurrentDictionary<int, StackedStackTrace> ParentStackTraces = new ConcurrentDictionary<int, StackedStackTrace>();

        public static void Register(Thread thread)
        {
            ParentStackTraces.TryAdd(thread.ManagedThreadId, GetParentStackTrace());
        }

        public static void UnregisterCurrentThread()
        {
            ParentStackTraces.TryRemove(Thread.CurrentThread.ManagedThreadId, out _);
        }

        public static bool TryGetParentStackTrace(out IStackedStackTrace parentStackTrace)
        {
            if (ParentStackTraces.TryGetValue(Thread.CurrentThread.ManagedThreadId, out var stackTrace))
            {
                parentStackTrace = stackTrace;
                return true;
            }

            parentStackTrace = null;
            return false;
        }

        public static int Count()
        {
            return ParentStackTraces.Count;
        }

        private static StackedStackTrace GetParentStackTrace()
        {
            var stackTrace = ParentStackTraces.TryGetValue(Thread.CurrentThread.ManagedThreadId, out var parentStackTrace)
                ? parentStackTrace.Clone()
                : new StackedStackTrace();

            stackTrace.Add(new StackTrace(2, true));

            return stackTrace;
        }

        public interface IStackedStackTrace
        {
            string ToString();
        }

        private class StackedStackTrace : IStackedStackTrace
        {
            private readonly IList<StackTrace> _stackTraces;

            public StackedStackTrace()
            {
                this._stackTraces = new List<StackTrace>();
            }

            private StackedStackTrace(IEnumerable<StackTrace> stackTraces)
            {
                this._stackTraces = new List<StackTrace>(stackTraces);
            }

            public void Add(StackTrace stackTrace)
            {
                this._stackTraces.Add(stackTrace);
            }

            public StackedStackTrace Clone()
            {
                return new StackedStackTrace(this._stackTraces);
            }

            public override string ToString()
            {
                var sb = new StringBuilder();

                for (var i = this._stackTraces.Count - 1; i >= 0; i--)
                {
                    sb.Append("Parent thread ").Append(i).AppendLine(":");
                    sb.Append(this._stackTraces[i]);
                }

                return sb.ToString();
            }
        }
    }
}