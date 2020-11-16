using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThreadStacktrace
{
    public sealed class SingleThreadTaskScheduler : TaskScheduler
    {
        public override int MaximumConcurrencyLevel => 1;

        protected override void QueueTask(Task task)
        {
            this.TryExecuteTask(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            this.TryExecuteTask(task);
            return true;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return Enumerable.Empty<Task>();
        }
    }
}