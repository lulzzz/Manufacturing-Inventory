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
    public class TransactionTableViewModel : InventoryViewModelBase {

        private IRegionManager _regionManager;
        private ManufacturingContext _context;

        protected IDispatcherService Dispatcher { get => ServiceContainer.GetService<IDispatcherService>("TransactionTableDispatcher"); }

        private ObservableCollection<Transaction> _transaction = new ObservableCollection<Transaction>();
        private Transaction _selectedTransaction;
        private int _selectedPartId;
        private bool _showTableLoading;

        public AsyncCommand InitializeCommand { get; private set; }
        public PrismCommands.DelegateCommand ViewDetailsCommand { get; private set; }

        public TransactionTableViewModel(IRegionManager regionManager,IEventAggregator eventAggregator,ManufacturingContext context) {
            this._regionManager = regionManager;
            this._context = context;
            this.InitializeCommand = new AsyncCommand(this.InitializeHandler);
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

        public int SelectedPartId { 
            get => this._selectedPartId;
            set => SetProperty(ref this._selectedPartId, value);
        }

        public bool ShowTableLoading {
            get => this._showTableLoading;
            set => SetProperty(ref this._showTableLoading, value);
        }

        private void ViewTransactionDetailsHandler() {
            if (this.SelectedTransaction!=null){
                this._regionManager.Regions[LocalRegions.DetailsRegion].RemoveAll();
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add(ParameterKeys.SelectedTransaction, this.SelectedTransaction);
                parameters.Add(ParameterKeys.IsEdit, false);
                this._regionManager.RequestNavigate(Regions.PartInstanceDetailsRegion, ModuleViews.TransactionDetailsView, parameters);
            }
        }

        private async Task InitializeHandler() {
            this.Dispatcher.BeginInvoke(() => this.ShowTableLoading = true);
            var transactions = await this._context.Transactions.Include(e => e.PartInstance).Where(e => e.PartInstance.PartId == this.SelectedPartId).ToListAsync();
            this.Transactions = new ObservableCollection<Transaction>(transactions);
            this.Dispatcher.BeginInvoke(() => this.ShowTableLoading = false);
        }
    }
}
