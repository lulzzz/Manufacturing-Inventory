﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ManufacturingInventory.PartsManagment.Views
{
    /// <summary>
    /// Interaction logic for PartsNavigationView.xaml
    /// </summary>
    public partial class PartsNavigationView : UserControl
    {
        public string PanelCaption { get { return "Parts"; } }
        public PartsNavigationView()
        {
            InitializeComponent();
        }
    }
}
