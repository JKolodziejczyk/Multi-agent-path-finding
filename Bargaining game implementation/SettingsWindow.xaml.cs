﻿using System;
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
using System.Windows.Shapes;

namespace Bargaining_game_implementation
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }


        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to save these settings?",
        "Save", MessageBoxButton.YesNoCancel,
        MessageBoxImage.Question, MessageBoxResult.Cancel) != MessageBoxResult.Yes)
            {
                //e.Cancel = true;
            }
            else
            {
                try
                {
                    Properties.Settings.Default.Width = int.Parse(WidthSet.Text);
                    Properties.Settings.Default.Height = int.Parse(HeightSet.Text);
                    Properties.Settings.Default.Players = int.Parse(PlayersSet.Text);
                    Properties.Settings.Default.FPS = int.Parse(FPSSet.Text);
                    Properties.Settings.Default.Save();
                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Close();

                }
                catch (FormatException)
                {
                    Console.WriteLine("Input string is invalid.");
                    //e.Cancel = true;
                }
            }
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MainWindow main = new MainWindow();
            main.Show();
        }
    }
}