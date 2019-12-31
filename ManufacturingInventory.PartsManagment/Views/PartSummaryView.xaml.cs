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
    /// Interaction logic for PartSummaryView.xaml
    /// </summary>
    public partial class PartSummaryView : UserControl {
        public PartSummaryView() {
            InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.PartSummaryView_PropertyChanged;
        }

        private void PartSummaryView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            var context = (ObservableObject<object>)sender;
            var summaryContext = (Common.Model.Entities.Part)context.Value;
            (DataContext as PartSummaryViewModel).SelectedPart = summaryContext;
        }
    }
}
