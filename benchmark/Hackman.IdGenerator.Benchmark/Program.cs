using BenchmarkDotNet.Running;
using Hackman;
using System.Diagnostics;
using System.Linq;

//var summary = BenchmarkRunner.Run<IdGeneratorTest>();
var idGenerator = new IdGenerator();
foreach (var item in Enumerable.Range(0, 5).Select(i => idGenerator.NextId()))
{
    Console.WriteLine(item);
}
var i = 8000000;
var stopwatch = new Stopwatch();
stopwatch.Start();
Console.WriteLine(System.Linq.Enumerable.Range(0, i).Select(i => idGenerator.NextId()).Distinct().Count());
stopwatch.Stop();
Console.WriteLine(stopwatch.ElapsedMilliseconds);