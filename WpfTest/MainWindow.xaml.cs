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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfTest
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void Button1_Click(object sender, RoutedEventArgs e)
    {
      var c = new Color();
      c.A = 255;
      c.R = 255;
      c.G = 0;
      c.B = 0;
      int hash = c.GetHashCode();
      //var values = c.GetNativeColorValues();
    }

    private void Window_Loaded_1(object sender, RoutedEventArgs e)
    {
      // Button1.Click is an event
    }
  }
}
