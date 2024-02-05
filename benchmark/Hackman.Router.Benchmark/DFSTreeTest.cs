// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using DotNext.Runtime.Caching;
using Hackman.Router.Radix;
using System.Collections.Concurrent;
using System.Collections.Frozen;

[AllStatisticsColumn]
public class DFSTreeTest
{
    private readonly DFSTree<List<string>> dfs;
    private readonly Dictionary<string, string> dict;
    private readonly FrozenDictionary<string, string> frozenDict;
    private readonly ConcurrentDictionary<string, string> concurrentdict;
    private readonly ConcurrentCache<string, DFSNode<List<string>>> cache;

    public DFSTreeTest()
    {
        var tree = new Tree<string>();
        tree.Insert("/a", "/a");
        tree.Insert("/b", "/b");
        tree.Insert("/bb", "/bb");
        tree.Insert("/ab", "/ab");
        dfs = tree.BuildDFSTree(i => i);

        dict = new Dictionary<string, string>();
        dict.Add("/a", "/a");
        dict.Add("/b", "/b");
        dict.Add("/bb", "/bb");
        dict.Add("/ab", "/ab");
        frozenDict = dict.ToFrozenDictionary();

        concurrentdict = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();
        concurrentdict.TryAdd("/a", "/a");
        concurrentdict.TryAdd("/b", "/b");
        concurrentdict.TryAdd("/bb", "/bb");
        concurrentdict.TryAdd("/ab", "/ab");

        cache = new ConcurrentCache<string, DFSNode<List<string>>>(10, CacheEvictionPolicy.LRU);
    }

    [Benchmark]
    public void DFSTreeSearchChildren()
    {
        dfs.Search("/bbb");
    }

    [Benchmark]
    public void DFSTreeSearchChildrenWithCache()
    {
        var key = "/bbb";
        if (!cache.TryGetValue(key, out var n))
        {
            n = dfs.Search(key);
            cache.AddOrUpdate(key, n, out var added);
        }
    }

    [Benchmark]
    public void DictGetChildren()
    {
        dict.TryGetValue("/bbb", out var a);
    }

    [Benchmark]
    public void FrozenDictionaryGetChildren()
    {
        frozenDict.TryGetValue("/bbb", out var a);
    }

    [Benchmark]
    public void ConcurrentDictionaryGetChildren()
    {
        concurrentdict.TryGetValue("/bbb", out var a);
    }
}