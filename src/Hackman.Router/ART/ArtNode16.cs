using System;

namespace Hackman.Router.ART
{
    internal class ArtNode16 : ArtNode
    {
        public static int count;

        public ArtNode16() : base()
        {
            count++;
        }

        public ArtNode16(ArtNode16 other) : base(other)
        {
            System.Array.Copy(other.keys, 0, keys, 0, other.num_children);
            for (int i = 0; i < other.num_children; i++)
            {
                children[i] = other.children[i];
                children[i].refcount++;
            }

            count++;
        }

        public ArtNode16(ArtNode4 other) : this()
        {
            // ArtNode
            this.num_children = other.num_children;
            this.partial_len = other.partial_len;
            System.Array.Copy(other.partial, 0, this.partial, 0, Math.Min(MAX_PREFIX_LEN, this.partial_len));

            // ArtNode16 from ArtNode4
            System.Array.Copy(other.keys, 0, keys, 0, this.num_children);
            for (int i = 0; i < this.num_children; i++)
            {
                children[i] = other.children[i];
                children[i].refcount++;
            }
        }

        // ArtNode
        // ArtNode16 from ArtNode4
        public ArtNode16(ArtNode48 other) : this()
        {
            // ArtNode
            this.num_children = other.num_children;
            this.partial_len = other.partial_len;
            System.Array.Copy(other.partial, 0, this.partial, 0, Math.Min(MAX_PREFIX_LEN, this.partial_len));

            // ArtNode16 from ArtNode48
            int child = 0;
            for (int i = 0; i < 256; i++)
            {
                int pos = To_uint(other.keys[i]);
                if (pos != 0)
                {
                    keys[child] = (byte)i;
                    children[child] = other.children[pos - 1];
                    children[child].refcount++;
                    child++;
                }
            }
        }

        // ArtNode
        // ArtNode16 from ArtNode4
        // ArtNode
        // ArtNode16 from ArtNode48
        public override Node N_clone()
        {
            return new ArtNode16(this);
        }

        // ArtNode
        // ArtNode16 from ArtNode4
        // ArtNode
        // ArtNode16 from ArtNode48
        public override ChildPtr Find_child(byte c)
        {
            // TODO: avoid linear search using intrinsics if available
            for (int i = 0; i < this.num_children; i++)
            {
                if (keys[i] == c)
                {
                    return new ArrayChildPtr(children, i);
                }
            }

            return null;
        }

        // ArtNode
        // ArtNode16 from ArtNode4
        // ArtNode
        // ArtNode16 from ArtNode48
        // TODO: avoid linear search using intrinsics if available
        public override Leaf Minimum()
        {
            return Node.Minimum(children[0]);
        }

        // ArtNode
        // ArtNode16 from ArtNode4
        // ArtNode
        // ArtNode16 from ArtNode48
        // TODO: avoid linear search using intrinsics if available
        public override void Add_child(ChildPtr @ref, byte c, Node child)
        {
            if (this.num_children < 16)
            {
                // TODO: avoid linear search using intrinsics if available
                int idx;
                for (idx = 0; idx < this.num_children; idx++)
                {
                    if (To_uint(c) < To_uint(keys[idx]))
                        break;
                }

                // Shift to make room
                System.Array.Copy(this.keys, idx, this.keys, idx + 1, this.num_children - idx);
                System.Array.Copy(this.children, idx, this.children, idx + 1, this.num_children - idx);

                // Insert element
                this.keys[idx] = c;
                this.children[idx] = child;
                child.refcount++;
                this.num_children++;
            }
            else
            {
                // Copy the node16 into a new node48
                ArtNode48 result = new ArtNode48(this);

                // Update the parent pointer to the node48
                @ref.Change(result);

                // Insert the element into the node48 instead
                result.Add_child(@ref, c, child);
            }
        }

        // ArtNode
        // ArtNode16 from ArtNode4
        // ArtNode
        // ArtNode16 from ArtNode48
        // TODO: avoid linear search using intrinsics if available
        // TODO: avoid linear search using intrinsics if available
        // Shift to make room
        // Insert element
        // Copy the node16 into a new node48
        // Update the parent pointer to the node48
        // Insert the element into the node48 instead
        public override void Remove_child(ChildPtr @ref, byte c)
        {
            int idx;
            for (idx = 0; idx < this.num_children; idx++)
            {
                if (c == keys[idx])
                    break;
            }

            if (idx == this.num_children)
                return;
            children[idx].Decrement_refcount();

            // Shift to fill the hole
            System.Array.Copy(this.keys, idx + 1, this.keys, idx, this.num_children - idx - 1);
            System.Array.Copy(this.children, idx + 1, this.children, idx, this.num_children - idx - 1);
            this.num_children--;
            if (num_children == 3)
            {
                ArtNode4 result = new ArtNode4(this);
                @ref.Change(result);
            }
        }

        // ArtNode
        // ArtNode16 from ArtNode4
        // ArtNode
        // ArtNode16 from ArtNode48
        // TODO: avoid linear search using intrinsics if available
        // TODO: avoid linear search using intrinsics if available
        // Shift to make room
        // Insert element
        // Copy the node16 into a new node48
        // Update the parent pointer to the node48
        // Insert the element into the node48 instead
        // Shift to fill the hole
        public override bool Exhausted(int i)
        {
            return i >= num_children;
        }

        // ArtNode
        // ArtNode16 from ArtNode4
        // ArtNode
        // ArtNode16 from ArtNode48
        // TODO: avoid linear search using intrinsics if available
        // TODO: avoid linear search using intrinsics if available
        // Shift to make room
        // Insert element
        // Copy the node16 into a new node48
        // Update the parent pointer to the node48
        // Insert the element into the node48 instead
        // Shift to fill the hole
        public override int NextChildAtOrAfter(int i)
        {
            return i;
        }

        // ArtNode
        // ArtNode16 from ArtNode4
        // ArtNode
        // ArtNode16 from ArtNode48
        // TODO: avoid linear search using intrinsics if available
        // TODO: avoid linear search using intrinsics if available
        // Shift to make room
        // Insert element
        // Copy the node16 into a new node48
        // Update the parent pointer to the node48
        // Insert the element into the node48 instead
        // Shift to fill the hole
        public override Node ChildAt(int i)
        {
            return children[i];
        }

        // ArtNode
        // ArtNode16 from ArtNode4
        // ArtNode
        // ArtNode16 from ArtNode48
        // TODO: avoid linear search using intrinsics if available
        // TODO: avoid linear search using intrinsics if available
        // Shift to make room
        // Insert element
        // Copy the node16 into a new node48
        // Update the parent pointer to the node48
        // Insert the element into the node48 instead
        // Shift to fill the hole
        public override int Decrement_refcount()
        {
            if (--this.refcount <= 0)
            {
                int freed = 0;
                for (int i = 0; i < this.num_children; i++)
                {
                    freed += children[i].Decrement_refcount();
                }

                count--;

                // delete this;
                return freed + 232; // object size (8) + refcount (4) +
                // num_children int (4) + partial_len int (4) +
                // pointer to partial array (8) + partial array size (8+4+1*MAX_PREFIX_LEN)
                // pointer to key array (8) + key array size (8+4+1*16) +
                // pointer to children array (8) + children array size (8+4+8*16)
            }

            return 0;
        }

        // ArtNode
        // ArtNode16 from ArtNode4
        // ArtNode
        // ArtNode16 from ArtNode48
        // TODO: avoid linear search using intrinsics if available
        // TODO: avoid linear search using intrinsics if available
        // Shift to make room
        // Insert element
        // Copy the node16 into a new node48
        // Update the parent pointer to the node48
        // Insert the element into the node48 instead
        // Shift to fill the hole
        // delete this;
        // object size (8) + refcount (4) +
        // num_children int (4) + partial_len int (4) +
        // pointer to partial array (8) + partial array size (8+4+1*MAX_PREFIX_LEN)
        // pointer to key array (8) + key array size (8+4+1*16) +
        // pointer to children array (8) + children array size (8+4+8*16)
        internal byte[] keys = new byte[16];

        // ArtNode
        // ArtNode16 from ArtNode4
        // ArtNode
        // ArtNode16 from ArtNode48
        // TODO: avoid linear search using intrinsics if available
        // TODO: avoid linear search using intrinsics if available
        // Shift to make room
        // Insert element
        // Copy the node16 into a new node48
        // Update the parent pointer to the node48
        // Insert the element into the node48 instead
        // Shift to fill the hole
        // delete this;
        // object size (8) + refcount (4) +
        // num_children int (4) + partial_len int (4) +
        // pointer to partial array (8) + partial array size (8+4+1*MAX_PREFIX_LEN)
        // pointer to key array (8) + key array size (8+4+1*16) +
        // pointer to children array (8) + children array size (8+4+8*16)
        internal Node[] children = new Node[16];
    }
}