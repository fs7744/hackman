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

        public void AddValue(T value)
        {
            if (Value == null)
            {
                Value = new List<T>();
            }
            Value.Add(value);
            HasValue = true;
        }
    }

    public class Tree<T>
    {
        public Node<T> Root = new Node<T>() { Children = new List<Node<T>>(), Path = string.Empty };

        public StringComparison Comparison = StringComparison.Ordinal;

        public (int index, Node<T> node, int nodePathLength) FindCommonPrexNode(string path)
        {
            return FindCommonPrexNode(path, Root, -1);
        }

        public (int index, Node<T> node, int nodePathLength) FindCommonPrexNode(string path, Node<T> node, int index)
        {
            var idx = index;
            if ((node.Path == null || node.Path.Length == 0) && node != Root) { return (idx, node, 0); }
            var p = node.Path;
            int i = 1;
            for (; i <= p.Length; i++)
            {
                if (StringHelper.Equals(path, idx + 1, p, i - 1, 1, Comparison))
                {
                    idx++;
                }
                else
                {
                    return (index + i - 1, node, i - 1);
                }
            }
            if (node.Children == null || node.Children.Count == 0) { return (index + p.Length, node, p.Length); }
            idx = index + p.Length;
            foreach (var item in node.Children)
            {
                var (xp, xn, xpi) = FindCommonPrexNode(path, item, idx);
                if (xpi > 0)
                {
                    return (xp, xn, xpi);
                }
            }
            return (index + p.Length, node, p.Length);
        }

        public void Insert(string path, T value)
        {
            var (index, node, nodePathLength) = FindCommonPrexNode(path);
            if (node == Root)
            {
                var n = new Node<T>() { Path = path };
                n.AddValue(value);
                node.Children.Add(n);
            }
            else if (nodePathLength == node.Path.Length)
            {
                node.AddValue(value);
            }
            else
            {
                var newNode = new Node<T>() { Path = node.Path.Substring(nodePathLength), Children = node.Children, Value = node.Value, HasValue = node.HasValue };
                node.Children = new List<Node<T>> { newNode };
                node.Path = node.Path.Substring(0, nodePathLength);
                node.Value = null;
                node.HasValue = false;
                if (path.Length <= index + 1)
                {
                    node.AddValue(value);
                }
                else
                {
                    var n = new Node<T>() { Path = path.Substring(index + 1) };
                    n.AddValue(value);
                    node.Children.Add(n);
                }
            }
        }
    }
}