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
using System.Runtime.CompilerServices;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class CheckoutViewModel : InventoryViewModelNavigationBase {
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("CheckoutMessageBoxService"); }
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("CheckoutDispatcherService"); }
        protected IExportService ExportService { get => ServiceContainer.GetService<IExportService>("ExportOutgoingService"); }

        private ManufacturingContext _context;
        private IEventAggregator _eventAggregator;

        private string _quantityLabel;

        private ObservableCollection<Transaction> _transactions=new ObservableCollection<Transaction>();
        private ObservableCollection<Consumer> _consumers;
        private Transaction _selectedTransaction;
        private PartInstance _selectedPartInstance;
        private Transaction _newTransaction = new Transaction();
        private Consumer _selectedConsumer;
        private double _measuredWeight;
        private double _weight;
        private double _totalCost;
        private int _quantity;
        private bool _isBubbler = false;
        private bool _isNotBubbler = false;
        private bool _isInitialized = false;

        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand<ExportFormat> ExportOutgoingCommand { get; private set; }
        public AsyncCommand AddToOutgoingCommand { get; private set; }
        public AsyncCommand RemoveFromOutgoingCommand { get; private set; }
        public AsyncCommand CheckOutCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }


        public CheckoutViewModel(ManufacturingContext context,IEventAggregator eventAggregator) {
            this._context = context;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.LoadHandler);
            this.CheckOutCommand = new AsyncCommand(this.CheckOutHandler);
            this.AddToOutgoingCommand = new AsyncCommand(this.AddToOutgoingHandler,this.CanAdd);
            this.RemoveFromOutgoingCommand = new AsyncCommand(this.RemoveFromOutgoingHandler);
            this.CancelCommand = new AsyncCommand(this.CancelHandler);
            this.ExportOutgoingCommand = new AsyncCommand<ExportFormat>(this.ExportOutputHandler);
        }

        public override bool KeepAlive => false;

        public ObservableCollection<Transaction> Transactions { 
            get => this._transactions;
            set => SetProperty(ref this._transactions, value);
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

        public double MeasuredWeight { 
            get => this._measuredWeight;
            set {
                this.SelectedPartInstance.UpdateWeight(value);
                this.Weight = this.SelectedPartInstance.BubblerParameter.Weight;
                this.SetProperty(ref this._measuredWeight, value);
            }
        }

        public double Weight {
            get => this._weight;
            set => SetProperty(ref this._weight, value);
        }

        public double TotalCost { 
            get => this._totalCost;
            set => SetProperty(ref this._totalCost,value);
        }

        public int Quantity { 
            get => this._quantity;
            set {
                if (this.SelectedPartInstance.IsBubbler) {
                    this.TotalCost = this.SelectedPartInstance.UnitCost * value;
                }
                SetProperty(ref this._quantity, value);
            }
        }

        public Consumer SelectedConsumer { 
            get => this._selectedConsumer;
            set => SetProperty(ref this._selectedConsumer, value);
        }

        private async Task AddToOutgoingHandler() {
            await Task.Run(() => {
                if (this.IsBubbler) {
                    this.NewTransaction.TotalCost = this.TotalCost;
                    this.NewTransaction.ParameterValue = this.Weight;
                    this.NewTransaction.IsReturning = true;
                } else {

                }
                this.NewTransaction.Quantity = this.Quantity;
                this.NewTransaction.LocationId = this.SelectedConsumer.Id;
                
                DispatcherService.BeginInvoke(() => {
                    this.Transactions.Add(this.NewTransaction);
                    this.MessageBoxService.ShowMessage("Item added to Output", "Success");
                });
            });
        }

        private async Task RemoveFromOutgoingHandler() {

        }

        private async Task CheckOutHandler() {

        }

        private async Task CancelHandler() {

        }

        private bool CanAdd() {
            return this.SelectedConsumer != null && this.Quantity >= 1;
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
                if (partInstance.Quantity >= 1) {
                    Transaction transaction = new Transaction(partInstance, InventoryAction.OUTGOING);
                    this.SelectedPartInstance = partInstance;
                    this.NewTransaction = transaction;
                    this._isInitialized = false;
                    
                    this.IsBubbler = !partInstance.IsBubbler;
                    this.IsNotBubbler = partInstance.IsBubbler;
                    this.QuantityLabel = (this._isBubbler) ? "Quantity" : "Enter Quantity";
                    if (partInstance.IsBubbler) {
                        this.TotalCost = partInstance.TotalCost;
                        this.Quantity = partInstance.Quantity;
                    }
                } else {
                    this.MessageBoxService.ShowMessage("Error: Item must have a Quantity of 1 or more", "Error", MessageButton.OK, MessageIcon.Error);
                }
            }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) { 

        }
    }
}
