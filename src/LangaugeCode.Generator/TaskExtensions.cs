using System.Threading.Tasks;

namespace Panlingo.LangaugeCode.Generator
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
