using BenchmarkDotNet.Running;
using Hackman;
using System.Diagnostics;
using System.Linq;

//var summary = BenchmarkRunner.Run<IdGeneratorTest>();
var idGenerator = new UIdGenerator();
var oidGenerator = new IdGenerator();
foreach (var item in Enumerable.Range(0, 5))
{
    var r0 = oidGenerator.NextId();
    Console.WriteLine($"{r0} {Environment.NewLine} {Convert.ToString(r0, 2)}");
    var r = idGenerator.NextId();
    //Console.WriteLine($"{r} {Environment.NewLine} {Convert.ToString(r, 2)}");
}
var i = 8000000;
var stopwatch = new Stopwatch();
stopwatch.Start();
Console.WriteLine(System.Linq.Enumerable.Range(0, i).Select(i => idGenerator.NextId()).Distinct().Count());
stopwatch.Stop();
Console.WriteLine(stopwatch.ElapsedMilliseconds);