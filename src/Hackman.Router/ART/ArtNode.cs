using System;

namespace Hackman.Router.ART
{
    internal abstract class ArtNode : Node
    {
        public ArtNode() : base()
        {
        }

        public ArtNode(ArtNode other) : base(other)
        {
            this.num_children = other.num_children;
            this.partial_len = other.partial_len;
            System.Array.Copy(other.partial, 0, partial, 0, Math.Min(Node.MAX_PREFIX_LEN, partial_len));
        }

        public virtual int Check_prefix(byte[] key, int depth)
        {
            int max_cmp = Math.Min(Math.Min(partial_len, Node.MAX_PREFIX_LEN), key.Length - depth);
            int idx;
            for (idx = 0; idx < max_cmp; idx++)
            {
                if (partial[idx] != key[depth + idx])
                    return idx;
            }

            return idx;
        }

        /// <summary>
        /// Calculates the index at which the prefixes mismatch
        /// </summary>
        public virtual int Prefix_mismatch(byte[] key, int depth)
        {
            int max_cmp = Math.Min(Math.Min(Node.MAX_PREFIX_LEN, partial_len), key.Length - depth);
            int idx;
            for (idx = 0; idx < max_cmp; idx++)
            {
                if (partial[idx] != key[depth + idx])
                    return idx;
            }

            // If the prefix is short we can avoid finding a leaf
            if (partial_len > Node.MAX_PREFIX_LEN)
            {
                // Prefix is longer than what we've checked, find a leaf
                Leaf l = this.Minimum();
                max_cmp = Math.Min(l.key.Length, key.Length) - depth;
                for (; idx < max_cmp; idx++)
                {
                    if (l.key[idx + depth] != key[depth + idx])
                        return idx;
                }
            }

            return idx;
        }

        /// <summary>
        /// Calculates the index at which the prefixes mismatch
        /// </summary>
        // If the prefix is short we can avoid finding a leaf
        // Prefix is longer than what we've checked, find a leaf
        public abstract ChildPtr Find_child(byte c);

        /// <summary>
        /// Calculates the index at which the prefixes mismatch
        /// </summary>
        // If the prefix is short we can avoid finding a leaf
        // Prefix is longer than what we've checked, find a leaf
        public abstract void Add_child(ChildPtr @ref, byte c, Node child);

        /// <summary>
        /// Calculates the index at which the prefixes mismatch
        /// </summary>
        // If the prefix is short we can avoid finding a leaf
        // Prefix is longer than what we've checked, find a leaf
        public abstract void Remove_child(ChildPtr @ref, byte c);

        /// <summary>
        /// Calculates the index at which the prefixes mismatch
        /// </summary>
        // If the prefix is short we can avoid finding a leaf
        // Prefix is longer than what we've checked, find a leaf
        // Precondition: isLastChild(i) == false
        public abstract int NextChildAtOrAfter(int i);

        /// <summary>
        /// Calculates the index at which the prefixes mismatch
        /// </summary>
        // If the prefix is short we can avoid finding a leaf
        // Prefix is longer than what we've checked, find a leaf
        // Precondition: isLastChild(i) == false
        public abstract Node ChildAt(int i);

        /// <summary>
        /// Calculates the index at which the prefixes mismatch
        /// </summary>
        // If the prefix is short we can avoid finding a leaf
        // Prefix is longer than what we've checked, find a leaf
        // Precondition: isLastChild(i) == false
        public override bool Insert(ChildPtr @ref, byte[] key, object value, int depth, bool force_clone)
        {
            bool do_clone = force_clone || this.refcount > 1;

            // Check if given node has a prefix
            if (partial_len > 0)
            {
                // Determine if the prefixes differ, since we need to split
                int prefix_diff = Prefix_mismatch(key, depth);
                if (prefix_diff >= partial_len)
                {
                    depth += partial_len;
                }
                else
                {
                    // Create a new node
                    ArtNode4 result = new ArtNode4();
                    Node ref_old = @ref.Get();
                    @ref.Change_no_decrement(result); // don't decrement yet, because doing so might destroy self
                    result.partial_len = prefix_diff;
                    Array.Copy(partial, 0, result.partial, 0, Math.Min(Node.MAX_PREFIX_LEN, prefix_diff));

                    // Adjust the prefix of the old node
                    ArtNode this_writable1 = do_clone ? (ArtNode)this.N_clone() : this;
                    if (partial_len <= Node.MAX_PREFIX_LEN)
                    {
                        result.Add_child(@ref, this_writable1.partial[prefix_diff], this_writable1);
                        this_writable1.partial_len -= (prefix_diff + 1);
                        Array.Copy(this_writable1.partial, prefix_diff + 1, this_writable1.partial, 0, Math.Min(Node.MAX_PREFIX_LEN, this_writable1.partial_len));
                    }
                    else
                    {
                        this_writable1.partial_len -= (prefix_diff + 1);
                        Leaf ll = this.Minimum();
                        result.Add_child(@ref, ll.key[depth + prefix_diff], this_writable1);
                        System.Array.Copy(ll.key, depth + prefix_diff + 1, this_writable1.partial, 0, Math.Min(Node.MAX_PREFIX_LEN, this_writable1.partial_len));
                    }

                    // Insert the new leaf
                    Leaf l = new Leaf(key, value);
                    result.Add_child(@ref, key[depth + prefix_diff], l);
                    ref_old.Decrement_refcount();
                    return true;
                }
            }

            // Clone self if necessary
            ArtNode this_writable = do_clone ? (ArtNode)this.N_clone() : this;
            if (do_clone)
            {
                @ref.Change(this_writable);
            }

            // Do the insert, either in a child (if a matching child already exists) or in self
            ChildPtr child = this_writable.Find_child(key[depth]);
            if (child != null)
            {
                return Node.Insert(child.Get(), child, key, value, depth + 1, force_clone);
            }
            else
            {
                // No child, node goes within us
                Leaf l = new Leaf(key, value);
                this_writable.Add_child(@ref, key[depth], l);

                // If `this` was full and `do_clone` is true, we will clone a full node
                // and then immediately delete the clone in favor of a larger node.
                // TODO: avoid this
                return true;
            }
        }

        /// <summary>
        /// Calculates the index at which the prefixes mismatch
        /// </summary>
        // If the prefix is short we can avoid finding a leaf
        // Prefix is longer than what we've checked, find a leaf
        // Precondition: isLastChild(i) == false
        // Check if given node has a prefix
        // Determine if the prefixes differ, since we need to split
        // Create a new node
        // don't decrement yet, because doing so might destroy self
        // Adjust the prefix of the old node
        // Insert the new leaf
        // Clone self if necessary
        // Do the insert, either in a child (if a matching child already exists) or in self
        // No child, node goes within us
        // If `this` was full and `do_clone` is true, we will clone a full node
        // and then immediately delete the clone in favor of a larger node.
        // TODO: avoid this
        public override bool Delete(ChildPtr @ref, byte[] key, int depth, bool force_clone)
        {
            // Bail if the prefix does not match
            if (partial_len > 0)
            {
                int prefix_len = Check_prefix(key, depth);
                if (prefix_len != Math.Min(MAX_PREFIX_LEN, partial_len))
                {
                    return false;
                }

                depth += partial_len;
            }

            bool do_clone = force_clone || this.refcount > 1;

            // Clone self if necessary. Note: this allocation will be wasted if the
            // key does not exist in the child's subtree
            ArtNode this_writable = do_clone ? (ArtNode)this.N_clone() : this;

            // Find child node
            ChildPtr child = this_writable.Find_child(key[depth]);
            if (child == null)
                return false; // when translating to C++, make sure to delete this_writable
            if (do_clone)
            {
                @ref.Change(this_writable);
            }

            bool child_is_leaf = child.Get() is Leaf;
            bool do_delete = child.Get().Delete(child, key, depth + 1, do_clone);
            if (do_delete && child_is_leaf)
            {
                // The leaf to delete is our child, so we must remove it
                this_writable.Remove_child(@ref, key[depth]);
            }

            return do_delete;
        }

        /// <summary>
        /// Calculates the index at which the prefixes mismatch
        /// </summary>
        // If the prefix is short we can avoid finding a leaf
        // Prefix is longer than what we've checked, find a leaf
        // Precondition: isLastChild(i) == false
        // Check if given node has a prefix
        // Determine if the prefixes differ, since we need to split
        // Create a new node
        // don't decrement yet, because doing so might destroy self
        // Adjust the prefix of the old node
        // Insert the new leaf
        // Clone self if necessary
        // Do the insert, either in a child (if a matching child already exists) or in self
        // No child, node goes within us
        // If `this` was full and `do_clone` is true, we will clone a full node
        // and then immediately delete the clone in favor of a larger node.
        // TODO: avoid this
        // Bail if the prefix does not match
        // Clone self if necessary. Note: this allocation will be wasted if the
        // key does not exist in the child's subtree
        // Find child node
        // when translating to C++, make sure to delete this_writable
        // The leaf to delete is our child, so we must remove it
        internal int num_children = 0;

        /// <summary>
        /// Calculates the index at which the prefixes mismatch
        /// </summary>
        // If the prefix is short we can avoid finding a leaf
        // Prefix is longer than what we've checked, find a leaf
        // Precondition: isLastChild(i) == false
        // Check if given node has a prefix
        // Determine if the prefixes differ, since we need to split
        // Create a new node
        // don't decrement yet, because doing so might destroy self
        // Adjust the prefix of the old node
        // Insert the new leaf
        // Clone self if necessary
        // Do the insert, either in a child (if a matching child already exists) or in self
        // No child, node goes within us
        // If `this` was full and `do_clone` is true, we will clone a full node
        // and then immediately delete the clone in favor of a larger node.
        // TODO: avoid this
        // Bail if the prefix does not match
        // Clone self if necessary. Note: this allocation will be wasted if the
        // key does not exist in the child's subtree
        // Find child node
        // when translating to C++, make sure to delete this_writable
        // The leaf to delete is our child, so we must remove it
        internal int partial_len = 0;

        /// <summary>
        /// Calculates the index at which the prefixes mismatch
        /// </summary>
        // If the prefix is short we can avoid finding a leaf
        // Prefix is longer than what we've checked, find a leaf
        // Precondition: isLastChild(i) == false
        // Check if given node has a prefix
        // Determine if the prefixes differ, since we need to split
        // Create a new node
        // don't decrement yet, because doing so might destroy self
        // Adjust the prefix of the old node
        // Insert the new leaf
        // Clone self if necessary
        // Do the insert, either in a child (if a matching child already exists) or in self
        // No child, node goes within us
        // If `this` was full and `do_clone` is true, we will clone a full node
        // and then immediately delete the clone in favor of a larger node.
        // TODO: avoid this
        // Bail if the prefix does not match
        // Clone self if necessary. Note: this allocation will be wasted if the
        // key does not exist in the child's subtree
        // Find child node
        // when translating to C++, make sure to delete this_writable
        // The leaf to delete is our child, so we must remove it
        internal readonly byte[] partial = new byte[Node.MAX_PREFIX_LEN];
    }
}