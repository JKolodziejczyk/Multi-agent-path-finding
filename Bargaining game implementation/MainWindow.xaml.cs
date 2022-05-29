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
        int players = 0;
        int FPS = 10;
        int width = 20;
        int height = 12;
        Dictionary<int, Rectangle> rectDict = new Dictionary<int, Rectangle>();

        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromMilliseconds(1000 / FPS);
            //timer.Tick += SimulationLoop;
            timer.Start();
            SplitGrid(grid);
            grid.Focus();
            grid.BeginInit();
            DrawRectangles(grid);

        }
        public void DrawRectangles(Grid grid)
        {
            for (int j = 0; j < width; j++)
            {
                for (int i = 0; i < height; i++)
                {
                    Rectangle rect = new Rectangle()
                    {
                        Fill = Brushes.Red
                    };
                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                    grid.Children.Add(rect);
                    rectDict[j * height + i] = rect;
                    if (i % 2 == 0) rectDict[j * height + i].Fill = Brushes.Green;
                }
            }
        }
        public void SplitGrid(Grid grid)
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
    }
}
