using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace ThreadStacktrace
{
    public sealed class ThreadStackTrace
    {
        private static readonly ConcurrentDictionary<int, ThreadStackTrace> ParentThreadStackTraces = new ConcurrentDictionary<int, ThreadStackTrace>();

        private readonly IList<StackTrace> _stackTraces;
        private readonly IList<int> _threadIds;

        private ThreadStackTrace()
        {
            this._stackTraces = new List<StackTrace>();
            this._threadIds = new List<int>();
        }

        private ThreadStackTrace(ThreadStackTrace other)
        {
            this._stackTraces = new List<StackTrace>(other._stackTraces);
            this._threadIds = new List<int>(other._threadIds);
        }

        private void Add(StackTrace stackTrace, int threadId)
        {
            this._stackTraces.Add(stackTrace);
            this._threadIds.Add(threadId);
        }

        private ThreadStackTrace Clone()
        {
            return new ThreadStackTrace(this);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var i = this._stackTraces.Count - 1; i >= 0; i--)
            {
                sb.Append("Parent thread #").Append(this._threadIds[i]).AppendLine(":");
                sb.Append(this._stackTraces[i]);
            }

            return sb.ToString();
        }

        // Static Methods

        public static ThreadStackTrace GetCurrentThreadStackTrace()
        {
            var currentThreadId = Thread.CurrentThread.ManagedThreadId;

            var stackTrace = ParentThreadStackTraces.TryGetValue(currentThreadId, out var parentStackTrace)
                ? parentStackTrace.Clone()
                : new ThreadStackTrace();

            stackTrace.Add(new StackTrace(2, true), currentThreadId);

            return stackTrace;
        }

        public static bool TryGetParentThreadStackTrace(int threadId, out ThreadStackTrace parentThreadStackTrace)
        {
            if (ParentThreadStackTraces.TryGetValue(threadId, out var stackTrace))
            {
                parentThreadStackTrace = stackTrace;
                return true;
            }

            parentThreadStackTrace = null;
            return false;
        }

        public static void RegisterParentThreadStackTrace(int threadId, ThreadStackTrace parentThreadStackTrace)
        {
            ParentThreadStackTraces.TryAdd(threadId, parentThreadStackTrace);
        }

        public static void UnregisterParentThreadStackTrace(int threadId)
        {
            ParentThreadStackTraces.TryRemove(threadId, out _);
        }
    }
}