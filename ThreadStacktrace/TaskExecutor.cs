using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadStacktrace
{
    public class TaskExecutor : IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private readonly TaskFactory _taskFactory;

        public TaskExecutor()
        {
            var taskScheduler = TaskScheduler.Current;
            this._cts = new CancellationTokenSource();
            this._taskFactory = new TaskFactory(this._cts.Token, TaskCreationOptions.AttachedToParent, TaskContinuationOptions.AttachedToParent, taskScheduler);
        }

        public void Execute<TState>(Action<TState> action, TState actionState)
            where TState : new()
        {
            void ActionWrapper(TaskExecutionState<TState> scopedTaskState)
            {
                var currentThreadId = Thread.CurrentThread.ManagedThreadId;
                var canRegisterParentStacktrace = currentThreadId != scopedTaskState.ParentThreadId;

                try
                {
                    if (canRegisterParentStacktrace)
                    {
                        ThreadStackTrace.RegisterParentThreadStackTrace(currentThreadId, scopedTaskState.ThreadStackTrace);
                    }

                    action(scopedTaskState.ActionState);
                }
                finally
                {
                    if (canRegisterParentStacktrace)
                    {
                        ThreadStackTrace.UnregisterParentThreadStackTrace(currentThreadId);
                    }
                }
            }

            this._cts.Token.ThrowIfCancellationRequested();
            var stackedStackTrace = ThreadStackTrace.GetCurrentThreadStackTrace();
            var taskState = new TaskExecutionState<TState>(actionState, Thread.CurrentThread.ManagedThreadId, stackedStackTrace, this._cts.Token);
            this._taskFactory.StartNew(ActionWrapper, taskState, this._taskFactory.CreationOptions);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._cts.Dispose();
            }
        }
    }
}