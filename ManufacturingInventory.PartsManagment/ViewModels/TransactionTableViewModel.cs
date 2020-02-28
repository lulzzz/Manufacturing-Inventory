using DevExpress.Mvvm;
using ManufacturingInventory.Application.Boundaries.TransactionEdit;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Application.UI.Services;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.PartsManagment.Internal;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class TransactionTableViewModel : InventoryViewModelBase {

        private IRegionManager _regionManager;
        private ITransactionEditUseCase _transactionEdit;
        private IEventAggregator _eventAggregator;

        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("TransactionTableDispatcher"); }
        protected IExportService ExportService { get => ServiceContainer.GetService<IExportService>("TransactionTableExportService"); }

        private ObservableCollection<Transaction> _transaction = new ObservableCollection<Transaction>();
        private Transaction _selectedTransaction;
        private int _selectedPartId;
        private bool _showTableLoading;
        private bool _isBubbler;
        private bool _returnInProgress = false;
        private bool _isInitialized = false;

        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand UndoTransactionCommand { get; private set; }
        public AsyncCommand ReturnItemCommand { get; private set; }
        public AsyncCommand<ExportFormat> ExportTableCommand { get; private set; }

        public TransactionTableViewModel(ITransactionEditUseCase transactionEdit,IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._regionManager = regionManager;
            this._transactionEdit = transactionEdit;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.InitializeHandler);
            this.ReturnItemCommand = new AsyncCommand(this.ReturnItemHandler,()=>!this._returnInProgress);
            this.ExportTableCommand = new AsyncCommand<ExportFormat>(this.ExportTableHandler);
            this._eventAggregator.GetEvent<ReturnDoneEvent>().Subscribe(async (instanceId) => { await this.ReturnDoneHandler(instanceId);});
            this._eventAggregator.GetEvent<ReturnCancelEvent>().Subscribe(async () => await this.ReturnCancelHandler());
            this._eventAggregator.GetEvent<OutgoingDoneEvent>().Subscribe(async (instanceId) => { await this.ReloadHandler(); });
            this._eventAggregator.GetEvent<CheckInDoneEvent>().Subscribe(async (instanceId) =>{ await this.ReloadHandler(); });
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

        private async Task ReturnItemHandler() {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    if (this.SelectedTransaction != null) {
                        this._regionManager.Regions[LocalRegions.DetailsRegion].RemoveAll();
                        NavigationParameters parameters = new NavigationParameters();
                        parameters.Add(ParameterKeys.SelectedTransaction, this.SelectedTransaction);
                        this._regionManager.RequestNavigate(LocalRegions.DetailsRegion, ModuleViews.ReturnItemView, parameters);
                    }
                });
            });
            this._returnInProgress = true;
        }

        private void ViewInstanceHandler(int instanceId) {
            this.CleanupRegions();
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add(ParameterKeys.InstanceId, instanceId);
            parameters.Add(ParameterKeys.IsEdit, false);
            parameters.Add(ParameterKeys.IsBubbler, this.IsBubbler);
            parameters.Add(ParameterKeys.PartId, this.SelectedPartId);
            this._regionManager.RequestNavigate(LocalRegions.DetailsRegion, ModuleViews.PartInstanceDetailsView, parameters);
        }

        private async Task ReturnDoneHandler(int instanceId) {
            this._returnInProgress = false;
            await this.ReloadWithInstanceHandler(true,instanceId);
        }

        private Task UndoTransactionHandler() {
            return Task.CompletedTask;
        }

        private async Task ExportTableHandler(ExportFormat format) {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
                    using (FileStream file = File.Create(path)) {
                        this.ExportService.Export(file, format);
                    }
                    using (var process = new Process()) {
                        process.StartInfo.UseShellExecute = true;
                        process.StartInfo.FileName = path;
                        process.StartInfo.CreateNoWindow = true;
                        process.Start();
                    }
                });
            });
        }

        private async Task ReturnCancelHandler() {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            await this._transactionEdit.Load();
            var transactions = await this._transactionEdit.GetTransactions(GetBy.PART, this.SelectedPartId);
            this.DispatcherService.BeginInvoke(() => {
                this.Transactions = new ObservableCollection<Transaction>(transactions);
                this.CleanupRegions();
                this.ShowTableLoading = false;
            });
        }

        private async Task ReloadHandler() {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            await this._transactionEdit.Load();
            var transactions = await this._transactionEdit.GetTransactions(GetBy.PART, this.SelectedPartId);
            this.DispatcherService.BeginInvoke(() => {
                this.Transactions = new ObservableCollection<Transaction>(transactions);
                this.ShowTableLoading = false;
            });
        }

        public async Task ReloadWithInstanceHandler(bool sendToInstanceTable,int instanceId) {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            await this._transactionEdit.Load();
            var transactions = await this._transactionEdit.GetTransactions(GetBy.PART, this.SelectedPartId);
            this.DispatcherService.BeginInvoke(() => {
                this.Transactions = new ObservableCollection<Transaction>(transactions);
                this.ViewInstanceHandler(instanceId);
                this.ShowTableLoading = false;
            });
        }

        private async Task InitializeHandler() {
            if (!this._isInitialized) {
                this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
                var transactions = await this._transactionEdit.GetTransactions(GetBy.PART, this.SelectedPartId);
                this.DispatcherService.BeginInvoke(() => {
                    this.Transactions = new ObservableCollection<Transaction>(transactions);
                    this.ShowTableLoading = false;
                });
                this._isInitialized = true;
            }
        }

        public void CleanupRegions() {
            this._regionManager.Regions[LocalRegions.DetailsRegion].RemoveAll();
        }
    }
}
