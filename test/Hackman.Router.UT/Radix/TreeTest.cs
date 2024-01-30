using Hackman.Router.Radix;

namespace Hackman.Router.UT.Radix
{
    public class TreeTest
    {
        #region FindCommonPrexNode

        [Fact]
        public void Empty_FindCommonPrexNodeTest()
        {
            var tree = new Tree<string>();
            var r = tree.FindCommonPrexNode("/a");
            Assert.Equal(0, r.nodePathLength);
            Assert.Equal(-1, r.index);
            Assert.Same(tree.Root, r.node);
        }

        [Fact]
        public void OnePath_FindCommonPrexNodeTest()
        {
            var tree = new Tree<string>();
            var n = new Node<string>() { Path = "/" };
            tree.Root.Children.Add(n);
            var r = tree.FindCommonPrexNode("/a");
            Assert.Equal(1, r.nodePathLength);
            Assert.Equal(0, r.index);
            Assert.Same(n, r.node);
        }

        [Fact]
        public void TwoPath_FindCommonPrexNodeTest()
        {
            var tree = new Tree<string>();
            var n = new Node<string>() { Path = "/", Children = new List<Node<string>>() };
            tree.Root.Children.Add(n);
            n.Children.Add(new Node<string>() { Path = "a" });
            var n2 = new Node<string>() { Path = "b" };
            n.Children.Add(n2);
            var r = tree.FindCommonPrexNode("/bb");
            Assert.Equal(1, r.nodePathLength);
            Assert.Equal(1, r.index);
            Assert.Same(n2, r.node);
        }

        [Fact]
        public void TwoPathPart_FindCommonPrexNodeTest()
        {
            var tree = new Tree<string>();
            var n = new Node<string>() { Path = "/", Children = new List<Node<string>>() };
            tree.Root.Children.Add(n);
            n.Children.Add(new Node<string>() { Path = "a" });
            var n2 = new Node<string>() { Path = "b" };
            n.Children.Add(n2);
            var r = tree.FindCommonPrexNode("/cc");
            Assert.Equal(1, r.nodePathLength);
            Assert.Equal(0, r.index);
            Assert.Same(n, r.node);
        }

        [Fact]
        public void TwoPathFull_FindCommonPrexNodeTest()
        {
            var tree = new Tree<string>();
            var n = new Node<string>() { Path = "/", Children = new List<Node<string>>() };
            tree.Root.Children.Add(n);
            n.Children.Add(new Node<string>() { Path = "a" });
            var n2 = new Node<string>() { Path = "b" };
            n.Children.Add(n2);
            n2 = new Node<string>() { Path = "cccc" };
            n.Children.Add(n2);
            var r = tree.FindCommonPrexNode("/cc");
            Assert.Equal(2, r.nodePathLength);
            Assert.Equal(2, r.index);
            Assert.Same(n2, r.node);
        }

        #endregion FindCommonPrexNode

        #region Insert

        [Fact]
        public void Empty_InsertTest()
        {
            var tree = new Tree<string>();
            tree.Insert("/a", "/a");
            Assert.False(tree.Root.HasValue);
            Assert.Null(tree.Root.Value);
            Assert.Single(tree.Root.Children);
            Assert.Equal("/a", tree.Root.Children.First().Path);
        }

        [Fact]
        public void Two_InsertTest()
        {
            var tree = new Tree<string>();
            tree.Insert("/a", "/a");
            tree.Insert("/b", "/b");
            Assert.False(tree.Root.HasValue);
            Assert.Null(tree.Root.Value);
            Assert.Single(tree.Root.Children);
            Assert.Equal("/", tree.Root.Children.First().Path);
            Assert.Equal(2, tree.Root.Children.First().Children.Count);
            Assert.Equal("a", tree.Root.Children.First().Children.First().Path);
            Assert.Equal("b", tree.Root.Children.First().Children.Last().Path);
        }

        [Fact]
        public void TwoPart_InsertTest()
        {
            var tree = new Tree<string>();
            tree.Insert("/a", "/a");
            tree.Insert("/", "/");
            Assert.False(tree.Root.HasValue);
            Assert.Null(tree.Root.Value);
            Assert.Single(tree.Root.Children);
            Assert.Equal("/", tree.Root.Children.First().Path);
            Assert.True(tree.Root.Children.First().HasValue);
            Assert.Single(tree.Root.Children.First().Value);
            Assert.Equal("/", tree.Root.Children.First().Value.First());
            Assert.Single(tree.Root.Children.First().Children);
            Assert.Equal("a", tree.Root.Children.First().Children.First().Path);
        }

        [Fact]
        public void Three_InsertTest()
        {
            var tree = new Tree<string>();
            tree.Insert("/a", "/a");
            tree.Insert("/b", "/b");
            tree.Insert("/bb", "/bb");
            Assert.False(tree.Root.HasValue);
            Assert.Null(tree.Root.Value);
            Assert.Single(tree.Root.Children);
            Assert.Equal("/", tree.Root.Children.First().Path);
            Assert.Equal(2, tree.Root.Children.First().Children.Count);
            Assert.Equal("a", tree.Root.Children.First().Children.First().Path);
            Assert.Equal("b", tree.Root.Children.First().Children.Last().Path);
            Assert.True(tree.Root.Children.First().Children.Last().HasValue);
            Assert.Single(tree.Root.Children.First().Children.Last().Children);
            Assert.Equal("b", tree.Root.Children.First().Children.Last().Children.First().Path);
        }
        #endregion
    }

    public class DFSTreeTest
    {

        [Fact]
        public void SearchChildrenTest()
        {
            var tree = new Tree<string>();
            tree.Insert("/a", "/a");
            tree.Insert("/b", "/b");
            tree.Insert("/bb", "/bb");
            var d = tree.BuildDFSTree();
            var n = d.Search("/bbb");
            Assert.Single(n.Value);
            Assert.Equal("/bb", n.Value.First());
        }
    }
}