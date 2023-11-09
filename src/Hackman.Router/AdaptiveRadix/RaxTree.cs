using System;
using System.Collections.Generic;
using System.Text;

namespace Hackman.Router.AdaptiveRadix
{
    public struct raxNode
    {
        public bool iskey; // Does this node contain a key?
                            
        public bool isnull; // Associated value is NULL (don't store it).
                              
        public bool iscompr; // Node is compressed.
                               
        public uint size; // Number of children, or compressed string len.

        public byte[] data;
    }

    public struct raxTree
    {
        public raxNode head;
        public ulong numele;
        public ulong numnodes;
    }
}
