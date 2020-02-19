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
        private bool _createTransaction;
        private bool _createPrice;
        private PartInstance _selectedPartInstance=new PartInstance();
        private Price _price;
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
        private DateTime _transactionTimeStamp;
        private bool _notNoPriceOption;
        private PriceOption _selectedPriceOption=PriceOption.NoPrice;
        private bool returningFromOption = false;
        private bool firstPriceOptionEvent = false;

        public AsyncCommand CheckInCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand<PriceOption> PriceOptionSelectionChangedCommand { get; private set; }


        public CheckInViewModel(ICheckInUseCase checkIn,IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._checkIn = checkIn;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.Load);
            this.CheckInCommand = new AsyncCommand(this.CheckInHandler,this.CanExecuteCheckIn);
            this.CancelCommand = new AsyncCommand(this.CancelHandler);
            this.PriceOptionSelectionChangedCommand = new AsyncCommand<PriceOption>(this.PriceOptionSelectionChangedHandler);
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

        public bool CreateTransaction { 
            get => this._createTransaction;
            set => SetProperty(ref this._createTransaction, value);
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

        public bool NotNoPriceOption { 
            get => this._notNoPriceOption;
            set => SetProperty(ref this._notNoPriceOption, value);
        }

        #endregion


        #region ComboEditHandlers
        private async Task CreatePriceHandler(PriceOption previousOption) {
            await Task.Factory.StartNew(() => {
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
            await Task.Factory.StartNew(() => {
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
            if (this.SelectedPartType != null) {
                this.SelectedPartInstance.PartTypeId = this.SelectedPartType.Id;
            }

            if (this.SelectedCondition != null) {
                this.SelectedPartInstance.ConditionId = this.SelectedCondition.Id;
            }

            this.SelectedPartInstance.LocationId = this.SelectedWarehouse.Id;

            if (this.Price != null) {
                this.Price.DistributorId = this.SelectedDistributor.Id;
            }

            CheckInInput input = new CheckInInput(this.SelectedPartInstance,this.SelectedPriceOption, this.TransactionTimeStamp,this._partId,this.Price);

            var response = await this._checkIn.Execute(input);
            if (response.Success) {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Success", MessageButton.OK, MessageIcon.Information);
                    this._eventAggregator.GetEvent<CheckInDoneEvent>().Publish();
                });
            } else {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Error", MessageButton.OK, MessageIcon.Error);
                });
            }
        }

        private Task CancelHandler() {
            return Task.Factory.StartNew(() => this._eventAggregator.GetEvent<CheckInCancelEvent>().Publish());
        }

        private async Task Load() {
            if (!this._isInitialized) {

                this._prices = await this._checkIn.GetAvailablePrices(this._partId);
                var distributors = await this._checkIn.GetDistributors();
                var categories = await this._checkIn.GetCategories();
                var warehouses = await this._checkIn.GetWarehouses();

                this.DispatcherService.BeginInvoke(() => {
                    this.Distributors = new ObservableCollection<Distributor>(distributors);
                    this.Conditions = new ObservableCollection<Condition>(categories.OfType<Condition>());
                    this.PartTypes = new ObservableCollection<PartType>(categories.OfType<PartType>());
                    this.Warehouses = new ObservableCollection<Warehouse>(warehouses);
                    this.TransactionTimeStamp = DateTime.Now;
                    var partInstance = new PartInstance();
                    if (this.IsBubbler) {
                        partInstance.BubblerParameter = new BubblerParameter();
                        partInstance.Quantity = 1;
                        partInstance.MinQuantity = 1;
                        partInstance.SafeQuantity = 1;
                        partInstance.IsBubbler = true;
                    }
                    partInstance.CostReported = false;
                    this.CreateNewPrice = false;
                    this.CreateTransaction = false;
                    this.SelectedPartInstance = partInstance;
                });
                this._isInitialized = true;
            }
        }

        private bool CanExecuteCheckIn() {
            if (this.SelectedPartInstance != null) {
                if (this.SelectedPartInstance.CostReported) {
                    return this.SelectedPriceOption != PriceOption.NoPrice && this.SelectedWarehouse != null && this.SelectedDistributor != null;
                } else {
                    if (this.SelectedPriceOption != PriceOption.NoPrice) {
                        return this.SelectedWarehouse != null && this.SelectedDistributor != null;
                    } else {
                        return this.SelectedWarehouse != null;
                    }
                }
            } else {
                return false;
            }
        }
        
        #endregion



        public override void OnNavigatedTo(NavigationContext navigationContext) {
            this._partId =Convert.ToInt32(navigationContext.Parameters[ParameterKeys.PartId]);
            this.IsBubbler = Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsBubbler]);
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
