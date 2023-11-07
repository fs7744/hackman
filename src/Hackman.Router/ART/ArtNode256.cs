using System;

namespace Hackman.Router.ART
{
    internal class ArtNode256 : ArtNode
    {
        public static int count;

        public ArtNode256() : base()
        {
            count++;
        }

        public ArtNode256(ArtNode256 other) : base(other)
        {
            for (int i = 0; i < 256; i++)
            {
                children[i] = other.children[i];
                if (children[i] != null)
                {
                    children[i].refcount++;
                }
            }

            count++;
        }

        public ArtNode256(ArtNode48 other) : this()
        {
            // ArtNode
            this.num_children = other.num_children;
            this.partial_len = other.partial_len;
            System.Array.Copy(other.partial, 0, this.partial, 0, Math.Min(MAX_PREFIX_LEN, this.partial_len));

            // ArtNode256 from ArtNode48
            for (int i = 0; i < 256; i++)
            {
                if (other.keys[i] != 0)
                {
                    children[i] = other.children[To_uint(other.keys[i]) - 1];
                    children[i].refcount++;
                }
            }
        }

        // ArtNode
        // ArtNode256 from ArtNode48
        public override Node N_clone()
        {
            return new ArtNode256(this);
        }

        // ArtNode
        // ArtNode256 from ArtNode48
        public override ChildPtr Find_child(byte c)
        {
            if (children[To_uint(c)] != null)
                return new ArrayChildPtr(children, To_uint(c));
            return null;
        }

        // ArtNode
        // ArtNode256 from ArtNode48
        public override Leaf Minimum()
        {
            int idx = 0;
            while (children[idx] == null)
                idx++;
            return Node.Minimum(children[idx]);
        }

        // ArtNode
        // ArtNode256 from ArtNode48
        public override void Add_child(ChildPtr @ref, byte c, Node child)
        {
            this.num_children++;
            this.children[To_uint(c)] = child;
            child.refcount++;
        }

        // ArtNode
        // ArtNode256 from ArtNode48
        public override void Remove_child(ChildPtr @ref, byte c)
        {
            children[To_uint(c)].Decrement_refcount();
            children[To_uint(c)] = null;
            num_children--;
            if (num_children == 37)
            {
                ArtNode48 result = new ArtNode48(this);
                @ref.Change(result);
            }
        }

        // ArtNode
        // ArtNode256 from ArtNode48
        public override bool Exhausted(int c)
        {
            for (int i = c; i < 256; i++)
            {
                if (children[i] != null)
                {
                    return false;
                }
            }

            return true;
        }

        // ArtNode
        // ArtNode256 from ArtNode48
        public override int NextChildAtOrAfter(int c)
        {
            int pos = c;
            for (; pos < 256; pos++)
            {
                if (children[pos] != null)
                {
                    break;
                }
            }

            return pos;
        }

        // ArtNode
        // ArtNode256 from ArtNode48
        public override Node ChildAt(int pos)
        {
            return children[pos];
        }

        // ArtNode
        // ArtNode256 from ArtNode48
        public override int Decrement_refcount()
        {
            if (--this.refcount <= 0)
            {
                int freed = 0;
                for (int i = 0; i < 256; i++)
                {
                    if (children[i] != null)
                    {
                        freed += children[i].Decrement_refcount();
                    }
                }

                count--;

                // delete this;
                return freed + 2120; // object size (8) + refcount (4) +
                // num_children int (4) + partial_len int (4) +
                // pointer to partial array (8) + partial array size (8+4+1*MAX_PREFIX_LEN)
                // pointer to children array (8) + children array size (8+4+8*256) +
                // padding (4)
            }

            return 0;
        }

        // ArtNode
        // ArtNode256 from ArtNode48
        // delete this;
        // object size (8) + refcount (4) +
        // num_children int (4) + partial_len int (4) +
        // pointer to partial array (8) + partial array size (8+4+1*MAX_PREFIX_LEN)
        // pointer to children array (8) + children array size (8+4+8*256) +
        // padding (4)
        internal Node[] children = new Node[256];
    }
}