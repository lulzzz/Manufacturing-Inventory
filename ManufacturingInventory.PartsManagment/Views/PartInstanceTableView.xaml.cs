using ManufacturingInventory.PartsManagment.Internal;
using ManufacturingInventory.PartsManagment.ViewModels;
using Prism.Common;
using Prism.Regions;
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
    /// Interaction logic for PartInstanceTableView.xaml
    /// </summary>
    public partial class PartInstanceTableView : UserControl {
        public PartInstanceTableView() {
            InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.PartInstanceTableView_PropertyChanged;
        }

        private void PartInstanceTableView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            var context = (ObservableObject<object>)sender;
            var instanceContext = (DataTraveler)context.Value;
            (DataContext as PartInstanceTableViewModel).SelectedPartId = instanceContext.PartId;
            (DataContext as PartInstanceTableViewModel).IsBubbler = instanceContext.HoldsBubblers;
        }
    }
}
