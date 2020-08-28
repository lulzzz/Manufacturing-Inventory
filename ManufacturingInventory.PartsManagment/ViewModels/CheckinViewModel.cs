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
using System.Text;
using System.ComponentModel;
using BindableBase = DevExpress.Mvvm.BindableBase;
using ManufacturingInventory.Common.Application.ValidationRules;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class CheckInViewModel : InventoryViewModelNavigationBase,IDataErrorInfo {

        public IDialogService SelectPriceDialog { get { return ServiceContainer.GetService<IDialogService>("SelectPriceDialog"); } }
        public IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("CheckInDispatcherService"); } }
        public IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("CheckInMessageBoxService"); }

        private ICheckInUseCase _checkIn;
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;

        private bool _isInitialized=false;

        private SelectPricePopupViewModel _selectPricePopupViewModel;
        private int _partId;
        private int? _instanceId;
        private bool _isBubbler;
        private bool _isExisting;
        private bool _createPrice;
        private bool _canChangeQuantity;
        private bool _isReusable;
        private bool _notNoPriceOption;
        private bool _canEditStock;
        private bool _costReported;

        private PartInstance _selectedPartInstance=new PartInstance();
        private Price _price;
        private ObservableCollection<Condition> _conditions;
        private ObservableCollection<Warehouse> _warehouses;
        private ObservableCollection<StockType> _partTypes;
        private ObservableCollection<Usage> _usageList;
        private ObservableCollection<Distributor> _distributors;
        private IEnumerable<Price> _prices;

        private Condition _selectedCondition;
        private StockType _selectedPartType;
        private Warehouse _selectedWarehouse;
        private Distributor _selectedDistributor;
        private Usage _selectedUsage;
        private string _name;
        private double _netWeight;
        private double _grossWeight;
        private double _measuredWeight;
        private double _weight;
        private int _quantity;
        private double _totalCost;
        private double _unitCost;
        private DateTime _transactionTimeStamp;
        private PriceOption _selectedPriceOption=PriceOption.NoPrice;
        private bool returningFromOption = false;

        public AsyncCommand CheckInCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand<PriceOption> PriceOptionSelectionChangedCommand { get; private set; }
        public AsyncCommand StockTypeSelectionChanged { get; private set; }
        //public AsyncCommand ReusableCheckedCommand { get; private set; }
        //public AsyncCommand ReusableUnCheckedCommand { get; private set; }

        public CheckInViewModel(ICheckInUseCase checkIn,IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._checkIn = checkIn;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.Load);
            this.CheckInCommand = new AsyncCommand(this.CheckInHandler,this.CanExecuteCheckIn);
            this.CancelCommand = new AsyncCommand(this.CancelHandler);
            this.PriceOptionSelectionChangedCommand = new AsyncCommand<PriceOption>(this.PriceOptionSelectionChangedHandler);
            this.StockTypeSelectionChanged = new AsyncCommand(this.StockTypeSelectionChangedHandler);
            //this.ReusableCheckedCommand = new AsyncCommand(this.ReusableUnCheckedHandler);
            //this.ReusableUnCheckedCommand = new AsyncCommand(this.ReusableUnCheckedHandler);
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

        public StockType SelectedStockType {
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

        public int Quantity {
            get => this._quantity;
            set {
                this.TotalCost = this.UnitCost * value; 
                SetProperty(ref this._quantity, value);
            }
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

        public ObservableCollection<Condition> Conditions {
            get => this._conditions;
            set => SetProperty(ref this._conditions, value);
        }

        public ObservableCollection<Warehouse> Warehouses {
            get => this._warehouses;
            set => SetProperty(ref this._warehouses, value);
        }

        //public ObservableCollection<StockType> StockTypes {
        //    get => this._partTypes;
        //    set => SetProperty(ref this._partTypes, value);
        //}

        public ObservableCollection<Distributor> Distributors {
            get => this._distributors;
            set => SetProperty(ref this._distributors, value);
        }

        public bool CreateNewPrice {
            get => this._createPrice;
            set => SetProperty(ref this._createPrice, value);
        }

        public DateTime TransactionTimeStamp {
            get => this._transactionTimeStamp;
            set => SetProperty(ref this._transactionTimeStamp, value);
        }

        public PriceOption SelectedPriceOption {
            get => this._selectedPriceOption;
            set => SetProperty(ref this._selectedPriceOption, value);
        }

        public ObservableCollection<Usage> UsageList {
            get => this._usageList;
            set => SetProperty(ref this._usageList, value);
        }

        public Usage SelectedUsage {
            get => this._selectedUsage;
            set => SetProperty(ref this._selectedUsage, value);
        }
       
        public double UnitCost {
            get => this._unitCost;
            set => SetProperty(ref this._unitCost, value);
        }

        public double TotalCost {
            get => this._totalCost;
            set => SetProperty(ref this._totalCost, value);
        }

        public string Name { 
            get => this._name;
            set {
                this.SelectedPartInstance.Name = value;
                SetProperty(ref this._name, value);
            } 
        }

        public bool CostReported { 
            get => this._costReported;
            set {
                this.SelectedPartInstance.CostReported = value;
                SetProperty(ref this._costReported, value);
            }
        }

        public bool CanChangeQuantity { 
            get => this._canChangeQuantity;
            set=>SetProperty(ref this._canChangeQuantity, value);
        }

        public bool NotNoPriceOption {
            get => this._notNoPriceOption;
            set => SetProperty(ref this._notNoPriceOption, value);
        }

        public bool CanEditStock {
            get => this._canEditStock;
            set => SetProperty(ref this._canEditStock, value);
        }

        public bool IsExisting {
            get => this._isExisting;
            set => SetProperty(ref this._isExisting, value);
        }

        public bool IsReusable {
            get => this._isReusable;
            set {
                this.CanChangeQuantity = !value;
                this.Quantity = (value) ? 1 : 0;
                SetProperty(ref this._isReusable, value);
            }
        }

        public bool IsBubbler {
            get => this._isBubbler;
            set => SetProperty(ref this._isBubbler, value);
        }

        #endregion

        #region ValidationRegion

        public string Error {
            get {
                //if (!this._validationEnabled) return null;
                IDataErrorInfo me = (IDataErrorInfo)this;
                string error = me[BindableBase.GetPropertyName(() => this.Name)] +
                              me[BindableBase.GetPropertyName(() => this.Quantity)] +
                              me[BindableBase.GetPropertyName(() => this.NetWeight)] +
                              me[BindableBase.GetPropertyName(() => this.GrossWeight)] +
                              me[BindableBase.GetPropertyName(() => this.Measured)] +
                              me[BindableBase.GetPropertyName(() => this.SelectedPriceOption)];
                if (!string.IsNullOrEmpty(error))
                    return "Please check inputted data";
                return null;
            }
        }

        public string this[string columnName] {
            get {
                //if(!this._validationEnabled) return null;
                string nameProp = BindableBase.GetPropertyName(() => this.Name);
                string warehouseProp = BindableBase.GetPropertyName(() => this.SelectedWarehouse);
                string quantityProp = BindableBase.GetPropertyName(() => this.Quantity);
                string netProp = BindableBase.GetPropertyName(() => this.NetWeight);
                string grossProp = BindableBase.GetPropertyName(() => this.GrossWeight);
                string measuredProp = BindableBase.GetPropertyName(() => this.Measured);
                if (columnName == nameProp) {
                    return RequiredValidationRule.GetErrorMessage(nameProp, this.Name);
                } else if (columnName == warehouseProp) {
                    if (this.SelectedWarehouse == null)
                        return "Warehouse must be selected";
                } else if (columnName == quantityProp) {
                    if (this.Quantity == 0 && !this.IsBubbler)
                        return "Quantity must be >0";
                } else if (columnName == netProp) {
                    if (this.IsBubbler && this.NetWeight == 0)
                        return "Net must be >0";
                } else if (columnName == grossProp) {
                    if (this.IsBubbler && this.GrossWeight == 0)
                        return "Gross must be >0";
                } else if (columnName == measuredProp) {
                    if (this.IsBubbler && this.Measured == 0)
                        return "Measured must be >0";
                }
                return null;
            }
        }

        private void OnValidationFailed(string error) {
            this.MessageBoxService.ShowMessage("Check In Failed. "+Environment.NewLine+ error, "Error",MessageButton.OK,MessageIcon.Error);
        }

        private string EnableValidationAndGetError() {
            //this._validationEnabled = true;
            string error = ((IDataErrorInfo)this).Error;
            if (!string.IsNullOrEmpty(error)) {
                this.RaisePropertyChanged();
                return error;
            }
            return null;
        }

        #endregion

        #region ToggleHandlers

        //private async Task ReusableCheckedHandler() {
        //    await Task.Run(() => {
        //        this.CanChangeQuantity = false;
        //        this.Quantity = 1;
        //    });

        //}

        //private async Task ReusableUnCheckedHandler() {
        //    await Task.Run(() => {
        //        this.CanChangeQuantity = true;
        //        this.Quantity = 0;
        //    });

        //}

        #endregion

        #region ComboEditHandlers

        private async Task StockTypeSelectionChangedHandler() {
            if (this.SelectedStockType != null && !this.SelectedStockType.IsDefault) {
                await Task.Run(() =>{
                    this.CanEditStock = false;
                    this.SelectedPartInstance.MinQuantity = this.SelectedStockType.MinQuantity;
                    this.SelectedPartInstance.SafeQuantity = this.SelectedStockType.SafeQuantity;
                    this.DispatcherService.BeginInvoke(() => {
                        RaisePropertyChanged("SelectedPartInstance");
                    });
                });
            }
        }

        private async Task CreatePriceHandler(PriceOption previousOption) {
            await Task.Run(() => {
                    this.DispatcherService.BeginInvoke(() => {
                        switch (previousOption) {
                            case PriceOption.CreateNew: {
                                this.Price = new Price();
                                this.SelectedDistributor = null;
                                this.NotNoPriceOption = true;
                                break;
                            }
                            case PriceOption.UseExisting: {
                                if (!this.returningFromOption) {
                                    StringBuilder builder = new StringBuilder();
                                    builder.AppendLine("Warning");
                                    builder.AppendLine("Do you want to continue with previous price?").AppendLine();
                                    builder.AppendLine("Select Yes to edit previous price and save as new");
                                    builder.AppendLine("Select No to start with blank price");
                                    builder.AppendLine("Select Cancel To revert back").AppendLine();
                                    var response = this.MessageBoxService.ShowMessage(builder.ToString(), "Warning", MessageButton.YesNoCancel, MessageIcon.Warning);
                                    switch (response) {
                                        case MessageResult.Cancel:
                                            this.SelectedPriceOption = previousOption;
                                            this.NotNoPriceOption = (previousOption != PriceOption.NoPrice);
                                            this.returningFromOption = true;
                                            break;
                                        case MessageResult.Yes:
                                            var price = new Price();
                                            price.Set(this.Price);
                                            this.Price = price;
                                            this.NotNoPriceOption = true;
                                            this.returningFromOption = false;
                                            break;
                                        case MessageResult.No:
                                            this.Price = new Price();
                                            this.SelectedDistributor = null;
                                            this.NotNoPriceOption = true;
                                            this.returningFromOption = false;
                                            break;
                                        default:
                                            this.Price = null;
                                            this.NotNoPriceOption = false;
                                            this.SelectedPriceOption = PriceOption.NoPrice;
                                            this.returningFromOption = false;
                                            break;
                                    }
                                } else {
                                    this.returningFromOption = false;
                                }

                                break;
                            }

                            case PriceOption.NoPrice:
                                this.Price = new Price();
                                this.SelectedDistributor = null;
                                this.NotNoPriceOption = true;
                                this.returningFromOption = false;
                                break;
                        }
                    });
            });
        }

        private async Task NoPriceSelectedHandler(PriceOption previousOption) {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    switch (previousOption) {
                        case PriceOption.CreateNew:
                        case PriceOption.UseExisting: {
                            if (!this.returningFromOption) {
                                StringBuilder builder = new StringBuilder();
                                builder.AppendLine("Warning");
                                builder.AppendLine("This will clear pricing info");
                                builder.AppendLine("Continue?");
                                var response = this.MessageBoxService.ShowMessage(builder.ToString(), "Warning", MessageButton.YesNo, MessageIcon.Warning);
                                if (response == MessageResult.Yes) {
                                    this.Price = null;
                                    this.NotNoPriceOption = false;
                                    this.returningFromOption = false;
                                } else {
                                    this.SelectedPriceOption = previousOption;
                                    this.returningFromOption = true;
                                }
                            } else {
                                this.Price = null;
                                this.NotNoPriceOption = false;
                                this.returningFromOption = false;
                            }
                            break;
                        }
                        case PriceOption.NoPrice:
                            break;
                    }
                });
            });
        }

        private async Task SelectExistingPriceHandler(PriceOption previousOption) {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    switch (previousOption) {
                        case PriceOption.CreateNew: {
                            if (!this.returningFromOption) {
                                StringBuilder builder = new StringBuilder();
                                builder.AppendLine("Warning");
                                builder.AppendLine("This Will Replace Pricing Info With Selected");
                                builder.AppendLine("Continue?");
                                var response = this.MessageBoxService.ShowMessage(builder.ToString(), "Warning", MessageButton.YesNo, MessageIcon.Warning);
                                if (response == MessageResult.Yes) {
                                    if (this.ShowSelectPriceDialog()) {
                                        this.Price = this._selectPricePopupViewModel.SelectedPrice;
                                        this.SelectedDistributor = this.Distributors.FirstOrDefault(e => e.Id == this.Price.DistributorId);
                                        this.NotNoPriceOption = true;
                                        this.returningFromOption = false;
                                    } else {
                                        this.SelectedPriceOption = PriceOption.NoPrice;
                                        this.NotNoPriceOption = false;
                                        this.returningFromOption = false;
                                        this.Price = null;
                                        this.SelectedDistributor = null;
                                    }
                                } else {
                                    this.SelectedPriceOption = previousOption;
                                    this.returningFromOption = true;
                                }
                            } else {
                                this.returningFromOption = false;
                            }
                            break;
                        }
                        case PriceOption.UseExisting: {
                            break;
                        }
                        case PriceOption.NoPrice: {
                            if (this.ShowSelectPriceDialog()) {
                                this.Price = this._selectPricePopupViewModel.SelectedPrice;
                                this.SelectedDistributor = this.Distributors.FirstOrDefault(e => e.Id == this.Price.DistributorId);
                                this.NotNoPriceOption = true;
                                this.returningFromOption = false;
                            }
                            break;
                        }
                    }
                });
            });
        }

        private async Task PriceOptionSelectionChangedHandler(PriceOption previousOption) {
            switch (this.SelectedPriceOption) {
                case PriceOption.CreateNew: {
                    await this.CreatePriceHandler(previousOption);
                    break;
                }
                case PriceOption.UseExisting: {
                    await this.SelectExistingPriceHandler(previousOption);
                    break;
                }
                case PriceOption.NoPrice: {
                    await this.NoPriceSelectedHandler(previousOption);
                    break;
                }
            }
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
            UICommand result = SelectPriceDialog.ShowDialog(
                dialogCommands: new List<UICommand>() { continueCommand, cancelCommand },
                title: "Select Price Dialog", viewModel: this._selectPricePopupViewModel);
            return result == continueCommand;
        }

        #endregion

        #region ActionRegion

        private async Task CheckInHandler() {
            string error = this.EnableValidationAndGetError();
            if (error != null) {
                this.DispatcherService.BeginInvoke(() => {
                    this.OnValidationFailed(error);
                });
                return;
            }else if(this.SelectedPriceOption == PriceOption.NoPrice && this.CostReported && !this._isExisting) {
                this.DispatcherService.BeginInvoke(() => {
                    this.OnValidationFailed("Cost Reported Set To True, You Must Selected A Price Option");
                });
                return;
            }
            if (this._isExisting) {
                await this.CheckInExisiting();
            } else {
                await this.CheckInNew();
            }
        }

        private async Task CheckInExisiting() {
            CheckInInput input = new CheckInInput(this.SelectedPartInstance, this.SelectedPriceOption, this.TransactionTimeStamp, this._partId,true,quantity:this.Quantity);
            var response = await this._checkIn.Execute(input);
            if (response.Success) {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Success", MessageButton.OK, MessageIcon.Information);
                    this._eventAggregator.GetEvent<CheckInDoneEvent>().Publish(response.PartInstance.Id);
                });
            } else {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Error", MessageButton.OK, MessageIcon.Error);
                });
            }
        }

        private async Task CheckInNew() {
            if (this.SelectedStockType != null) {
                this.SelectedPartInstance.StockTypeId = this.SelectedStockType.Id;
            }

            if (this.SelectedCondition != null) {
                this.SelectedPartInstance.ConditionId = this.SelectedCondition.Id;
            }

            if (this.SelectedUsage != null) {
                this.SelectedPartInstance.UsageId = this.SelectedUsage.Id;
            }

            this.SelectedPartInstance.LocationId = this.SelectedWarehouse.Id;
            this.SelectedPartInstance.IsReusable = this.IsReusable;

            if (this.Price != null) {
                this.Price.DistributorId = this.SelectedDistributor.Id;
            }
            this.SelectedPartInstance.Quantity = this.Quantity;
            CheckInInput input = new CheckInInput(this.SelectedPartInstance, this.SelectedPriceOption, this.TransactionTimeStamp, this._partId, false,price:this.Price);

            var response = await this._checkIn.Execute(input);
            if (response.Success) {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Success", MessageButton.OK, MessageIcon.Information);
                    this._eventAggregator.GetEvent<CheckInDoneEvent>().Publish(response.PartInstance.Id);
                });
            } else {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Error", MessageButton.OK, MessageIcon.Error);
                });
            }
        }

        private async Task CancelHandler() {
            await Task.Run(() => this._eventAggregator.GetEvent<CheckInCancelEvent>().Publish());
        }

        private bool CanExecuteCheckIn() {
            return true;
            //if (this.SelectedPartInstance != null) {
            //    if (this._isExisting) {
            //        return this.Quantity!=0;
            //    } else {
            //        if (this.SelectedPartInstance.CostReported) {
            //            return this.SelectedPriceOption != PriceOption.NoPrice && this.SelectedWarehouse != null && this.SelectedDistributor != null;
            //        } else {
            //            if (this.SelectedPriceOption != PriceOption.NoPrice) {
            //                return this.SelectedWarehouse != null && this.SelectedDistributor != null;
            //            } else {
            //                return this.SelectedWarehouse != null;
            //            }
            //        }
            //    }
            //} else {
            //    return false;
            //}
        }

        #endregion

        #region LoadRegion

        private async Task Load() {
            if (this._isExisting) {           
                await this.LoadExisiting();
            } else {
                await this.LoadNew();
            }
        }

        private async Task LoadExisiting() {
            if (!this._isInitialized) {
                this._prices = await this._checkIn.GetAvailablePrices(this._partId);
                var distributors = await this._checkIn.GetDistributors();
                var categories = await this._checkIn.GetCategories();
                var warehouses = await this._checkIn.GetWarehouses();
                var partInstance = await this._checkIn.GetExisitingPartInstance(this._instanceId.Value);
                this.DispatcherService.BeginInvoke(() => {
                    this.SelectedPartInstance = partInstance;
                    this.Distributors = new ObservableCollection<Distributor>(distributors);
                    this.Conditions = new ObservableCollection<Condition>(categories.OfType<Condition>());
                    //this.StockTypes = new ObservableCollection<StockType>(categories.OfType<StockType>());
                    this.Warehouses = new ObservableCollection<Warehouse>(warehouses);
                    this.UsageList = new ObservableCollection<Usage>(categories.OfType<Usage>());
                    this.TransactionTimeStamp = DateTime.Now;
                    this.CostReported = this.SelectedPartInstance.CostReported;
                    this.CanEditStock = this.SelectedPartInstance.StockTypeId==Constants.DefaultStockId;
                    //this.SelectedStockType = this.StockTypes.FirstOrDefault(e => e.Id == partInstance.StockTypeId);
                    this.SelectedWarehouse = this.Warehouses.FirstOrDefault(e => e.Id == partInstance.LocationId);
                    this.IsReusable = this.SelectedPartInstance.IsReusable;

                    if (this.SelectedPartInstance.ConditionId.HasValue) {
                        this.SelectedCondition = this.Conditions.FirstOrDefault(e => e.Id == partInstance.ConditionId);
                    }

                    if (this.SelectedPartInstance.UsageId.HasValue) {
                        this.SelectedUsage = this.UsageList.FirstOrDefault(e => e.Id == partInstance.UsageId);
                    }

                    this.Quantity = 0;
                    this.Name = this.SelectedPartInstance.Name;
                    this.UnitCost = this.SelectedPartInstance.UnitCost;
                });
                this._isInitialized = true;
            }
        }

        private async Task LoadNew() {
            if (!this._isInitialized) {
                this._prices = await this._checkIn.GetAvailablePrices(this._partId);
                var distributors = await this._checkIn.GetDistributors();
                var categories = await this._checkIn.GetCategories();
                var warehouses = await this._checkIn.GetWarehouses();
                var part = await this._checkIn.GetPart(this._partId);

                this.DispatcherService.BeginInvoke(() => {
                    this.Distributors = new ObservableCollection<Distributor>(distributors);
                    this.Conditions = new ObservableCollection<Condition>(categories.OfType<Condition>());
                    //this.StockTypes = new ObservableCollection<StockType>(categories.OfType<StockType>());
                    this.Warehouses = new ObservableCollection<Warehouse>(warehouses);
                    this.UsageList = new ObservableCollection<Usage>(categories.OfType<Usage>());
                    //this.SelectedStockType = this.StockTypes.FirstOrDefault(e => e.Id == Constants.DefaultStockId);
                    this.TransactionTimeStamp = DateTime.Now;
                    var partInstance = new PartInstance();
                    if (part != null) {
                        //partInstance.CostReported = part.DefaultToCostReported;
                        this.CostReported = part.DefaultToCostReported;
                        partInstance.CostReported = part.DefaultToCostReported;
                        if (part.WarehouseId.HasValue) {
                            partInstance.LocationId = part.WarehouseId.Value;
                            this.SelectedWarehouse = this.Warehouses.FirstOrDefault(e => e.Id == part.WarehouseId);
                        }
                    }

                    if (this.IsBubbler) {
                        partInstance.BubblerParameter = new BubblerParameter();
                        partInstance.IsBubbler = true;
                        partInstance.MinQuantity = 1;
                        partInstance.SafeQuantity = 1;
                        this.IsReusable = true;
                    }
                    partInstance.StockTypeId= (this.SelectedStockType != null) ? this.SelectedStockType.Id : Constants.DefaultStockId;
                    this.SelectedPartInstance = partInstance;
                    this.Quantity = (this.IsBubbler) ? 1 : 0;
                    this.CanEditStock = !this.IsBubbler && !this._isExisting;
                    this.CanChangeQuantity = !this.IsBubbler;
                });
                this._isInitialized = true;
            }
        }

        #endregion

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            this._partId =Convert.ToInt32(navigationContext.Parameters[ParameterKeys.PartId]);
            this.IsBubbler = Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsBubbler]);
            this.IsExisting = Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsExisiting]);
            if (this._isExisting) {
                this._instanceId=Convert.ToInt32(navigationContext.Parameters[ParameterKeys.InstanceId]);
            }
            this._isInitialized = false;
            this._eventAggregator.GetEvent<RenameHeaderEvent>().Publish("Check In Product");
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }
    }
}
