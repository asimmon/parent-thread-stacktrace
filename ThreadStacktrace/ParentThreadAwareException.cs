using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;

namespace ThreadStacktrace
{
    [Serializable]
    public class ParentThreadAwareException : Exception
    {
        public ParentThreadAwareException()
        {
        }

        public ParentThreadAwareException(string message)
            : base(message)
        {
        }

        public ParentThreadAwareException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ParentThreadAwareException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override string ToString()
        {
            var currentThreadId = Thread.CurrentThread.ManagedThreadId;

            var stackTraceText = base.ToString();

            if (ThreadStackTrace.TryGetParentThreadStackTrace(currentThreadId, out var parentThreadStackTrace))
            {
                return "Current thread #" + currentThreadId.ToString(CultureInfo.InvariantCulture) + ":" + Environment.NewLine + stackTraceText + Environment.NewLine + parentThreadStackTrace;
            }

            return stackTraceText;
        }
    }
}