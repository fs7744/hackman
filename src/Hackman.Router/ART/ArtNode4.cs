using System;

namespace Hackman.Router.ART
{
    internal class ArtNode4 : ArtNode
    {
        public static int count;

        public ArtNode4() : base()
        {
            count++;
        }

        public ArtNode4(ArtNode4 other) : base(other)
        {
            System.Array.Copy(other.keys, 0, keys, 0, other.num_children);
            for (int i = 0; i < other.num_children; i++)
            {
                children[i] = other.children[i];
                children[i].refcount++;
            }

            count++;
        }

        public ArtNode4(ArtNode16 other) : this()
        {
            // ArtNode
            this.num_children = other.num_children;
            this.partial_len = other.partial_len;
            System.Array.Copy(other.partial, 0, this.partial, 0, Math.Min(MAX_PREFIX_LEN, this.partial_len));

            // ArtNode4 from ArtNode16
            System.Array.Copy(other.keys, 0, keys, 0, this.num_children);
            for (int i = 0; i < this.num_children; i++)
            {
                children[i] = other.children[i];
                children[i].refcount++;
            }
        }

        // ArtNode
        // ArtNode4 from ArtNode16
        public override Node N_clone()
        {
            return new ArtNode4(this);
        }

        // ArtNode
        // ArtNode4 from ArtNode16
        public override ChildPtr Find_child(byte c)
        {
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
        // ArtNode4 from ArtNode16
        public override Leaf Minimum()
        {
            return Node.Minimum(children[0]);
        }

        // ArtNode
        // ArtNode4 from ArtNode16
        public override void Add_child(ChildPtr @ref, byte c, Node child)
        {
            if (this.num_children < 4)
            {
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
                // Copy the node4 into a new node16
                ArtNode16 result = new ArtNode16(this);

                // Update the parent pointer to the node16
                @ref.Change(result);

                // Insert the element into the node16 instead
                result.Add_child(@ref, c, child);
            }
        }

        // ArtNode
        // ArtNode4 from ArtNode16
        // Shift to make room
        // Insert element
        // Copy the node4 into a new node16
        // Update the parent pointer to the node16
        // Insert the element into the node16 instead
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

            // Remove nodes with only a single child
            if (num_children == 1)
            {
                Node child = children[0];
                if (!(child is Leaf))
                {
                    if (((ArtNode)child).refcount > 1)
                    {
                        child = child.N_clone();
                    }

                    ArtNode an_child = (ArtNode)child;

                    // Concatenate the prefixes
                    int prefix = partial_len;
                    if (prefix < MAX_PREFIX_LEN)
                    {
                        partial[prefix] = keys[0];
                        prefix++;
                    }

                    if (prefix < MAX_PREFIX_LEN)
                    {
                        int sub_prefix = Math.Min(an_child.partial_len, MAX_PREFIX_LEN - prefix);
                        System.Array.Copy(an_child.partial, 0, partial, prefix, sub_prefix);
                        prefix += sub_prefix;
                    }

                    // Store the prefix in the child
                    System.Array.Copy(partial, 0, an_child.partial, 0, Math.Min(prefix, MAX_PREFIX_LEN));
                    an_child.partial_len += partial_len + 1;
                }

                @ref.Change(child);
            }
        }

        // ArtNode
        // ArtNode4 from ArtNode16
        // Shift to make room
        // Insert element
        // Copy the node4 into a new node16
        // Update the parent pointer to the node16
        // Insert the element into the node16 instead
        // Shift to fill the hole
        // Remove nodes with only a single child
        // Concatenate the prefixes
        // Store the prefix in the child
        public override bool Exhausted(int i)
        {
            return i >= num_children;
        }

        // ArtNode
        // ArtNode4 from ArtNode16
        // Shift to make room
        // Insert element
        // Copy the node4 into a new node16
        // Update the parent pointer to the node16
        // Insert the element into the node16 instead
        // Shift to fill the hole
        // Remove nodes with only a single child
        // Concatenate the prefixes
        // Store the prefix in the child
        public override int NextChildAtOrAfter(int i)
        {
            return i;
        }

        // ArtNode
        // ArtNode4 from ArtNode16
        // Shift to make room
        // Insert element
        // Copy the node4 into a new node16
        // Update the parent pointer to the node16
        // Insert the element into the node16 instead
        // Shift to fill the hole
        // Remove nodes with only a single child
        // Concatenate the prefixes
        // Store the prefix in the child
        public override Node ChildAt(int i)
        {
            return children[i];
        }

        // ArtNode
        // ArtNode4 from ArtNode16
        // Shift to make room
        // Insert element
        // Copy the node4 into a new node16
        // Update the parent pointer to the node16
        // Insert the element into the node16 instead
        // Shift to fill the hole
        // Remove nodes with only a single child
        // Concatenate the prefixes
        // Store the prefix in the child
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
                return freed + 128; // object size (8) + refcount (4) +
                // num_children int (4) + partial_len int (4) +
                // pointer to partial array (8) + partial array size (8+4+1*MAX_PREFIX_LEN)
                // pointer to key array (8) + key array size (8+4+1*4) +
                // pointer to children array (8) + children array size (8+4+8*4) +
                // padding (4)
            }

            return 0;
        }

        // ArtNode
        // ArtNode4 from ArtNode16
        // Shift to make room
        // Insert element
        // Copy the node4 into a new node16
        // Update the parent pointer to the node16
        // Insert the element into the node16 instead
        // Shift to fill the hole
        // Remove nodes with only a single child
        // Concatenate the prefixes
        // Store the prefix in the child
        // delete this;
        // object size (8) + refcount (4) +
        // num_children int (4) + partial_len int (4) +
        // pointer to partial array (8) + partial array size (8+4+1*MAX_PREFIX_LEN)
        // pointer to key array (8) + key array size (8+4+1*4) +
        // pointer to children array (8) + children array size (8+4+8*4) +
        // padding (4)
        internal byte[] keys = new byte[4];

        // ArtNode
        // ArtNode4 from ArtNode16
        // Shift to make room
        // Insert element
        // Copy the node4 into a new node16
        // Update the parent pointer to the node16
        // Insert the element into the node16 instead
        // Shift to fill the hole
        // Remove nodes with only a single child
        // Concatenate the prefixes
        // Store the prefix in the child
        // delete this;
        // object size (8) + refcount (4) +
        // num_children int (4) + partial_len int (4) +
        // pointer to partial array (8) + partial array size (8+4+1*MAX_PREFIX_LEN)
        // pointer to key array (8) + key array size (8+4+1*4) +
        // pointer to children array (8) + children array size (8+4+8*4) +
        // padding (4)
        internal Node[] children = new Node[4];
    }
}