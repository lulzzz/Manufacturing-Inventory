using Prism.Commands;
using Prism.Mvvm;
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
using ManufacturingInventory.Application.Boundaries.CheckIn;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Common.Application.UI.ViewModels;
using Condition = ManufacturingInventory.Infrastructure.Model.Entities.Condition;
using System.Collections.Generic;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class CheckInViewModel : InventoryViewModelNavigationBase {

        public IDialogService SelectPriceDialog { get { return ServiceContainer.GetService<IDialogService>("SelectPriceDialog"); } }
        public IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("CheckInDispatcherService"); } }
        public IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("CheckInMessageBoxService"); }

        private ICheckInUseCase _checkIn;
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;

        private bool _isInitialized=false;

        private SelectPricePopupViewModel _selectPricePopupViewModel;
        private int _partId;
        private bool _isBubbler;
        private bool _newPriceGroupCollapsed;
        private bool _createTransaction;
        private bool _createPrice;
        private PartInstance _selectedPartInstance=new PartInstance();
        private Price _price;
        private BubblerParameter _bubblerParameter;
        private ObservableCollection<Condition> _conditions;
        private ObservableCollection<Warehouse> _warehouses;
        private ObservableCollection<PartType> _partTypes;
        private ObservableCollection<Distributor> _distributors;
        private IEnumerable<Price> _prices;

        private Condition _selectedCondition;
        private PartType _selectedPartType;
        private Warehouse _selectedWarehouse;
        private Distributor _selectedDistributor;
        private double _netWeight;
        private double _grossWeight;
        private double _measuredWeight;
        private double _weight;

        public AsyncCommand CheckInCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }
        
        public AsyncCommand SelectExistingPriceCommand { get; private set; }

        public AsyncCommand NewPriceCollapsedCommand { get; private set;}
        public AsyncCommand NewPriceExpandedCommand { get; private set; }

        public AsyncCommand PriceSelectorCollapsedCommand { get; private set; }
        public AsyncCommand PriceSelectorExpandedCommand { get; private set; }

        public CheckInViewModel(ICheckInUseCase checkIn,IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._checkIn = checkIn;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.Load);
            this.SelectExistingPriceCommand = new AsyncCommand(this.SelectExistingPriceHandler);
        }

        #region VariableBindings

        public override bool KeepAlive => false;

        public PartInstance SelectedPartInstance {
            get => this._selectedPartInstance;
            set => SetProperty(ref this._selectedPartInstance, value);
        }
        public Price Price {
            get => this._price;
            set => SetProperty(ref this._price, value);
        }

        public Condition SelectedCondition {
            get => this._selectedCondition;
            set => SetProperty(ref this._selectedCondition, value);
        }

        public PartType SelectedPartType {
            get => this._selectedPartType;
            set => SetProperty(ref this._selectedPartType, value);
        }

        public Warehouse SelectedWarehouse {
            get => this._selectedWarehouse;
            set => SetProperty(ref this._selectedWarehouse, value);
        }

        public Distributor SelectedDistributor {
            get => this._selectedDistributor;
            set => SetProperty(ref this._selectedDistributor, value);
        }

        public double Weight {
            get => this._weight;
            set => this.SetProperty(ref this._weight, value);
        }

        public double Measured {
            get => this._measuredWeight;
            set {
                if (this.IsBubbler) {
                    this.SelectedPartInstance.UpdateWeight(value);
                    this.Weight = this.SelectedPartInstance.BubblerParameter.Weight;
                }
                this.SetProperty(ref this._measuredWeight, value);
            }
        }

        public double GrossWeight {
            get => this._grossWeight;
            set {
                if (this.IsBubbler) {
                    this.SelectedPartInstance.BubblerParameter.GrossWeight = value;
                    this.SelectedPartInstance.UpdateWeight();
                    this.Weight = this.SelectedPartInstance.BubblerParameter.Weight;
                }
                SetProperty(ref this._grossWeight, value);
            }
        }

        public double NetWeight {
            get => this._netWeight;
            set {
                if (this.IsBubbler) {
                    this.SelectedPartInstance.BubblerParameter.NetWeight = value;
                    this.SelectedPartInstance.UpdateWeight();
                    this.Weight = this.SelectedPartInstance.BubblerParameter.Weight;
                }
                SetProperty(ref this._netWeight, value);
            }
        }

        public bool IsBubbler {
            get => this._isBubbler;
            set => SetProperty(ref this._isBubbler, value);
        }

        public ObservableCollection<Condition> Conditions {
            get => this._conditions;
            set => SetProperty(ref this._conditions, value);
        }

        public ObservableCollection<Warehouse> Warehouses {
            get => this._warehouses;
            set => SetProperty(ref this._warehouses, value);
        }

        public ObservableCollection<PartType> PartTypes {
            get => this._partTypes;
            set => SetProperty(ref this._partTypes, value);
        }

        public ObservableCollection<Distributor> Distributors {
            get => this._distributors;
            set => SetProperty(ref this._distributors, value);
        }

        public bool NewPriceGroupCollapsed { 
            get => this._newPriceGroupCollapsed;
            set => SetProperty(ref this._newPriceGroupCollapsed, value);
        }

        public bool CreateTransaction { 
            get => this._createTransaction;
            set => SetProperty(ref this._createTransaction, value);
        }

        public bool CreatePrice {
            get => this._createPrice;
            set => SetProperty(ref this._createPrice, value);
        }

        #endregion

        private Task CheckInHandler() {
            //CheckInInput input=new CheckInInput(this.SelectedPartInstance,)
            return Task.CompletedTask;
        }

        private Task CancelHandler() {
            return Task.Factory.StartNew(() => this._eventAggregator.GetEvent<CheckInCancelEvent>().Publish());
        }

        private Task SelectExistingPriceHandler() {
            return Task.Factory.StartNew(() => {
                this.DispatcherService.BeginInvoke(() => {
                    if (this.ShowSelectPriceDialog()) {
                        this.Price = this._selectPricePopupViewModel.SelectedPrice;
                        this.NewPriceGroupCollapsed = false;
                    }
                });
            });
        }

        private Task NewPriceCollapsedHandler() {
            return Task.CompletedTask;
        }

        private Task NewPriceExpandedHandler() {
            return Task.CompletedTask;
        }

        private Task PriceSelectorCollapsedHandler() {
            return Task.CompletedTask;
        }

        private Task PriceSelectorExpandedHandler() {
            return Task.CompletedTask;
        }


        private bool ShowSelectPriceDialog() {
            if (this._selectPricePopupViewModel == null) {
                this._selectPricePopupViewModel = new SelectPricePopupViewModel(this._prices);
            }

            UICommand continueCommand = new UICommand() {
                Caption = "Continue With Selection",
                IsCancel = false,
                IsDefault = true,
            };

            UICommand cancelCommand = new UICommand() {
                Id = MessageBoxResult.Cancel,
                Caption = "Cancel",
                IsCancel = true,
                IsDefault = false,
            };
            UICommand result=SelectPriceDialog.ShowDialog(
                dialogCommands: new List<UICommand>() { continueCommand, cancelCommand },
                title: "Select Price Dialog",viewModel: this._selectPricePopupViewModel);
            return result == continueCommand;
        }

        private async Task Load() {
            if (!this._isInitialized) {
                this._prices = await this._checkIn.GetAvailablePrices(this._partId);
                var distributors = await this._checkIn.GetDistributors();
                var categories = await this._checkIn.GetCategories();
                var warehouses = await this._checkIn.GetWarehouses();

                this.DispatcherService.BeginInvoke(() => {
                    this.SelectedPartInstance = new PartInstance();
                    this.SelectedPartInstance.BubblerParameter = new BubblerParameter();

                    this.Distributors = new ObservableCollection<Distributor>(distributors);
                    this.Conditions = new ObservableCollection<Condition>(categories.OfType<Condition>());
                    this.PartTypes = new ObservableCollection<PartType>(categories.OfType<PartType>());
                    this.Warehouses = new ObservableCollection<Warehouse>(warehouses);
                    this.SelectedPartInstance = new PartInstance();
                    this.Price = new Price();
                    this._bubblerParameter = new BubblerParameter();
                    this.NewPriceGroupCollapsed = true;
                });
                this._isInitialized = false;
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            this._partId =Convert.ToInt32(navigationContext.Parameters[ParameterKeys.PartId]);
            this.IsBubbler = Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsBubbler]);
            this._isInitialized = false;
            
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }

    }
}
