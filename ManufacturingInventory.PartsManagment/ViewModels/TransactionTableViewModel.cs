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
using ManufacturingInventory.Infrastructure.Model.Services;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class TransactionTableViewModel : InventoryViewModelBase {

        private IRegionManager _regionManager;
        //private ManufacturingContext _context;
        IEntityProvider<Transaction> _provider;
        private IPartManagerService _partManager;

        protected IDispatcherService Dispatcher { get => ServiceContainer.GetService<IDispatcherService>("TransactionTableDispatcher"); }

        private ObservableCollection<Transaction> _transaction = new ObservableCollection<Transaction>();
        private Transaction _selectedTransaction;
        private int _selectedPartId;
        private bool _showTableLoading;
        private bool _isBubbler;
        private bool _isNotBubbler;



        public AsyncCommand InitializeCommand { get; private set; }
        public PrismCommands.DelegateCommand ViewDetailsCommand { get; private set; }

        public TransactionTableViewModel(IEntityProvider<Transaction> provider,IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._regionManager = regionManager;
            this._provider = provider;
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

        public bool IsBubbler {
            get => this._isBubbler;
            set => SetProperty(ref this._isBubbler, value);
        }

        public bool IsNotBubbler {
            get => this._isNotBubbler;
            set => SetProperty(ref this._isNotBubbler, value);
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
            //var part = await this._context.Parts.AsNoTracking().FirstOrDefaultAsync(e => e.Id == this.SelectedPartId);
            var part = await this._partManager.PartService.GetEntityAsync(e => e.Id == this.SelectedPartId,false);
            var transactions = await this._partManager.TransactionService.GetEntityListAsync(e => e.PartInstance.PartId == this.SelectedPartId);
            //var transactions = await this._context.Transactions.AsNoTracking()
            //    .Include(e => e.PartInstance)
            //    .Include(e=>e.Location)
            //    .Where(e => e.PartInstance.PartId == this.SelectedPartId).ToListAsync();

            this.Dispatcher.BeginInvoke(() => {
                this.Transactions = new ObservableCollection<Transaction>(transactions);
                this.IsBubbler = part.HoldsBubblers;
                this.IsNotBubbler = !this.IsBubbler;
                this.ShowTableLoading = false;
            });
        }
    }
}
