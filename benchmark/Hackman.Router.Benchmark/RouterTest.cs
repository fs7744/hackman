﻿using BenchmarkDotNet.Attributes;
using Hackman.Router.Radix;
using System.Collections.Frozen;

namespace Hackman.Router.Benchmark
{
    public class TestPriorityPathRouterMetadata : IPriorityRouterMetadata
    {
        private readonly Func<MatchRouter, TestPriorityPathRouterMetadata, MatchRouter> buildMatchRouter;

        public TestPriorityPathRouterMetadata(int order, string path, PathMatch pathMatch, Func<MatchRouter, TestPriorityPathRouterMetadata, MatchRouter> buildMatchRouter)
        {
            this.Order = order;
            this.Path = path;
            this.PathMatch = pathMatch;
            this.buildMatchRouter = buildMatchRouter;
        }

        public int Order { get; set; }

        public string Path { get; set; }

        public PathMatch PathMatch { get; set; }

        public MatchRouter BuildMatchRouter(MatchRouter next)
        {
            return buildMatchRouter(next, this);
        }
    }

    public class TestRoutingContext : IRoutingContext
    {
        public string Path { get; set; }
    }

    public class NoCachePriorityPathRouter
    {
        private readonly FrozenDictionary<string, MatchRouter> e;
        private readonly DFSTree<MatchRouter> t;

        public NoCachePriorityPathRouter(FrozenDictionary<string, MatchRouter> e, DFSTree<MatchRouter> t)
        {
            this.e = e;
            this.t = t;
        }

        public IPriorityRouterMetadata Match(IRoutingContext context)
        {
            if (e.TryGetValue(context.Path, out var router))
            {
                return router(context);
            }

            var n = t.Search(context.Path);
            while (n != null)
            {
                var r = n.Value?.Invoke(context);
                if (r != null)
                    return r;
                n = n.ValueParent;
            }
            return null;
        }
    }

    [AllStatisticsColumn]
    public class RouterTest
    {
        PriorityPathRouter priorityPathRouter;
        NoCachePriorityPathRouter noCachePriorityPathRouter;
        public RouterTest()
        {
            var b = new PriorityPathRouterBuilder();

            b.Add(new TestPriorityPathRouterMetadata(0, "/a", PathMatch.Prefix, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(0, "/b", PathMatch.Prefix, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(0, "/bb", PathMatch.Prefix, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(0, "/ab", PathMatch.Prefix, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(0, "/a/dsd/csd", PathMatch.Exact, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(1, "/a/dsd/csd", PathMatch.Exact, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(0, "/aaa/bbb/", PathMatch.Prefix, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(0, "/aaa/cccc/", PathMatch.Prefix, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(0, "/aaa/bbddb/", PathMatch.Prefix, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(0, "/aaa/ccfffcc/", PathMatch.Prefix, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(0, "/aaa/bbvvvb/", PathMatch.Prefix, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(0, "/aaa/ccddcc/", PathMatch.Prefix, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(0, "/aaa/fsd/", PathMatch.Prefix, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(0, "/aaa/cfffccc/", PathMatch.Prefix, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(0, "/aaa/ssdds/", PathMatch.Prefix, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(0, "/aaa/ccffcc/", PathMatch.Prefix, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(0, "/aaa/bbggb/", PathMatch.Prefix, (next, m) => (c) => m));
            b.Add(new TestPriorityPathRouterMetadata(0, "/aaa/ccddcc/", PathMatch.Prefix, (next, m) => (c) => m));
            priorityPathRouter = b.Build();
            var (e, t) = b.CreateData();
            noCachePriorityPathRouter = new NoCachePriorityPathRouter(e, t);
        }

        [Benchmark]
        public void PriorityPathRouterGet()
        {
            priorityPathRouter.Match(new TestRoutingContext() { Path = "/aaa/ccddcc/ddsds" });
        }

        [Benchmark]
        public void NoCachePriorityPathRouterGet()
        {
            noCachePriorityPathRouter.Match(new TestRoutingContext() { Path = "/aaa/ccddcc/ddsds" });
        }
    }
}
