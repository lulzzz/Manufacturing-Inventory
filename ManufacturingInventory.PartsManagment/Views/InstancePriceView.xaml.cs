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
using ManufacturingInventory.PartsManagment.Internal;
using ManufacturingInventory.PartsManagment.ViewModels;
using Prism.Common;
using Prism.Regions;

namespace ManufacturingInventory.PartsManagment.Views {
    /// <summary>
    /// Interaction logic for InstancePriceView.xaml
    /// </summary>
    public partial class InstancePriceView : UserControl {
        public InstancePriceView() {
            InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.InstancePriceView_PropertyChanged;
        }

        private void InstancePriceView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            var context = (ObservableObject<object>)sender;
            var priceContext = (PriceDataTraveler)context.Value;
            (DataContext as InstancePriceViewModel).PriceId = priceContext.PriceId;
            (DataContext as InstancePriceViewModel).InstanceId = priceContext.InstanceId;
            (DataContext as InstancePriceViewModel).PartId = priceContext.PartId;
            (DataContext as InstancePriceViewModel).IsNew = priceContext.IsNew;
        }
    }
}
