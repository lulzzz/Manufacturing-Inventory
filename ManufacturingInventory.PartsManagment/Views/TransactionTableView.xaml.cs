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
using Prism.Common;
using Prism.Regions;
using ManufacturingInventory.PartsManagment.ViewModels;
using System.Collections.ObjectModel;
using ManufacturingInventory.Common.Model.Entities;

namespace ManufacturingInventory.PartsManagment.Views {
    /// <summary>
    /// Interaction logic for PartInstanceTransactionTableView.xaml
    /// </summary>
    public partial class TransactionTableView : UserControl {
        public TransactionTableView() {
            InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.PartTransactionsChanged;
        }

        private void PartTransactionsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            var context = (ObservableObject<object>)sender;
            var transactions = (ObservableCollection<Transaction>)context.Value;
            (DataContext as TransactionTableViewModel).Transactions = transactions;
        }
    }
}
