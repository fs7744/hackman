namespace Hackman.Router.ART
{
    internal class ArrayChildPtr : ChildPtr
    {
        public ArrayChildPtr(Node[] children, int i)
        {
            this.children = children;
            this.i = i;
        }

        public override Node Get()
        {
            return children[i];
        }

        public override void Set(Node n)
        {
            children[i] = n;
        }

        private Node[] children;
        private readonly int i;
    }
}