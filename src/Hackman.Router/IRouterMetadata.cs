using DotNext.Runtime.Caching;
using Hackman.Router.Radix;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace Hackman.Router
{
    public interface IRoutingContext
    {
        public string Path { get; }
    }

    public interface IPriorityRouterMetadata
    {
        public int Order { get; }

        public string Path { get; }

        public PathMatch PathMatch { get; }

        public MatchRouter BuildMatchRouter(MatchRouter next);
    }

    public enum PathMatch
    {
        Exact,
        Prefix
    }

    public delegate IPriorityRouterMetadata MatchRouter(IRoutingContext context);

    public class PriorityPathRouterBuilder
    {
        private readonly StringComparison comparison;
        private Tree<IPriorityRouterMetadata> tree;

        public int CacheCapacity { get; set; } = 10000;
        public CacheEvictionPolicy CacheEvictionPolicy { get; set; } = CacheEvictionPolicy.LRU;

        private Dictionary<string, List<IPriorityRouterMetadata>> exact;

        public PriorityPathRouterBuilder(StringComparison comparison = StringComparison.Ordinal)
        {
            tree = new Tree<IPriorityRouterMetadata>() { Comparison = comparison };
            exact = new Dictionary<string, List<IPriorityRouterMetadata>>(comparison == StringComparison.OrdinalIgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
            this.comparison = comparison;
        }

        public void Add(IPriorityRouterMetadata metadata)
        {
            switch (metadata.PathMatch)
            {
                case PathMatch.Exact:
                    if (exact.ContainsKey(metadata.Path))
                    {
                        exact[metadata.Path].Add(metadata);
                    }
                    else
                    {
                        exact.Add(metadata.Path, new List<IPriorityRouterMetadata>() { metadata });
                    }
                    break;

                case PathMatch.Prefix:
                    tree.Insert(metadata.Path, metadata);
                    break;
            }
        }

        public PriorityPathRouter Build()
        {
            var e = exact.ToFrozenDictionary(i => i.Key, i =>
            {
                MatchRouter router = (c) => null;
                foreach (var item in i.Value.OrderByDescending(j => j.Order))
                {
                    var x = item.BuildMatchRouter(router);
                    if (x != null)
                    {
                        router = x;
                    }
                }
                return router;
            }, comparison == StringComparison.OrdinalIgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
            var t = tree.BuildDFSTree<MatchRouter>(i =>
            {
                MatchRouter router = (c) => null;
                foreach (var item in i.OrderByDescending(j => j.Order))
                {
                    var x = item.BuildMatchRouter(router);
                    if (x != null)
                    {
                        router = x;
                    }
                }
                return router;
            });
            return new PriorityPathRouter(e, t, comparison, CacheCapacity, CacheEvictionPolicy);
        }
    }

    public class PriorityPathRouter
    {
        private readonly FrozenDictionary<string, MatchRouter> e;
        private readonly DFSTree<MatchRouter> t;
        private readonly ConcurrentCache<string, DFSNode<MatchRouter>> cache;

        public PriorityPathRouter(FrozenDictionary<string, MatchRouter> e, DFSTree<MatchRouter> t, StringComparison comparison, int cacheCapacity, CacheEvictionPolicy cacheEvictionPolicy)
        {
            this.e = e;
            this.t = t;
            this.cache = new ConcurrentCache<string, DFSNode<MatchRouter>>(cacheCapacity, cacheEvictionPolicy, comparison == StringComparison.OrdinalIgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
        }

        public IPriorityRouterMetadata Match(IRoutingContext context)
        {
            if (e.TryGetValue(context.Path, out var router))
            {
                return router(context);
            }

            if (!cache.TryGetValue(context.Path, out var n))
            {
                n = t.Search(context.Path);
                cache.AddOrUpdate(context.Path, n, out var added);
            }
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
}