using System;
using System.Collections;
using System.Collections.Generic;

namespace Hackman.Router.ART
{
    internal class ArtIterator : IEnumerator<Tuple<byte[], Object>>
    {
        private Stack<Node> elemStack = new Stack<Node>();
        private Stack<int> idxStack = new Stack<int>();
        private Node root;

        public ArtIterator(Node root)
        {
            this.root = root;
        }

        public Tuple<byte[], object> Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            elemStack.Clear();
            idxStack.Clear();
            root = null;
        }

        public bool MoveNext()
        {
            if (elemStack.Count != 0)
            {
                Leaf leaf = (Leaf)elemStack.Peek();
                byte[] key = leaf.key;
                object value = leaf.value;

                // Mark the leaf as consumed
                idxStack.Push(idxStack.Pop() + 1);
                MaybeAdvance();
                Current = new Tuple<byte[], object>(key, value);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            if (root != null)
            {
                elemStack.Push(root);
                idxStack.Push(0);
                MaybeAdvance();
            }
        }

        // Mark the leaf as consumed
        // Postcondition: if the stack is nonempty, the top of the stack must contain a leaf
        private void MaybeAdvance()
        {
            // Pop exhausted nodes
            while (elemStack.Count != 0 && elemStack.Peek().Exhausted(idxStack.Peek()))
            {
                elemStack.Pop();
                idxStack.Pop();
                if (elemStack.Count != 0)
                {
                    // Move on by advancing the exhausted node's parent
                    idxStack.Push(idxStack.Pop() + 1);
                }
            }

            if (elemStack.Count != 0)
            {
                // Descend to the next leaf node element
                while (true)
                {
                    if (elemStack.Peek() is Leaf)
                    {
                        // Done - reached the next element
                        break;
                    }
                    else
                    {
                        // Advance to the next child of this node
                        ArtNode cur = (ArtNode)elemStack.Peek();
                        idxStack.Push(cur.NextChildAtOrAfter(idxStack.Pop()));
                        Node child = cur.ChildAt(idxStack.Peek());

                        // Push it onto the stack
                        elemStack.Push(child);
                        idxStack.Push(0);
                    }
                }
            }
        }
    }
}