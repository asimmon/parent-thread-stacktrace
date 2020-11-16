using System;
using System.Threading.Tasks;

namespace ThreadStacktrace
{
    public static class TaskFactoryExtensions
    {
        public static Task<TResult> StartNew<TState, TResult>(this TaskFactory taskFactory, Func<TState, TResult> function, TState state)
        {
            return taskFactory.StartNew(s => function((TState)s), state);
        }

        public static Task<TResult> StartNew<TState, TResult>(this TaskFactory taskFactory, Func<TState, TResult> function, TState state, TaskCreationOptions creationOptions)
        {
            return taskFactory.StartNew(s => function((TState)s), state, creationOptions);
        }

        public static Task StartNew<TState>(this TaskFactory taskFactory, Action<TState> action, TState state)
        {
            return taskFactory.StartNew(s => action((TState)s), state);
        }

        public static Task StartNew<TState>(this TaskFactory taskFactory, Action<TState> action, TState state, TaskCreationOptions creationOptions)
        {
            return taskFactory.StartNew(s => action((TState)s), state, creationOptions);
        }
    }
}