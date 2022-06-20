using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DotNetty.Common.Utilities;

namespace Bargaining_game_implementation
{
    public class Graph
    {
        public List<Node> Nodes { get; set; }

        public Graph(int[][] nodes)
        {
            Nodes = new List<Node>();
            for(int i=0; i<nodes.Length; i++)
            {
                for(int j=0; j<nodes[i].Length; j++)
                {
                    if (nodes[i][j] == 0)
                    {
                        Nodes.Add(new Node(i, j));
                    }
                }
            }
            for (int i = 0; i < nodes.Length; i++)
            {
                for (int j = 0; j < nodes[i].Length; j++)
                {
                    if (nodes[i][j] == 0)
                    {
                        if(Nodes.Exists(x=>x.Index==(i,j)))
                        {
                            var node = Nodes.Find(x => x.Index == (i, j));
                            if (Nodes.Exists(x => x.Index == (i + 1, j)))
                            {
                                node.Neighbors.Add(Nodes.Find(x => x.Index == (i + 1, j)));
                            }
                            if (Nodes.Exists(x => x.Index == (i - 1, j)))
                            {
                                node.Neighbors.Add(Nodes.Find(x => x.Index == (i - 1, j)));
                            }
                            if (Nodes.Exists(x => x.Index == (i, j + 1)))
                            {
                                node.Neighbors.Add(Nodes.Find(x => x.Index == (i, j + 1)));
                            }
                            if (Nodes.Exists(x => x.Index == (i, j - 1)))
                            {
                                node.Neighbors.Add(Nodes.Find(x => x.Index == (i, j - 1)));
                            }
                        }
                    }
                }
            }
        }
        public List<Node> AStarSearch(Node start, Node finish)
        {
            var queue = new PriorityQueue<Node>();
            queue.Enqueue(start);
            var cameFrom = new Dictionary<Node,Node>();
            var costSoFar = new Dictionary<Node, double>();
            cameFrom[start] = start;
            costSoFar[start] = 0;
            while (queue.Count != 0)
            {
                Node current = queue.Dequeue();
                if(current == finish)
                {
                    break;
                }
                foreach(var node in current.Neighbors)
                {
                    if (node.IsOccupied && node!=finish)
                    {
                        continue;
                    }
                    double newCost = costSoFar[current] + 1;
                    var exist = costSoFar.TryGetValue(node, out double value);
                    if (!costSoFar.ContainsKey(node) || newCost < value)
                    {
                        costSoFar[node] = newCost;
                        var priority = newCost + Heurestic(finish, node);
                        node.Priority = priority;
                        queue.Enqueue(node);
                        cameFrom[node] = current;
                    }
                }
            }
            var path = new List<Node>();
            Node curr = finish;
            path.Add(finish);
            while (true)
            {
                var _ = cameFrom.TryGetValue(curr, out Node node);
                curr = node;
                if (curr == start)
                {
                    break;
                }
                path.Add(node);
            }
            path.Reverse();
            return path;
        }
        public double Heurestic(Node finish, Node current)
        {
            var dx = Math.Abs(current.Index.x - finish.Index.x);
            var dy = Math.Abs(current.Index.y - finish.Index.y);
            return (dx + dy) * 0.9999;
        }
    }
}
