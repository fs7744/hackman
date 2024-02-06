// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Hackman.Router.Benchmark;
new RouterTest().PriorityPathRouterGet();
var summary = BenchmarkRunner.Run<RouterTest>();

//var a = new ARTTest();
//Console.WriteLine(a.DictionarySearch());
//Console.WriteLine(a.ArtTreeSearch());
//Console.WriteLine(a.AdaptiveRadixTreeTest());
Console.WriteLine(1);