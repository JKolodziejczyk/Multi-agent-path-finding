using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Bargaining_game_implementation
{
    public class Snake
    {
        public List<int> Body { get; set; }
        public int CurrentDirection { get; set; }
        public int[] HeadPosition { get; set; }
        public int[] TailPosition { get; set; }
        public List<int> Moves { get; set; }
        private MainWindow Scope { get; }
        public int[] Target { get; set; }
        public bool IsFocusedOnTarget { get; set; }

        public Snake(int length, MainWindow scope)
        {
            this.Body = new List<int>();
            this.Moves = new List<int>();
            this.Body.Add(1);
            this.HeadPosition = new int[2];
            this.TailPosition = new int[2];
            this.Scope = scope;
            this.Target = new int[2];
            IsFocusedOnTarget = false;
            CurrentDirection = 3;
        }
        public bool CheckIfMovePossible(int direction)
        {
            direction = DirectionToCurrentDirection(direction);
            switch (direction)
            {
                case 1:
                    return Scope.rectDict[(HeadPosition[1] - 1) * Scope.height + HeadPosition[0]].Fill != Brushes.Green;
                case 2:
                    return Scope.rectDict[HeadPosition[1] * Scope.height + HeadPosition[0] + 1].Fill != Brushes.Green;
                case 3:
                    return Scope.rectDict[(HeadPosition[1] + 1) * Scope.height + HeadPosition[0]].Fill != Brushes.Green;
                case 4:
                    return Scope.rectDict[HeadPosition[1] * Scope.height + HeadPosition[0] - 1].Fill != Brushes.Green;
                default:
                    break;
            }
            return true;
        }
        public void Move(int direction)
        {
            if (direction == 0) return;
            CurrentDirection = direction;
            direction = DirectionToCurrentDirection(direction);
            Body.Insert(0, direction);
            switch (direction)
            {
                case 1: //down
                    HeadPosition[1]--;
                    break;
                case 2: //right
                    HeadPosition[0]++;
                    break;
                case 3: //up
                    HeadPosition[1]++;
                    break;
                case 4: //left
                    HeadPosition[0]--;
                    break;
                default:
                    break;
            }
            if (Scope.rectDict[HeadPosition[1] * Scope.height + HeadPosition[0]].Fill != Brushes.Red)
            {
                Scope.rectDict[TailPosition[1] * Scope.height + TailPosition[0]].Fill = Brushes.Gray;
                switch (Body.Last())
                {
                    case 1:
                        TailPosition[1]--;
                        break;
                    case 2:
                        TailPosition[0]++;
                        break;
                    case 3:
                        TailPosition[1]++;
                        break;
                    case 4:
                        TailPosition[0]--;
                        break;
                    default:
                        break;
                }
                Body.RemoveAt(Body.Count - 1);
            }
            Scope.rectDict[HeadPosition[1] * Scope.height + HeadPosition[0]].Fill = Brushes.Green;
        }
        public enum Directions
        {
            right = 1,
            up = 2,
            left = 3,
            down = 4
        }
        public int DirectionToCurrentDirection(int direction)
        {
            switch (direction)
            {
                case 1:
                    return 3;
                case 2:
                    return 4;
                case 3:
                    return 1;
                case 4:
                    return 2;
                default:
                    return 0;
            }
        }
    }
}
