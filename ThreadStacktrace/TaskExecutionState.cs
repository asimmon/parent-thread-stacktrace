using System.Threading;

namespace ThreadStacktrace
{
    public sealed class TaskExecutionState<TState> where TState : new()
    {
        public TaskExecutionState(TState actionState, int parentThreadId, ThreadStackTrace threadStackTrace, CancellationToken token)
        {
            this.ActionState = actionState;
            this.ParentThreadId = parentThreadId;
            this.ThreadStackTrace = threadStackTrace;
            this.Token = token;
        }

        public TState ActionState { get; }

        public int ParentThreadId { get; }

        public ThreadStackTrace ThreadStackTrace { get; }

        public CancellationToken Token { get; }
    }
}