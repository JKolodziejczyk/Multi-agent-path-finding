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
        List<Snake> snakes = new List<Snake>();
        Random rnd = new Random();
        public int[] backgroundBoard = new int[Properties.Settings.Default.Width * Properties.Settings.Default.Height];

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
            InitializeSnakes();
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
        public void InitializeSnakes()
        {
            for (int i = 0; i < players; i++)
            {
                snakes.Add(new Snake(1,this));
            }
            DrawSnakes();
        }
        public void DrawSnakes()
        {
            foreach(var snake in snakes)
            {
                snake.HeadPosition[0] = rnd.Next(5, height - 5);
                snake.HeadPosition[1] = rnd.Next(5, width - 5);
                snake.TailPosition[0] = snake.HeadPosition[0];
                snake.TailPosition[1] = snake.HeadPosition[1];
                var headPosition = snake.HeadPosition[1] * height + snake.HeadPosition[0];
                rectDict[headPosition].Fill = Brushes.Green;
                foreach(var direction in snake.Body)
                {
                    switch (direction)
                    {
                        case 1:
                            headPosition += height;
                            rectDict[headPosition].Fill = Brushes.Green;
                            snake.TailPosition[1]++;
                            break;
                        case 2:
                            headPosition -= 1;
                            rectDict[headPosition].Fill = Brushes.Green;
                            snake.TailPosition[0]--;
                            break;
                        case 3:
                            headPosition -= height;
                            rectDict[headPosition].Fill = Brushes.Green;
                            snake.TailPosition[1]--;
                            break;
                        case 4:
                            headPosition += 1;
                            rectDict[headPosition].Fill = Brushes.Green;
                            snake.TailPosition[0]++;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public void SimulationStep(object sender, EventArgs e)
        {
            bool moving = false;
            foreach(var snake in snakes)
            {
                if (snake.IsFocusedOnTarget) moving = true;
            }
            if (!moving)
            {
                iteration();
            }
            foreach (var snake in snakes)
            {
                int direction = 0;
                if (snake.Moves.Any())
                {
                    direction = snake.Moves.FirstOrDefault();
                    snake.Moves.RemoveAt(0);
                }
                else
                {
                    snake.IsFocusedOnTarget = false;
                }
                        /*
                    switch (direction)
                    {
                        case 1:
                            if (snake.CurrentDirection != 3)
                            {
                                if (snake.CheckIfMovePossible(direction))
                                {
                                    impossibleDirection = false;
                                }
                            }
                            break;
                        case 2:
                            if (snake.CurrentDirection != 4)
                            {
                                if (snake.CheckIfMovePossible(direction))
                                {
                                    impossibleDirection = false;
                                }
                            }
                            break;
                        case 3:
                            if (snake.CurrentDirection != 1)
                            {
                                if (snake.CheckIfMovePossible(direction))
                                {
                                    impossibleDirection = false;
                                }
                            }
                            break;
                        case 4:
                            if (snake.CurrentDirection != 2)
                            {
                                if (snake.CheckIfMovePossible(direction))
                                {
                                    impossibleDirection = false;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                        */
                snake.Move(direction);
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

        public void fixBoard()
        {
            //przejscie po calej planszy i sfixowanie wartosci zielonych pol
        }

        bool checkMax(Snake snake1, Snake snake2)
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

        void changeTargets(Snake snake1, Snake snake2)
        {
            var temporary = snake1.Target[0];
            snake1.Target[0] = snake2.Target[0];
            snake2.Target[0] = temporary;

            temporary = snake1.Target[1];
            snake1.Target[1] = snake2.Target[1];
            snake2.Target[1] = temporary;
        }

        void findBasePath(Snake snake)
        {
            int snakeX = snake.HeadPosition[1];
            int snakeY = snake.HeadPosition[0];
            if(snake.Target[0] > snakeY)
            {
                while(snake.Target[0] != snakeY)
                {
                    snake.Moves.Add(4); //down
                    snakeY++;
                }
            }
            else
            {
                while(snake.Target[0] != snakeY)
                {
                    snake.Moves.Add(2); //up
                    snakeY--;
                }
            }
            
            if(snake.Target[1] > snakeX)
            {
                while(snake.Target[1] != snakeX)
                {
                    snake.Moves.Add(1); //right
                    snakeX++;
                }
            }
            else
            {
                while(snake.Target[1] != snakeX)
                {
                    snake.Moves.Add(3); //left
                    snakeX--;
                }
            }

        }
        public void iteration()
        {
            //fixBoard();
            List<(int, int)> targets = DrawTargets();
            foreach(var snake in snakes)
            {
                snake.IsFocusedOnTarget = true;
                var target = targets.FirstOrDefault();
                try
                {
                    targets.RemoveAt(0);
                }
                catch
                {
                    /*MessageBox.Show("Error",
                    "Save", MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question, MessageBoxResult.Cancel) != MessageBoxResult.Yes);*/
                }
                snake.Target[0]=target.Item1;
                snake.Target[1]=target.Item2;
            }

            bool change=true;
            while (change)
            {
                change=false;
                foreach(var snake in snakes)
                {
                    foreach(var snake2 in snakes)
                    {
                        if(snake2 == snake) continue;
                        if (!checkMax(snake,snake2) )
                        {
                            changeTargets(snake, snake2);
                            // przy maksymalnej odleglosci - zmiana
                            change = true;
                        }
                        
                    }
                }
            }

            foreach(var snake in snakes)
            {
                findBasePath(snake);
            }

            /*change = true;
            while (change)
            {
                change=false;
                foreach(var snake in snakes)
                {
                    foreach(var snake2 in snakes)
                    {
                        if(snake2 == snake) continue;
                        if () 
                        {
                            
                        }
                        //wyznaczenie sciezek dla obu - jezeli nie bylo zamiany to sprawdzenie czy maja juz sciezki
                   }
                }
            }*/
        }
    }
}
