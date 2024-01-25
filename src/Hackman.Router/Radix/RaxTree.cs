using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hackman.Router.Radix
{
    internal static class StringHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Equals(string? strA, int indexA, string? strB, int indexB, int length, StringComparison comparisonType)
        {
            return string.Compare(strA, indexA, strB, indexB, length, comparisonType) == 0;
        }
    }

    public class Node<T>
    {
        public string Path;

        public bool HasValue;

        public List<T> Value;

        public List<Node<T>> Children;
    }

    public class Tree<T>
    {
        public Node<T> Root = new Node<T>() { Value = new List<T>(), Children = new List<Node<T>>(), Path = string.Empty };

        public StringComparison Comparison = StringComparison.Ordinal;

        public (int index, Node<T> node, int nodePathLength) FindCommonPrexNode(string path)
        {
            return FindCommonPrexNode(path, Root, 0);
        }

        public (int index, Node<T> node, int nodePathLength) FindCommonPrexNode(string path, Node<T> node, int index)
        {
            if ((node.Path == null || node.Path.Length == 0) && node != Root) { return (index, node, 0); }
            var p = node.Path;
            int i = 1;
            for (; i <= p.Length; i++)
            {
                if (StringHelper.Equals(path, index, p, i - 1, i, Comparison))
                {
                    index++;
                }
                else
                {
                    return (index, node, i - 1);
                }
            }
            if (node.Children == null || node.Children.Count == 0) { return (index, node, p.Length); }
            foreach (var item in node.Children)
            {
                var (xp, xn, xpi) = FindCommonPrexNode(path, item, index);
                if (xpi > 0)
                {
                    return (xp, xn, xpi);
                }
            }
            return (index, node, p.Length);
        }

        public void Insert(string path, T value)
        {
        }
    }
}