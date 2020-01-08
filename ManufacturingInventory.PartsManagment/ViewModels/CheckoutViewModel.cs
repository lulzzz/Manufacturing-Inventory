using Prism.Commands;
using Prism.Mvvm;
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
using ManufacturingInventory.Common.Application.UI.Services;
using System.IO;
using System.Diagnostics;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class CheckoutViewModel : InventoryViewModelNavigationBase {
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("CheckoutMessageBoxService"); }
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("CheckoutDispatcherService"); }
        protected IExportService ExportService { get => ServiceContainer.GetService<IExportService>("ExportOutgoingService"); }

        private ManufacturingContext _context;
        private IEventAggregator _eventAggregator;

        private string _quantityLabel;

        private ObservableCollection<Transaction> _transaction;
        private ObservableCollection<Consumer> _consumers;
        private Transaction _selectedTransaction;
        private PartInstance _selectedPartInstance;
        private Transaction _newTransaction = new Transaction();
        private bool _isBubbler = false;
        private bool _isNotBubbler = false;
        private bool _isInitialized = false;

        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand<string> ExportOutgoingCommand { get; private set; }
        public AsyncCommand AddToOutgoingCommand { get; private set; }
        public AsyncCommand RemoveFromOutgoingCommand { get; private set; }


        public CheckoutViewModel(ManufacturingContext context,IEventAggregator eventAggregator) {
            this._context = context;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.LoadHandler);
        }

        public override bool KeepAlive => false;

        public ObservableCollection<Transaction> Transaction { 
            get => this._transaction;
            set => SetProperty(ref this._transaction, value);
        }

        public ObservableCollection<Consumer> Consumers {
            get => this._consumers;
            set => SetProperty(ref this._consumers, value);
        }

        public Transaction SelectedTransaction { 
            get => this._selectedTransaction;
            set => SetProperty(ref this._selectedTransaction, value);
        }

        public PartInstance SelectedPartInstance { 
            get => this._selectedPartInstance;
            set => SetProperty(ref this._selectedPartInstance, value);
        }

        public Transaction NewTransaction { 
            get => this._newTransaction;
            set => SetProperty(ref this._newTransaction, value);
        }

        public bool IsBubbler { 
            get => this._isBubbler;
            set => SetProperty(ref this._isBubbler, value);
        }

        public bool IsNotBubbler {
            get => this._isNotBubbler;
            set => SetProperty(ref this._isNotBubbler, value);
        }

        public string QuantityLabel { 
            get => this._quantityLabel;
            set => SetProperty(ref this._quantityLabel, value);
        }

        private async Task LoadHandler() {
            if (!this._isInitialized) {
                var consumers = await this._context.Locations.OfType<Consumer>().ToListAsync();
                this.Consumers = new ObservableCollection<Consumer>(consumers);
                this._isInitialized = true;
            }
        }

        private async Task ExportOutputHandler(ExportFormat format) {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
                    using (FileStream file = File.Create(path)) {
                        this.ExportService.Export(file, format);
                    }
                    Process.Start(path);
                });
            });
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var partInstance = navigationContext.Parameters[ParameterKeys.SelectedPartInstance] as PartInstance;
            if (partInstance is PartInstance) {
                Transaction transaction = new Transaction(partInstance,InventoryAction.OUTGOING);
                this.SelectedPartInstance = partInstance;
                this.NewTransaction = transaction;
                this._isInitialized = false;
                this.IsBubbler = partInstance.IsBubbler;
                this.IsNotBubbler = !partInstance.IsBubbler;
                this.QuantityLabel = (this._isBubbler) ? "Quantity" : "Enter Quantity";
            }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) { 

        }


    }
}
