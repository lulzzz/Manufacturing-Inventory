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
    /// Interaction logic for PriceDetailsView.xaml
    /// </summary>
    public partial class PriceDetailsView : UserControl {
        public PriceDetailsView() {
            InitializeComponent();
            //RegionContext.GetObservableContext(this).PropertyChanged += this.PriceDetailsView_PropertyChanged;
        }

        //private void PriceDetailsView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
        //    var context = (ObservableObject<object>)sender;
        //    var instanceContext = (PriceDataTraveler)context.Value;
        //    (DataContext as PriceDetailsViewModel).PriceId = instanceContext.PriceId;
        //    (DataContext as PriceDetailsViewModel).IsEdit = instanceContext.IsEdit;
        //}
    }
}
