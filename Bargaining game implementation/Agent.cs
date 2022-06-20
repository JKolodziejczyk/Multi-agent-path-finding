using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Bargaining_game_implementation
{
    public class Agent
    {
        public Node HeadPosition { get; set; }
        private MainWindow Scope { get; }
        public Node Target { get; set; }
        public bool IsFocusedOnTarget { get; set; }
        public List<Node> Path { get; set; }
        public bool IsWaiting { get; set; }

        public Agent(MainWindow scope)
        {
            this.Scope = scope;
            IsFocusedOnTarget = false;
            Path = new List<Node>();
            IsWaiting = false;
        }
        public void Move(Node direction)
        {
            Scope.rectDict[HeadPosition.Index.y * Scope.height + HeadPosition.Index.x].Fill = Brushes.White;
            var node = Scope.map.Nodes.Find(x => x.Index == (HeadPosition.Index.x, HeadPosition.Index.y));
            HeadPosition.IsOccupied = false;
            HeadPosition = direction;
            Scope.rectDict[HeadPosition.Index.y * Scope.height + HeadPosition.Index.x].Fill = Brushes.Blue;
            direction.IsOccupied = true;
        }
    }
}
