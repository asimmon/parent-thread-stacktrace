using System;
using System.Runtime.Serialization;

namespace ThreadStacktrace
{
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
            var stackTraceText = base.ToString();

            if (ParentThreads.TryGetParentStackTrace(out var stackTrace))
            {
                stackTraceText += Environment.NewLine + stackTrace.ToString();
            }

            return stackTraceText;
        }
    }
}