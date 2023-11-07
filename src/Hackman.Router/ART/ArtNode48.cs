using System;

namespace Hackman.Router.ART
{
    internal class ArtNode48 : ArtNode
    {
        public static int count;

        public ArtNode48() : base()
        {
            count++;
        }

        public ArtNode48(ArtNode48 other) : base(other)
        {
            System.Array.Copy(other.keys, 0, keys, 0, 256);

            // Copy the children. We have to look at all elements of `children`
            // rather than just the first num_children elements because `children`
            // may not be contiguous due to deletion
            for (int i = 0; i < 48; i++)
            {
                children[i] = other.children[i];
                if (children[i] != null)
                {
                    children[i].refcount++;
                }
            }

            count++;
        }

        // Copy the children. We have to look at all elements of `children`
        // rather than just the first num_children elements because `children`
        // may not be contiguous due to deletion
        public ArtNode48(ArtNode16 other) : this()
        {
            // ArtNode
            this.num_children = other.num_children;
            this.partial_len = other.partial_len;
            System.Array.Copy(other.partial, 0, this.partial, 0, Math.Min(MAX_PREFIX_LEN, this.partial_len));

            // ArtNode48 from ArtNode16
            for (int i = 0; i < this.num_children; i++)
            {
                keys[To_uint(other.keys[i])] = (byte)(i + 1);
                children[i] = other.children[i];
                children[i].refcount++;
            }
        }

        // Copy the children. We have to look at all elements of `children`
        // rather than just the first num_children elements because `children`
        // may not be contiguous due to deletion
        // ArtNode
        // ArtNode48 from ArtNode16
        public ArtNode48(ArtNode256 other) : this()
        {
            // ArtNode
            this.num_children = other.num_children;
            this.partial_len = other.partial_len;
            System.Array.Copy(other.partial, 0, this.partial, 0, Math.Min(MAX_PREFIX_LEN, this.partial_len));

            // ArtNode48 from ArtNode256
            int pos = 0;
            for (int i = 0; i < 256; i++)
            {
                if (other.children[i] != null)
                {
                    keys[i] = (byte)(pos + 1);
                    children[pos] = other.children[i];
                    children[pos].refcount++;
                    pos++;
                }
            }
        }

        // Copy the children. We have to look at all elements of `children`
        // rather than just the first num_children elements because `children`
        // may not be contiguous due to deletion
        // ArtNode
        // ArtNode48 from ArtNode16
        // ArtNode
        // ArtNode48 from ArtNode256
        public override Node N_clone()
        {
            return new ArtNode48(this);
        }

        // Copy the children. We have to look at all elements of `children`
        // rather than just the first num_children elements because `children`
        // may not be contiguous due to deletion
        // ArtNode
        // ArtNode48 from ArtNode16
        // ArtNode
        // ArtNode48 from ArtNode256
        public override ChildPtr Find_child(byte c)
        {
            int idx = To_uint(keys[To_uint(c)]);
            if (idx != 0)
                return new ArrayChildPtr(children, idx - 1);
            return null;
        }

        // Copy the children. We have to look at all elements of `children`
        // rather than just the first num_children elements because `children`
        // may not be contiguous due to deletion
        // ArtNode
        // ArtNode48 from ArtNode16
        // ArtNode
        // ArtNode48 from ArtNode256
        public override Leaf Minimum()
        {
            int idx = 0;
            while (keys[idx] == 0)
                idx++;
            Node child = children[To_uint(keys[idx]) - 1];
            return Node.Minimum(child);
        }

        // Copy the children. We have to look at all elements of `children`
        // rather than just the first num_children elements because `children`
        // may not be contiguous due to deletion
        // ArtNode
        // ArtNode48 from ArtNode16
        // ArtNode
        // ArtNode48 from ArtNode256
        public override void Add_child(ChildPtr @ref, byte c, Node child)
        {
            if (this.num_children < 48)
            {
                // Have to do a linear scan because deletion may create holes in
                // children array
                int pos = 0;
                while (children[pos] != null)
                    pos++;
                this.children[pos] = child;
                child.refcount++;
                this.keys[To_uint(c)] = (byte)(pos + 1);
                this.num_children++;
            }
            else
            {
                // Copy the node48 into a new node256
                ArtNode256 result = new ArtNode256(this);

                // Update the parent pointer to the node256
                @ref.Change(result);

                // Insert the element into the node256 instead
                result.Add_child(@ref, c, child);
            }
        }

        // Copy the children. We have to look at all elements of `children`
        // rather than just the first num_children elements because `children`
        // may not be contiguous due to deletion
        // ArtNode
        // ArtNode48 from ArtNode16
        // ArtNode
        // ArtNode48 from ArtNode256
        // Have to do a linear scan because deletion may create holes in
        // children array
        // Copy the node48 into a new node256
        // Update the parent pointer to the node256
        // Insert the element into the node256 instead
        public override void Remove_child(ChildPtr @ref, byte c)
        {
            // Delete the child, leaving a hole in children. We can't shift children
            // because that would require decrementing many elements of keys
            int pos = To_uint(keys[To_uint(c)]);
            keys[To_uint(c)] = 0;
            children[pos - 1].Decrement_refcount();
            children[pos - 1] = null;
            num_children--;
            if (num_children == 12)
            {
                ArtNode16 result = new ArtNode16(this);
                @ref.Change(result);
            }
        }

        // Copy the children. We have to look at all elements of `children`
        // rather than just the first num_children elements because `children`
        // may not be contiguous due to deletion
        // ArtNode
        // ArtNode48 from ArtNode16
        // ArtNode
        // ArtNode48 from ArtNode256
        // Have to do a linear scan because deletion may create holes in
        // children array
        // Copy the node48 into a new node256
        // Update the parent pointer to the node256
        // Insert the element into the node256 instead
        // Delete the child, leaving a hole in children. We can't shift children
        // because that would require decrementing many elements of keys
        public override bool Exhausted(int c)
        {
            for (int i = c; i < 256; i++)
            {
                if (keys[i] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        // Copy the children. We have to look at all elements of `children`
        // rather than just the first num_children elements because `children`
        // may not be contiguous due to deletion
        // ArtNode
        // ArtNode48 from ArtNode16
        // ArtNode
        // ArtNode48 from ArtNode256
        // Have to do a linear scan because deletion may create holes in
        // children array
        // Copy the node48 into a new node256
        // Update the parent pointer to the node256
        // Insert the element into the node256 instead
        // Delete the child, leaving a hole in children. We can't shift children
        // because that would require decrementing many elements of keys
        public override int NextChildAtOrAfter(int c)
        {
            int pos = c;
            for (; pos < 256; pos++)
            {
                if (keys[pos] != 0)
                {
                    break;
                }
            }

            return pos;
        }

        // Copy the children. We have to look at all elements of `children`
        // rather than just the first num_children elements because `children`
        // may not be contiguous due to deletion
        // ArtNode
        // ArtNode48 from ArtNode16
        // ArtNode
        // ArtNode48 from ArtNode256
        // Have to do a linear scan because deletion may create holes in
        // children array
        // Copy the node48 into a new node256
        // Update the parent pointer to the node256
        // Insert the element into the node256 instead
        // Delete the child, leaving a hole in children. We can't shift children
        // because that would require decrementing many elements of keys
        public override Node ChildAt(int c)
        {
            return children[To_uint(keys[c]) - 1];
        }

        // Copy the children. We have to look at all elements of `children`
        // rather than just the first num_children elements because `children`
        // may not be contiguous due to deletion
        // ArtNode
        // ArtNode48 from ArtNode16
        // ArtNode
        // ArtNode48 from ArtNode256
        // Have to do a linear scan because deletion may create holes in
        // children array
        // Copy the node48 into a new node256
        // Update the parent pointer to the node256
        // Insert the element into the node256 instead
        // Delete the child, leaving a hole in children. We can't shift children
        // because that would require decrementing many elements of keys
        public override int Decrement_refcount()
        {
            if (--this.refcount <= 0)
            {
                int freed = 0;
                for (int i = 0; i < this.num_children; i++)
                {
                    if (children[i] != null)
                    {
                        freed += children[i].Decrement_refcount();
                    }
                }

                count--;

                // delete this;
                return freed + 728; // object size (8) + refcount (4) +
                // num_children int (4) + partial_len int (4) +
                // pointer to partial array (8) + partial array size (8+4+1*MAX_PREFIX_LEN)
                // pointer to key array (8) + key array size (8+4+1*256) +
                // pointer to children array (8) + children array size (8+4+8*48)
            }

            return 0;
        }

        // Copy the children. We have to look at all elements of `children`
        // rather than just the first num_children elements because `children`
        // may not be contiguous due to deletion
        // ArtNode
        // ArtNode48 from ArtNode16
        // ArtNode
        // ArtNode48 from ArtNode256
        // Have to do a linear scan because deletion may create holes in
        // children array
        // Copy the node48 into a new node256
        // Update the parent pointer to the node256
        // Insert the element into the node256 instead
        // Delete the child, leaving a hole in children. We can't shift children
        // because that would require decrementing many elements of keys
        // delete this;
        // object size (8) + refcount (4) +
        // num_children int (4) + partial_len int (4) +
        // pointer to partial array (8) + partial array size (8+4+1*MAX_PREFIX_LEN)
        // pointer to key array (8) + key array size (8+4+1*256) +
        // pointer to children array (8) + children array size (8+4+8*48)
        internal byte[] keys = new byte[256];

        // Copy the children. We have to look at all elements of `children`
        // rather than just the first num_children elements because `children`
        // may not be contiguous due to deletion
        // ArtNode
        // ArtNode48 from ArtNode16
        // ArtNode
        // ArtNode48 from ArtNode256
        // Have to do a linear scan because deletion may create holes in
        // children array
        // Copy the node48 into a new node256
        // Update the parent pointer to the node256
        // Insert the element into the node256 instead
        // Delete the child, leaving a hole in children. We can't shift children
        // because that would require decrementing many elements of keys
        // delete this;
        // object size (8) + refcount (4) +
        // num_children int (4) + partial_len int (4) +
        // pointer to partial array (8) + partial array size (8+4+1*MAX_PREFIX_LEN)
        // pointer to key array (8) + key array size (8+4+1*256) +
        // pointer to children array (8) + children array size (8+4+8*48)
        internal Node[] children = new Node[48];
    }
}