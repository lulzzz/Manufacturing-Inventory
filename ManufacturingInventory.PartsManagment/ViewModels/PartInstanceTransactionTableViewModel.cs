using Prism.Commands;
using Prism.Mvvm;
using DevExpress.Xpf.Core;
using ManufacturingInventory.Common.Application;
using Prism.Regions;
using DevExpress.Mvvm;
using PrismCommands = Prism.Commands;
using Prism.Events;
using ManufacturingInventory.Common.Model;
using System.Windows;
using ManufacturingInventory.Common.Model.Entities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ManufacturingInventory.PartsManagment.Internal;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Prism;


namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class PartInstanceTransactionTableViewModel : InventoryViewModelBase{
        private IRegionManager _regionManager;

        private ObservableCollection<Transaction> _transaction = new ObservableCollection<Transaction>();
        private Transaction _selectedTransaction;

        public PrismCommands.DelegateCommand ViewDetailsCommand { get; private set; }

        public PartInstanceTransactionTableViewModel(IRegionManager regionManager) {
            this._regionManager = regionManager;
            this.ViewDetailsCommand = new PrismCommands.DelegateCommand(this.ViewTransactionDetailsHandler);
        }

        public override bool KeepAlive => false;

        public ObservableCollection<Transaction> Transactions {
            get => this._transaction;
            set => SetProperty(ref this._transaction, value);
        }
        
        public Transaction SelectedTransaction { 
            get => this._selectedTransaction;
            set => SetProperty(ref this._selectedTransaction, value);
        }

        private void ViewTransactionDetailsHandler() {
            if (this.SelectedTransaction!=null){
                this._regionManager.Regions[Regions.PartInstanceDetailsRegion].RemoveAll();
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add(ParameterKeys.SelectedTransaction, this.SelectedTransaction);
                parameters.Add(ParameterKeys.IsEdit, false);
                this._regionManager.RequestNavigate(Regions.PartInstanceDetailsRegion, AppViews.TransactionDetailsView, parameters);
            }
        }
    }
}
