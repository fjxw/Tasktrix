using System.Runtime.CompilerServices;

namespace Tasktrix.ViewModels
{
    public class TaskAwaiter<T>
    {
        readonly Lazy<Task<T>> instance;

        public TaskAwaiter(Func<T> factory)
        {
            instance = new Lazy<Task<T>> (() => Task.Run(factory));
        }

        public TaskAwaiter(Func<Task<T>> factory)
        {
            instance = new Lazy<Task<T>> (() => Task.Run(factory));
        }

        public System.Runtime.CompilerServices.TaskAwaiter<T> GetAwaiter()
        {
            return instance.Value.GetAwaiter();
        }

        public void Start()
        {
            var unused = instance.Value;
        }
    }
}
