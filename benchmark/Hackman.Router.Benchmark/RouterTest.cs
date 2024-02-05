using BenchmarkDotNet.Attributes;

namespace Hackman.Router.Benchmark
{
    public class TestPriorityPathRouterMetadata : IPriorityRouterMetadata
    {
        public int Order => throw new NotImplementedException();

        public string Path => throw new NotImplementedException();

        public PathMatch PathMatch => throw new NotImplementedException();

        public MatchRouter BuildMatchRouter(MatchRouter next)
        {
            throw new NotImplementedException();
        }
    }

    [AllStatisticsColumn]
    public class RouterTest
    {
        PriorityPathRouter priorityPathRouter;
        public RouterTest()
        {
            var b = new PriorityPathRouterBuilder();
        }
    }
}
