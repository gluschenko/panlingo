using BenchmarkDotNet.Running;
using UserAgentBetchmarks.Tests;

namespace LanguageCode.Benchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<LanguageCodeTest>();

            Console.ReadKey();
        }
    }
}
