namespace Hackman.Router.ART
{
    public abstract class Node
    {
        public static readonly int MAX_PREFIX_LEN = 8;

        public Node()
        {
            refcount = 0;
        }

        public Node(Node other)
        {
            refcount = 0;
        }

        public abstract Node N_clone();

        public static Node N_clone(Node n)
        {
            if (n == null)
                return null;
            else
                return n.N_clone();
        }

        public abstract Leaf Minimum();

        public static Leaf Minimum(Node n)
        {
            if (n == null)
                return null;
            else
                return n.Minimum();
        }

        public abstract bool Insert(ChildPtr @ref, byte[] key, object value, int depth, bool force_clone);

        public static bool Insert(Node n, ChildPtr @ref, byte[] key, object value, int depth, bool force_clone)
        {
            // If we are at a NULL node, inject a leaf
            if (n == null)
            {
                @ref.Change(new Leaf(key, value));
                return true;
            }
            else
            {
                return n.Insert(@ref, key, value, depth, force_clone);
            }
        }

        // If we are at a NULL node, inject a leaf
        public abstract bool Delete(ChildPtr @ref, byte[] key, int depth, bool force_clone);

        // If we are at a NULL node, inject a leaf
        public abstract int Decrement_refcount();

        // If we are at a NULL node, inject a leaf
        public abstract bool Exhausted(int i);

        // If we are at a NULL node, inject a leaf
        public static bool Exhausted(Node n, int i)
        {
            if (n == null)
                return true;
            else
                return n.Exhausted(i);
        }

        // If we are at a NULL node, inject a leaf
        public static int To_uint(byte b)
        {
            return ((int)b) & 0xFF;
        }

        // If we are at a NULL node, inject a leaf
        internal int refcount;
    }
}