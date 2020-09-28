using System;
using System.Runtime.Serialization;

namespace ThreadStacktrace
{
    public class ParentThreadException : Exception
    {
        public ParentThreadException()
        {
        }

        public ParentThreadException(string message)
            : base(message)
        {
        }

        public ParentThreadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ParentThreadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override string ToString()
        {
            var stackTraceText = base.ToString();

            if (ParentThreads.TryGetParentStackTrace(out var stackTrace))
            {
                stackTraceText += Environment.NewLine + stackTrace;
            }

            return stackTraceText;
        }
    }
}