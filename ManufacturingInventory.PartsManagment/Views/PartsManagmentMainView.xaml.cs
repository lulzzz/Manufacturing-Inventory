using System;
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

namespace ManufacturingInventory.PartsManagment.Views {
    /// <summary>
    /// Interaction logic for PartsManagmentMainView.xaml
    /// </summary>
    public partial class PartsManagmentMainView : UserControl {
        private bool _isactive;
        public string PanelCaption { get { return "Parts Managment"; } }

        public bool IsActive {
            get => this._isactive;
            set => this._isactive = value;
        }

        public PartsManagmentMainView() {
            InitializeComponent();
            this.IsActive = true;
        }
    }
}
