using Hackman.Router.Radix;

namespace Hackman.Router.UT.Radix
{
    public class TreeTest
    {
        [Fact]
        public void Empty_FindCommonPrexNodeTest()
        {
            var tree = new Tree<string>();
            var r = tree.FindCommonPrexNode("/a");
            Assert.Equal(0, r.nodePathLength);
            Assert.Equal(0, r.index);
            Assert.Same(tree.Root, r.node);
        }
    }
}
