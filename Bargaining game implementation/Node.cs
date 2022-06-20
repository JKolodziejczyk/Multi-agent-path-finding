using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bargaining_game_implementation
{
    public class Node : IComparable
    {
        public (int x, int y) Index { get; set; }
        public bool IsOccupied { get; set; }
        public NodeList Neighbors { get; set; }
        public double Priority { get; set; }
        public bool Visited { get; set; }
        public Node() { }
        public Node(int x, int y)
        {
            Index = (x, y);
            IsOccupied = false;
            Priority = 0;
            Neighbors = new NodeList();
            Visited = false;
        }

        public int CompareTo(object obj)
        {
            var node = obj as Node;
            var p = Priority.CompareTo(node.Priority);
            return p;
        }
    }
    public class NodeList : Collection<Node>
    { 
        public NodeList() : base() { }

    }
}
