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

namespace AppBuilder;

/// <summary>
/// Interaction logic for AppPanel.xaml
/// </summary>
public partial class AppPanel : UserControl
{
    public AppPanel(AppNode app)
    {
        InitializeComponent();
        this.DataContext = app;
    }
}
