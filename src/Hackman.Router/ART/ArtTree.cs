using System;
using System.Collections.Generic;

namespace Hackman.Router.ART
{
    public class ArtTree : ChildPtr
    {
        public ArtTree()
        {
        }

        public ArtTree(ArtTree other)
        {
            root = other.root;
            num_elements = other.num_elements;
        }

        public virtual ArtTree Snapshot()
        {
            ArtTree b = new ArtTree();
            if (root != null)
            {
                b.root = Node.N_clone(root);
                b.root.refcount++;
            }

            b.num_elements = num_elements;
            return b;
        }

        public override Node Get()
        {
            return root;
        }

        public override void Set(Node n)
        {
            root = n;
        }

        public virtual object Search(byte[] key)
        {
            Node n = root;
            int prefix_len, depth = 0;
            while (n != null)
            {
                if (n is Leaf)
                {
                    Leaf l = (Leaf)n;

                    // Check if the expanded path matches
                    if (l.Matches(key))
                    {
                        return l.value;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    ArtNode an = (ArtNode)(n);

                    // Bail if the prefix does not match
                    if (an.partial_len > 0)
                    {
                        prefix_len = an.Check_prefix(key, depth);
                        if (prefix_len != Math.Min(Node.MAX_PREFIX_LEN, an.partial_len))
                        {
                            return null;
                        }

                        depth += an.partial_len;
                    }

                    if (depth >= key.Length)
                        return null;

                    // Recursively search
                    ChildPtr child = an.Find_child(key[depth]);
                    n = (child != null) ? child.Get() : null;
                    depth++;
                }
            }

            return null;
        }

        // Check if the expanded path matches
        // Bail if the prefix does not match
        // Recursively search
        public virtual void Insert(byte[] key, object value)
        {
            if (Node.Insert(root, this, key, value, 0, false))
                num_elements++;
        }

        // Check if the expanded path matches
        // Bail if the prefix does not match
        // Recursively search
        public virtual void Delete(byte[] key)
        {
            if (root != null)
            {
                bool child_is_leaf = root is Leaf;
                bool do_delete = root.Delete(this, key, 0, false);
                if (do_delete)
                {
                    num_elements--;
                    if (child_is_leaf)
                    {
                        // The leaf to delete is the root, so we must remove it
                        root = null;
                    }
                }
            }
        }

        // Check if the expanded path matches
        // Bail if the prefix does not match
        // Recursively search
        // The leaf to delete is the root, so we must remove it
        public virtual System.Collections.Generic.IEnumerator<Tuple<byte[], Object>> Iterator()
        {
            return new ArtIterator(root);
        }

        // Check if the expanded path matches
        // Bail if the prefix does not match
        // Recursively search
        // The leaf to delete is the root, so we must remove it
        public virtual IEnumerator<Tuple<byte[], Object>> PrefixIterator(byte[] prefix)
        {
            // Find the root node for the prefix
            Node n = root;
            int prefix_len, depth = 0;
            while (n != null)
            {
                if (n is Leaf)
                {
                    Leaf l = (Leaf)n;

                    // Check if the expanded path matches
                    if (l.Prefix_matches(prefix))
                    {
                        return new ArtIterator(l);
                    }
                    else
                    {
                        return new ArtIterator(null);
                    }
                }
                else
                {
                    if (depth == prefix.Length)
                    {
                        // If we have reached appropriate depth, return the iterator
                        if (n.Minimum().Prefix_matches(prefix))
                        {
                            return new ArtIterator(n);
                        }
                        else
                        {
                            return new ArtIterator(null);
                        }
                    }
                    else
                    {
                        ArtNode an = (ArtNode)(n);

                        // Bail if the prefix does not match
                        if (an.partial_len > 0)
                        {
                            prefix_len = an.Prefix_mismatch(prefix, depth);
                            if (prefix_len == 0)
                            {
                                // No match, return empty
                                return new ArtIterator(null);
                            }
                            else if (depth + prefix_len == prefix.Length)
                            {
                                // Prefix match, return iterator
                                return new ArtIterator(n);
                            }
                            else
                            {
                                // Full match, go deeper
                                depth += an.partial_len;
                            }
                        }

                        // Recursively search
                        ChildPtr child = an.Find_child(prefix[depth]);
                        n = (child != null) ? child.Get() : null;
                        depth++;
                    }
                }
            }

            return new ArtIterator(null);
        }

        // Check if the expanded path matches
        // Bail if the prefix does not match
        // Recursively search
        // The leaf to delete is the root, so we must remove it
        // Find the root node for the prefix
        // Check if the expanded path matches
        // If we have reached appropriate depth, return the iterator
        // Bail if the prefix does not match
        // No match, return empty
        // Prefix match, return iterator
        // Full match, go deeper
        // Recursively search
        public virtual long Size()
        {
            return num_elements;
        }

        // Check if the expanded path matches
        // Bail if the prefix does not match
        // Recursively search
        // The leaf to delete is the root, so we must remove it
        // Find the root node for the prefix
        // Check if the expanded path matches
        // If we have reached appropriate depth, return the iterator
        // Bail if the prefix does not match
        // No match, return empty
        // Prefix match, return iterator
        // Full match, go deeper
        // Recursively search
        public virtual int Destroy()
        {
            if (root != null)
            {
                int result = root.Decrement_refcount();
                root = null;
                return result;
            }
            else
            {
                return 0;
            }
        }

        // Check if the expanded path matches
        // Bail if the prefix does not match
        // Recursively search
        // The leaf to delete is the root, so we must remove it
        // Find the root node for the prefix
        // Check if the expanded path matches
        // If we have reached appropriate depth, return the iterator
        // Bail if the prefix does not match
        // No match, return empty
        // Prefix match, return iterator
        // Full match, go deeper
        // Recursively search
        private Node root = null;

        // Check if the expanded path matches
        // Bail if the prefix does not match
        // Recursively search
        // The leaf to delete is the root, so we must remove it
        // Find the root node for the prefix
        // Check if the expanded path matches
        // If we have reached appropriate depth, return the iterator
        // Bail if the prefix does not match
        // No match, return empty
        // Prefix match, return iterator
        // Full match, go deeper
        // Recursively search
        private long num_elements = 0;
    }
}