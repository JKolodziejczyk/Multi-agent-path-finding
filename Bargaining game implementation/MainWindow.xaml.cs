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
        public int width = Properties.Settings.Default.Map;
        public int height = Properties.Settings.Default.Mode;
        public Dictionary<int, Rectangle> rectDict = new Dictionary<int, Rectangle>();
        List<Agent> agents = new List<Agent>();
        List<(int, int)> targets;
        Random rnd = new Random();
        public Graph map;

        public MainWindow()
        {
            map = new Graph(ConstMaps.Maps[Properties.Settings.Default.Map]);
            height = ConstMaps.Maps[Properties.Settings.Default.Map].Length;
            width = ConstMaps.Maps[Properties.Settings.Default.Map][0].Length;
            InitializeComponent();
            timer.Interval = TimeSpan.FromMilliseconds(1000 / FPS);
            timer.Tick += SimulationStep;
            SplitGrid();
            //grid.Focus();
            //grid.BeginInit();
            //start.IsEnabled = true;
            //start.Visibility = Visibility.Visible;
            DrawRectangles();
            InitializeAgents();
            //targets = DrawTargets();
            stop.IsEnabled = false;
        }
        public void DrawRectangles()
        {
            for (int j = 0; j < width; j++)
            {
                for (int i = 0; i < height; i++)
                {
                    Rectangle rect = new Rectangle();
                    if (map.Nodes.Exists(x => x.Index == (i, j)))
                    {
                        rect = new Rectangle()
                        {
                            Fill = Brushes.White
                        };
                    }
                    else
                    {
                        rect = new Rectangle()
                        {
                            Fill = Brushes.Black
                        };
                    }
                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                    grid.Children.Add(rect);
                    rectDict[j * height + i] = rect;
                    //if (i % 2 == 0) rectDict[j * height + i].Fill = Brushes.Green;
                }
            }
        }
        public List<(int,int)> TestTargets()
        {
            List<(int, int)> targets = new List<(int, int)>();
                var number1 = 28;
                var number2 = 30;
                if (rectDict[number2 * height + number1].Fill == Brushes.White)
                {
                    rectDict[number2 * height + number1].Fill = Brushes.Red;
                    targets.Add((number1, number2));
                    var node = map.Nodes.Find(x => x.Index == (number1, number2));
                    node.IsOccupied = true;
                }
            number1 = 2;
            number2 = 30;
            if (rectDict[number2 * height + number1].Fill == Brushes.White)
            {
                rectDict[number2 * height + number1].Fill = Brushes.Red;
                targets.Add((number1, number2));
                var node = map.Nodes.Find(x => x.Index == (number1, number2));
                node.IsOccupied = true;
            }
            return targets;
        }
        public List<(int,int)> DrawTargets()
        {
            List<(int, int)> targets = new List<(int, int)>();
            for (int i = 0; i < players; i++)
            {
                var number1 = rnd.Next(0, height);
                var number2 = rnd.Next(0, width);
                if (rectDict[number2*height+number1].Fill == Brushes.White)
                {
                    rectDict[number2*height+number1].Fill = Brushes.Red;
                    targets.Add((number1, number2));
                    var node = map.Nodes.Find(x => x.Index == (number1, number2));
                    node.IsOccupied = true;
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
            //TestAgents();
        }
        public void TestAgents()
        {
            var x = 30;
            var y = 4;
            var headPosition = x * height + y;
            if (rectDict[headPosition].Fill == Brushes.White)
            {
                agents[0].HeadPosition = map.Nodes.Find(node => node.Index == (y, x));
                rectDict[headPosition].Fill = Brushes.Blue;
            }
            x = 30;
            y = 20;
            headPosition = x * height + y;
            if (rectDict[headPosition].Fill == Brushes.White)
            {
                agents[1].HeadPosition = map.Nodes.Find(node => node.Index == (y, x));
                rectDict[headPosition].Fill = Brushes.Blue;
            }
        }
        public void DrawAgents()
        {
            foreach(var agent in agents)
            {
                bool draw = true;
                while (draw)
                {
                    var x = rnd.Next(0, width);
                    var y = rnd.Next(0, height);
                    var headPosition = x * height + y;
                    if (rectDict[headPosition].Fill == Brushes.White)
                    {
                        agent.HeadPosition = map.Nodes.Find(node=> node.Index == (y, x));
                        rectDict[headPosition].Fill = Brushes.Blue;
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
                targets = DrawTargets();
                GetPaths();
            }
            foreach (var agent in agents)
            {
                Node direction = new Node();
                if (agent.Path.Any())
                {
                    direction = agent.Path.FirstOrDefault();
                    if (direction.IsOccupied && direction.Index!=agent.Target.Index)
                    {
                        if (agent.IsWaiting)
                        {
                            foreach (var node in agent.HeadPosition.Neighbors)
                            {
                                if (!node.IsOccupied)
                                {
                                    agent.Path.Insert(0, agent.HeadPosition);
                                    agent.Move(node);
                                    agent.IsWaiting = false;
                                }
                            }
                        }
                        else
                        {
                            agent.IsWaiting = true;
                        }
                        continue;
                    }
                    agent.IsWaiting = false;
                    agent.Path.RemoveAt(0);
                    agent.Move(direction);
                }
                else
                {
                    agent.IsFocusedOnTarget = false;
                }
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
        public void GetPaths()
        {
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
                var node = map.Nodes.Find(x => x.Index == (target.Item1, target.Item2));
                agent.Target = node;
                agent.Path = map.AStarSearch(agent.HeadPosition, agent.Target);
            }
            foreach(var agent in agents)
            {
                var node = map.Nodes.Find(x => x.Index == (agent.HeadPosition.Index.x, agent.HeadPosition.Index.y));
                node.IsOccupied = true;
            }
        }
    }
}
