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
        public HashSet<(Node position, int time)> SpaceTime { get; set; }
        public Graph(int[][] nodes)
        {
            SpaceTime = new HashSet<(Node position, int time)>();
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
        public List<Node> AStarSearch(Node start, Node finish, int iteration = 0)
        {
            int timer = iteration;
            int tries = 0;
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
                timer++;
                tries++;
                bool moved = false;
                foreach(var node in current.Neighbors)
                {
                    if ((node.IsOccupied && node!=finish)||SpaceTime.Contains((node,timer)))
                    {
                        continue;
                    }
                    if(SpaceTime.Contains((current,timer)) && SpaceTime.Contains((node, timer - 1)))
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
                        moved = true;
                    }
                }
                if (!moved)
                {
                    if (queue.Count == 0)
                    {
                        queue.Enqueue(current);
                    }
                }
                if (tries == 2000)
                {
                    return new List<Node>();
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
            timer = iteration;
            path.Reverse();
            SpaceTime.Add((start, timer));
            foreach(var pos in path)
            {
                timer++;
                SpaceTime.Add((pos, timer));
            }
            return path;
        }
        public double Heurestic(Node finish, Node current)
        {
            var dx = Math.Abs(current.Index.x - finish.Index.x);
            var dy = Math.Abs(current.Index.y - finish.Index.y);
            return (dx + dy) * 0.9999;
        }
        public bool IsConnected(List<(int,int)> targets)
        {
            var queue = new Queue<Node>();
            Node start = new Node();
            int counter = 1;
            foreach(var node in Nodes)
            {
                if (!node.IsOccupied)
                {
                    start = node;
                    break;
                }
            }
            start.Visited = true;
            Node next;
            queue.Enqueue(start);
            while (queue.Any())
            {
                next = queue.Dequeue();
                foreach(var node in next.Neighbors)
                {
                    if (!node.IsOccupied && !node.Visited)
                    {
                        queue.Enqueue(node);
                        node.Visited = true;
                        counter++;
                    }
                }
            }
            foreach(var node in Nodes)
            {
                node.Visited = false;
            }
            foreach(var target in targets)
            {
                var node = Nodes.Find(x => x.Index == (target.Item1, target.Item2));
                if(!node.Neighbors.Any(x => !x.IsOccupied))
                {
                    return false;
                }
            }
            return counter == Nodes.Count() - targets.Count();
        }
    }
}
