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
        public int[] HeadPosition { get; set; }
        public List<int> Moves { get; set; }
        private MainWindow Scope { get; }
        public int[] Target { get; set; }
        public bool IsFocusedOnTarget { get; set; }
        public String moveResult { get; set; }

        public Agent(MainWindow scope)
        {
            this.Moves = new List<int>();
            this.HeadPosition = new int[2];
            this.Scope = scope;
            this.Target = new int[2];
            IsFocusedOnTarget = false;
            moveResult = "";
        }
        public void Move(int direction)
        {
            if (direction == 0) return;
            direction = DirectionToCurrentDirection(direction);
            Scope.rectDict[HeadPosition[1] * Scope.height + HeadPosition[0]].Fill = Brushes.Gray;
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
