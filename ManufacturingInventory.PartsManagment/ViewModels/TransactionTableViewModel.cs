using Prism.Commands;
using Prism.Mvvm;
using DevExpress.Xpf.Core;
using ManufacturingInventory.Common.Application;
using Prism.Regions;
using DevExpress.Mvvm;
using PrismCommands = Prism.Commands;
using Prism.Events;
using System.Windows;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ManufacturingInventory.PartsManagment.Internal;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Prism;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Providers;
using ManufacturingInventory.Application.Boundaries.TransactionEdit;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class TransactionTableViewModel : InventoryViewModelBase {

        private IRegionManager _regionManager;
        private ITransactionEditUseCase _transactionEdit;
        private IEventAggregator _eventAggregator;

        protected IDispatcherService Dispatcher { get => ServiceContainer.GetService<IDispatcherService>("TransactionTableDispatcher"); }

        private ObservableCollection<Transaction> _transaction = new ObservableCollection<Transaction>();
        private Transaction _selectedTransaction;
        private int _selectedPartId;
        private bool _showTableLoading;
        private bool _isBubbler;
        private bool _returnInProgress = false;
        private bool _isInitialized = false;

        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand ViewDetailsCOmmand { get; private set; }
        public AsyncCommand UndoTransactionCommand { get; private set; }
        public AsyncCommand ReturnItemCommand { get; private set; }

        public TransactionTableViewModel(ITransactionEditUseCase transactionEdit,IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._regionManager = regionManager;
            this._transactionEdit = transactionEdit;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.InitializeHandler);
            this.ReturnItemCommand = new AsyncCommand(this.ReturnItemHandler,()=>!this._returnInProgress);
            this._eventAggregator.GetEvent<ReturnDoneEvent>().Subscribe(async () => { await this.ReturnDoneHandler();});
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

        public bool IsBubbler {
            get => this._isBubbler;
            set => SetProperty(ref this._isBubbler, value);
        }

        private Task ReturnItemHandler() {
            if (this.SelectedTransaction != null) {
                this._regionManager.Regions[LocalRegions.DetailsRegion].RemoveAll();
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add(ParameterKeys.SelectedTransaction, this.SelectedTransaction);
                //this.Dispatcher.BeginInvoke(() => this._regionManager.RequestNavigate(LocalRegions.DetailsRegion, ModuleViews.ReturnItemView, parameters));
                this._regionManager.RequestNavigate(LocalRegions.DetailsRegion, ModuleViews.ReturnItemView, parameters);
                this._returnInProgress = true;
            }
            return Task.CompletedTask;
        }

        private async Task ReturnDoneHandler() {
            this._returnInProgress = false;
            this._isInitialized = false;
            this.Dispatcher.BeginInvoke(() => this._regionManager.Regions[LocalRegions.DetailsRegion].RemoveAll());
            await this._transactionEdit.Load();
            await this.InitializeHandler();
        }

        private async Task ViewDetailsHandler() {

        }

        private async Task UndoTransactionHandler() {

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
            if (!this._isInitialized) {
                this.Dispatcher.BeginInvoke(() => this.ShowTableLoading = true);
                var transactions = await this._transactionEdit.GetTransactions(GetBy.PART, this.SelectedPartId);
                this.Dispatcher.BeginInvoke(() => {
                    this.Transactions = new ObservableCollection<Transaction>(transactions);
                    this.ShowTableLoading = false;
                });
                this._isInitialized = true;
            }
        }
    }
}
