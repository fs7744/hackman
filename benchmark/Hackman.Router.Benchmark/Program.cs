// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<ARTTest>();

//var a = new ARTTest();
//Console.WriteLine(a.DictionarySearch());
//Console.WriteLine(a.ArtTreeSearch());
//Console.WriteLine(a.AdaptiveRadixTreeTest());