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

namespace ManufacturingInventory.CategoryManagment.Views {
    /// <summary>
    /// Interaction logic for CategoryMainView.xaml
    /// </summary>
    public partial class CategoryMainView : UserControl {
        private bool _isactive;
        public string PanelCaption { get { return "Category Managment"; } }

        public bool IsActive {
            get => this._isactive;
            set => this._isactive = value;
        }

        public CategoryMainView() {
            InitializeComponent();
            this.IsActive = true;
        }
    }
}
