namespace Hackman.Router.UT
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

    public class PriorityPathRouterBuilderTest
    {
        [Fact]
        public void BuildTest()
        {
            var b = new PriorityPathRouterBuilder();

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
            var priorityPathRouter = b.Build();
        }
    }
}
