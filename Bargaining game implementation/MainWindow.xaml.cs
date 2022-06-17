using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Bargaining_game_implementation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        public int players = Properties.Settings.Default.Players;
        public int FPS = Properties.Settings.Default.FPS;
        public int width = Properties.Settings.Default.Width;
        public int height = Properties.Settings.Default.Height;
        public Dictionary<int, Rectangle> rectDict = new Dictionary<int, Rectangle>();
        List<Agent> agents = new List<Agent>();
        Random rnd = new Random();
        public int[] simulationArray = new int[Properties.Settings.Default.Width* Properties.Settings.Default.Height];

        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromMilliseconds(1000 / FPS);
            timer.Tick += SimulationStep;
            SplitGrid();
            //grid.Focus();
            //grid.BeginInit();
            //start.IsEnabled = true;
            start.Visibility = Visibility.Visible;
            DrawRectangles();
            InitializeAgents();
            //DrawTargets();
            stop.IsEnabled = false;
        }
        public void DrawRectangles()
        {
            for (int j = 0; j < width; j++)
            {
                for (int i = 0; i < height; i++)
                {
                    Rectangle rect = new Rectangle()
                    {
                        Fill = Brushes.Gray
                    };
                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                    grid.Children.Add(rect);
                    rectDict[j * height + i] = rect;
                    //if (i % 2 == 0) rectDict[j * height + i].Fill = Brushes.Green;
                }
            }
        }
        public List<(int,int)> DrawTargets()
        {
            List<(int, int)> targets = new List<(int, int)>();
            for (int i = 0; i < players; i++)
            {
                var number1 = rnd.Next(0, height);
                var number2 = rnd.Next(0, width);
                if (rectDict[number2*height+number1].Fill == Brushes.Gray)
                {
                    rectDict[number2*height+number1].Fill = Brushes.Red;
                    targets.Add((number1, number2));
                }
                else
                {
                    i--;
                }
            }
            return targets;
        }

        public void SplitGrid()
        {
            var h = this.Height;
            var w = this.Width;
            for(int j = 0; j<width; j++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(w/width, GridUnitType.Star) });
            }
            for (int i = 0; i < height; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(h/height, GridUnitType.Star) });
            }
        }
        public void InitializeAgents()
        {
            for (int i = 0; i < players; i++)
            {
                agents.Add(new Agent(this));
            }
            DrawAgents();
        }
        public void DrawAgents()
        {
            foreach(var agent in agents)
            {
                bool draw = true;
                while (draw)
                {
                    agent.HeadPosition[0] = rnd.Next(0, height);
                    agent.HeadPosition[1] = rnd.Next(0, width);
                    var headPosition = agent.HeadPosition[1] * height + agent.HeadPosition[0];
                    if (rectDict[headPosition].Fill == Brushes.Gray)
                    {
                        rectDict[headPosition].Fill = Brushes.Green;
                        draw = false;
                    }
                }
            }
        }
        public void SimulationStep(object sender, EventArgs e)
        {
            bool moving = false;
            foreach(var agent in agents)
            {
                if (agent.IsFocusedOnTarget) moving = true;
            }
            if (!moving)
            {
                Iteration();
            }
            foreach (var agent in agents)
            {
                int direction = 0;
                if (agent.Moves.Any())
                {
                    direction = agent.Moves.FirstOrDefault();
                    agent.Moves.RemoveAt(0);
                }
                else
                {
                    agent.IsFocusedOnTarget = false;
                }
                agent.Move(direction);
            }
        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            stop.IsEnabled = true;
            start.IsEnabled = false;
            //SimulationStep();
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.Show();
            timer.Stop();
            this.Close();
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
                stop.Content = "Resume";
            }
            else
            {
                timer.Start();
                stop.Content = "Stop";
            }
        }

        bool CheckMax(Agent snake1, Agent snake2)
        {
            int maxOld = Math.Max(
                Math.Abs(snake1.Target[0] - snake1.HeadPosition[0])+Math.Abs(snake1.Target[1] - snake1.HeadPosition[1]),
                Math.Abs(snake2.Target[0] - snake2.HeadPosition[0])+Math.Abs(snake2.Target[1] - snake2.HeadPosition[1])
                );
            int maxNew = Math.Max(
                Math.Abs(snake1.Target[0] - snake2.HeadPosition[0])+Math.Abs(snake1.Target[1] - snake2.HeadPosition[1]),
                Math.Abs(snake2.Target[0] - snake1.HeadPosition[0])+Math.Abs(snake2.Target[1] - snake1.HeadPosition[1])
               );
            if (maxOld > maxNew) return false;

            return true;
                //wywala true jak zmiana jest niepotrzebna
        }

        void ChangeTargets(Agent snake1, Agent snake2)
        {
            var temporary = snake1.Target[0];
            snake1.Target[0] = snake2.Target[0];
            snake2.Target[0] = temporary;

            temporary = snake1.Target[1];
            snake1.Target[1] = snake2.Target[1];
            snake2.Target[1] = temporary;
        }
        public void Iteration()
        {

            List<(int, int)> targets = DrawTargets();
            var targetsTemp = new List<(int, int)>(targets);
            foreach (var agent in agents)
            {
                agent.IsFocusedOnTarget = true;
                var target = targetsTemp.FirstOrDefault();
                try
                {
                    targetsTemp.RemoveAt(0);
                }
                catch
                {
                    /*MessageBox.Show("Error",
                    "Save", MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question, MessageBoxResult.Cancel) != MessageBoxResult.Yes);*/
                }
                agent.Target[0] = target.Item1;
                agent.Target[1] = target.Item2;
            }

            int[] fruitArray = new int[width * height];
            foreach (var target in targets)
            {
                fruitArray[target.Item2 * height + target.Item1] = 2;
            }

            bool change = true;
            while (change)
            {
                change = false;
                foreach (var agent in agents)
                {
                    foreach (var agent2 in agents)
                    {
                        if (agent2 == agent) continue;
                        if (!CheckMax(agent, agent2))
                        {
                            ChangeTargets(agent, agent2);
                            // przy maksymalnej odleglosci - zmiana
                            change = true;
                        }

                    }
                }
            }
        }
    }
}
