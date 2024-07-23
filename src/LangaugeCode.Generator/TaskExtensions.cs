using System.Threading.Tasks;

namespace LangaugeCode.Generator
{
    public static class TaskExtensions
    {
        public static T GetResult<T>(this Task<T> task)
        {
            task.Wait();
            return task.Result;
        }
    }
}
