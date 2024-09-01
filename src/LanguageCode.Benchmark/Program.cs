using BenchmarkDotNet.Running;
using LanguageCode.Benchmark.Tests;

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
