using Prism.Commands;
using Prism.Mvvm;
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
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Common.Application;
using Condition = ManufacturingInventory.Infrastructure.Model.Entities.Condition;
using ManufacturingInventory.Application.Boundaries.PartInstanceDetailsEdit;
using ManufacturingInventory.Application.Boundaries.ReturnItem;
using ManufacturingInventory.Common.Application.UI.Services;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class ReturnItemViewModel : InventoryViewModelNavigationBase {

        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("ReturnItemMessageBoxService"); }
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("ReturnItemDispatcherService"); }

        private IReturnItemUseCase _returnItem;
        private IEventAggregator _eventAggregator;

        private ObservableCollection<Warehouse> _warehouses;
        private ObservableCollection<Condition> _conditions;
        private Condition _selectedCondition;
        private Warehouse _selectedWarehouse;

        private Transaction _selectedTransaction;
        private PartInstance _transactionPartInstance;

        private DateTime _timeStamp;
        private double _measuredWeight;
        private double _weight;
        private double _instanceNet;
        private double _instanceGross;
        private int _quantity;
        private bool _isBubbler = false;
        private bool _isInitialized = false;

        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand ReturnItemCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }

        public ReturnItemViewModel(IReturnItemUseCase returnItem,IEventAggregator eventAggregator) {
            this._returnItem = returnItem;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.LoadHandler);
            this.ReturnItemCommand = new AsyncCommand(this.ReturnItemHandler);
            this.CancelCommand = new AsyncCommand(this.CancelHandler);
        }

        public override bool KeepAlive => false;

        public ObservableCollection<Warehouse> Warehouses { 
            get => this._warehouses;
            set => SetProperty(ref this._warehouses, value);
        }

        public ObservableCollection<Condition> Conditions { 
            get => this._conditions;
            set => SetProperty(ref this._conditions, value);
        }

        public Condition SelectedCondition { 
            get => this._selectedCondition;
            set => SetProperty(ref this._selectedCondition, value);
        }

        public Warehouse SelectedWarehouse { 
            get => this._selectedWarehouse;
            set => SetProperty(ref this._selectedWarehouse, value);
        }

        public DateTime TimeStamp { 
            get => this._timeStamp;
            set => SetProperty(ref this._timeStamp, value);
        }

        public double MeasuredWeight {
            get => this._measuredWeight;
            set {
                this.Weight = this._instanceNet - (this._instanceGross - value);
                this.SetProperty(ref this._measuredWeight, value);
            }
        }

        public double Weight { 
            get => this._weight;
            set => SetProperty(ref this._weight, value);
        }

        public int Quantity { 
            get => this._quantity;
            set => SetProperty(ref this._quantity, value);
        }

        public bool IsBubbler { 
            get => this._isBubbler;
            set => SetProperty(ref this._isBubbler, value);
        }

        public bool IsInitialized { 
            get => this._isInitialized;
            set => SetProperty(ref this._isInitialized, value);
        }

        public Transaction SelectedTransaction { 
            get => this._selectedTransaction;
            set => SetProperty(ref this._selectedTransaction, value);
        }

        public PartInstance TransactionPartInstance { 
            get => this._transactionPartInstance;
            set => SetProperty(ref this._transactionPartInstance, value);
        }

        private async Task ReturnItemHandler() {
            int conditionId = (this.SelectedCondition != null) ? this.SelectedCondition.Id : 0;

            ReturnItemInput input = new ReturnItemInput(this.TimeStamp, this.SelectedTransaction.Quantity, this.IsBubbler, 
                this.SelectedTransaction.PartInstanceId, this.SelectedWarehouse.Id, this.SelectedTransaction.Id, 
                this.Weight, this.MeasuredWeight, conditionId);

            var response = await this._returnItem.Execute(input);

            if (response.Success) {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Success", MessageButton.OK, MessageIcon.Information);
                });
                this._eventAggregator.GetEvent<ReturnDoneEvent>().Publish();
            } else {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage("Error Processing Return, Please Check Input and Try Again", "Error", MessageButton.OK, MessageIcon.Error);
                });
            }
        }

        private async Task CancelHandler() {
            await Task.Run(() => {
                this._eventAggregator.GetEvent<ReturnDoneEvent>().Publish();
            });
        }
        
        private async Task LoadHandler() {
            if (!this._isInitialized) {
                var warehouses = await this._returnItem.GetWarehouses();
                var conditions = await this._returnItem.GetConditions();
                var bubblerValues =await  this._returnItem.GetInstanceNetGross(this.SelectedTransaction.PartInstanceId);
                this._instanceNet = bubblerValues.Item1;
                this._instanceGross = bubblerValues.Item2;
                var wareHouseId = await this._returnItem.GetPartWarehouseId(this.SelectedTransaction.PartInstance.PartId);
                this.DispatcherService.BeginInvoke(() => {
                    this.Warehouses = new ObservableCollection<Warehouse>(warehouses);
                    this.Conditions = new ObservableCollection<Condition>(conditions);
                    this.Quantity = this.SelectedTransaction.Quantity;
                    this.IsBubbler = this.SelectedTransaction.PartInstance.IsBubbler;
                    this.TimeStamp = DateTime.Now;
                    this.SelectedWarehouse = this.Warehouses.FirstOrDefault(e => e.Id == wareHouseId);
                    this._isInitialized = true;
                });
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var transaction = navigationContext.Parameters[ParameterKeys.SelectedTransaction] as Transaction;
            if(transaction is Transaction) {
                this.SelectedTransaction = transaction;
            }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }


    }
}
