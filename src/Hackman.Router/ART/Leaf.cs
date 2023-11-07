using System;

namespace Hackman.Router.ART
{
    public class Leaf : Node
    {
        public static int count;

        public Leaf(byte[] key, object value) : base()
        {
            this.key = key;
            this.value = value;
            count++;
        }

        public Leaf(Leaf other) : base(other)
        {
            this.key = other.key;
            this.value = other.value;
            count++;
        }

        public override Node N_clone()
        {
            return new Leaf(this);
        }

        public virtual bool Matches(byte[] key)
        {
            if (this.key.Length != key.Length)
                return false;
            for (int i = 0; i < key.Length; i++)
            {
                if (this.key[i] != key[i])
                {
                    return false;
                }
            }

            return true;
        }

        public virtual bool Prefix_matches(byte[] prefix)
        {
            if (this.key.Length < prefix.Length)
                return false;
            for (int i = 0; i < prefix.Length; i++)
            {
                if (this.key[i] != prefix[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override Leaf Minimum()
        {
            return this;
        }

        public virtual int Longest_common_prefix(Leaf other, int depth)
        {
            int max_cmp = Math.Min(key.Length, other.key.Length) - depth;
            int idx;
            for (idx = 0; idx < max_cmp; idx++)
            {
                if (key[depth + idx] != other.key[depth + idx])
                {
                    return idx;
                }
            }

            return idx;
        }

        public override bool Insert(ChildPtr @ref, byte[] key, object value, int depth, bool force_clone)
        {
            bool clone = force_clone || this.refcount > 1;
            if (Matches(key))
            {
                if (clone)
                {
                    // Updating an existing value, but need to create a new leaf to
                    // reflect the change
                    @ref.Change(new Leaf(key, value));
                }
                else
                {
                    // Updating an existing value, and safe to make the change in
                    // place
                    this.value = value;
                }

                return false;
            }
            else
            {
                // New value
                // Create a new leaf
                Leaf l2 = new Leaf(key, value);

                // Determine longest prefix
                int longest_prefix = Longest_common_prefix(l2, depth);
                if (depth + longest_prefix >= this.key.Length || depth + longest_prefix >= key.Length)
                {
                    throw new NotSupportedException("keys cannot be prefixes of other keys");
                }

                // Split the current leaf into a node4
                ArtNode4 result = new ArtNode4();
                result.partial_len = longest_prefix;
                Node ref_old = @ref.Get();
                @ref.Change_no_decrement(result);
                System.Array.Copy(key, depth, result.partial, 0, Math.Min(Node.MAX_PREFIX_LEN, longest_prefix));

                // Add the leafs to the new node4
                result.Add_child(@ref, this.key[depth + longest_prefix], this);
                result.Add_child(@ref, l2.key[depth + longest_prefix], l2);
                ref_old.Decrement_refcount();

                // TODO: avoid the increment to self immediately followed by decrement
                return true;
            }
        }

        // Updating an existing value, but need to create a new leaf to
        // reflect the change
        // Updating an existing value, and safe to make the change in
        // place
        // New value
        // Create a new leaf
        // Determine longest prefix
        // Split the current leaf into a node4
        // Add the leafs to the new node4
        // TODO: avoid the increment to self immediately followed by decrement
        public override bool Delete(ChildPtr @ref, byte[] key, int depth, bool force_clone)
        {
            return Matches(key);
        }

        // Updating an existing value, but need to create a new leaf to
        // reflect the change
        // Updating an existing value, and safe to make the change in
        // place
        // New value
        // Create a new leaf
        // Determine longest prefix
        // Split the current leaf into a node4
        // Add the leafs to the new node4
        // TODO: avoid the increment to self immediately followed by decrement
        public override bool Exhausted(int i)
        {
            return i > 0;
        }

        // Updating an existing value, but need to create a new leaf to
        // reflect the change
        // Updating an existing value, and safe to make the change in
        // place
        // New value
        // Create a new leaf
        // Determine longest prefix
        // Split the current leaf into a node4
        // Add the leafs to the new node4
        // TODO: avoid the increment to self immediately followed by decrement
        public override int Decrement_refcount()
        {
            if (--this.refcount <= 0)
            {
                count--;

                // delete this;
                // Don't delete the actual key or value because they may be used
                // elsewhere
                return 32; // object size (8) + refcount (4) + pointer to key array (8) +
                // pointer to value (8) + padding (4)
            }

            return 0;
        }

        // Updating an existing value, but need to create a new leaf to
        // reflect the change
        // Updating an existing value, and safe to make the change in
        // place
        // New value
        // Create a new leaf
        // Determine longest prefix
        // Split the current leaf into a node4
        // Add the leafs to the new node4
        // TODO: avoid the increment to self immediately followed by decrement
        // delete this;
        // Don't delete the actual key or value because they may be used
        // elsewhere
        // object size (8) + refcount (4) + pointer to key array (8) +
        // pointer to value (8) + padding (4)
        internal object value;

        // Updating an existing value, but need to create a new leaf to
        // reflect the change
        // Updating an existing value, and safe to make the change in
        // place
        // New value
        // Create a new leaf
        // Determine longest prefix
        // Split the current leaf into a node4
        // Add the leafs to the new node4
        // TODO: avoid the increment to self immediately followed by decrement
        // delete this;
        // Don't delete the actual key or value because they may be used
        // elsewhere
        // object size (8) + refcount (4) + pointer to key array (8) +
        // pointer to value (8) + padding (4)
        internal readonly byte[] key;
    }
}