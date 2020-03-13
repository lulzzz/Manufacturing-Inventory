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
using ManufacturingInventory.Common.Application.UI.Services;
using ManufacturingInventory.Common.Application.UI.ViewModels;
using Prism.Common;
using Prism.Regions;

namespace ManufacturingInventory.Common.Application.UI.Views {
    /// <summary>
    /// Interaction logic for ContactTableDetailView.xaml
    /// </summary>
    public partial class ContactTableDetailView : UserControl {
        public ContactTableDetailView() {
            InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.ContactTableDetailView_PropertyChanged;
        }

        private void ContactTableDetailView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            var context = (ObservableObject<object>)sender;
            var contactContext = (ContactDataTraveler)context.Value;
            (DataContext as ContactTableDetailViewModel).DistributorId = contactContext.DistributorId;
            (DataContext as ContactTableDetailViewModel).CanEdit = contactContext.CanEdit;
        }
    }
}
