using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ThreadStacktrace
{
    public sealed class StackedStackTrace
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