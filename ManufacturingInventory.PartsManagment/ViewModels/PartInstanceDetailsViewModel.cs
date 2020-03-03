using DevExpress.Mvvm;
using ManufacturingInventory.Application.Boundaries.PartInstanceDetailsEdit;
using ManufacturingInventory.Application.Boundaries.PriceEdit;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Application.UI.Services;
using ManufacturingInventory.Common.Application.UI.ViewModels;
using ManufacturingInventory.Domain.Enums;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.PartsManagment.Internal;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Condition = ManufacturingInventory.Infrastructure.Model.Entities.Condition;

namespace ManufacturingInventory.PartsManagment.ViewModels {

    public enum MessageType {
        ERROR,
        WARNING,
        INFORMATION
    }

    public class PartInstanceDetailsViewModel : InventoryViewModelNavigationBase {

        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("PartInstanceDetailsMessageService"); }
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("PartInstanceDetailDispatcher"); }
        protected IExportService TransactionExportService { get => ServiceContainer.GetService<IExportService>("TransactionTableExportService"); }
        protected IExportService PriceHistoryExportService { get => ServiceContainer.GetService<IExportService>("PriceHistoryExportService"); }
        protected IWindowService WindowService { get => ServiceContainer.GetService<IWindowService>("PartInstanceDetailsLoadingService"); }

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IPartInstanceDetailsEditUseCase _editInstance;

        private bool _isBubbler = false;
        private bool _isInitialized = false;
        private bool _canEdit = false;
        private bool _isReusable;
        private bool _canEditStock;
        private bool _costReported;

        private bool _priceEditInProgress = false;
        private int _partId;
        private int _instanceId;
        private LoadingViewModel _loadingViewModel;

        private ObservableCollection<Condition> _conditions;
        private ObservableCollection<Location> _locations;
        private ObservableCollection<StockType> _stockTypes;
        private ObservableCollection<Usage> _usageList;
        private ObservableCollection<Transaction> _transactions;
        private ObservableCollection<Attachment> _attachments;

        private PartInstance _selectedPartInstance;
        private Location _selectedLocation;
        private Condition _selectedCondition;
        private Usage _selectedUsage;
        private StockType _selectedPartType;
        private AttachmentDataTraveler _instanceAttachmentTraveler;
        private PriceDataTraveler _priceDataTraveler;
        private bool _hasPrice;
        private string _addPriceButtonText;
        private int _selectedTabIndex;
        private bool _tableLoading;
        private double _measuredWeight;
        private double _weight;
        private double _grossWeight;
        private double _netWeight;
        private double _unitCost;
        private double _totalCost;
        private string _comments;
        private string _description;
        private int _quantity;


        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }

        public AsyncCommand NewPriceCommand { get; private set; }
        public AsyncCommand SelectPriceCommand { get; private set; }
        public AsyncCommand EditPriceCommand { get; private set; }

        public AsyncCommand ClosingCommand { get; private set; }

        public AsyncCommand<ExportFormat> ExportPriceHistoryCommand { get; private set; }
        public AsyncCommand<ExportFormat> ExportTransactionsCommand { get; private set; }

        public PartInstanceDetailsViewModel(IPartInstanceDetailsEditUseCase editInstance, IEventAggregator eventAggregator,IRegionManager regionManager) {
            this._editInstance = editInstance;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.InitializedHandler);
            this.EditPriceCommand = new AsyncCommand(this.EditPriceHandler, this.CanModifyPrice);
            this.NewPriceCommand = new AsyncCommand(this.NewPriceHandler,this.CanAddPrice);
            this.SelectPriceCommand = new AsyncCommand(this.SelectPriceHandler, this.CanModifyPrice);
            this.SaveCommand = new AsyncCommand(this.SaveHandler,this.CanSave);
            this.CancelCommand = new AsyncCommand(this.DiscardHandler);
            this.ExportTransactionsCommand = new AsyncCommand<ExportFormat>(this.ExportTransactionsHandler);
            this.ExportPriceHistoryCommand = new AsyncCommand<ExportFormat>(this.ExportPriceHistoryHandler);
            this.ClosingCommand = new AsyncCommand(this.ClosingHandler);
            this._eventAggregator.GetEvent<PriceEditDoneEvent>().Subscribe(async () => await this.PriceEditDoneHandler());
            this._eventAggregator.GetEvent<PriceEditCancelEvent>().Subscribe(async () => await this.PriceEditCancelHandler());

        }

        #region BindingVariables

        public override bool KeepAlive => false;

        public ObservableCollection<Condition> Conditions {
            get => this._conditions;
            set => SetProperty(ref this._conditions, value);
        }

        public ObservableCollection<Location> Locations {
            get => this._locations;
            set => SetProperty(ref this._locations, value);
        }

        public ObservableCollection<StockType> StockTypes {
            get => this._stockTypes;
            set => SetProperty(ref this._stockTypes, value);
        }

        public AttachmentDataTraveler AttachmentDataTraveler {
            get => this._instanceAttachmentTraveler;
            set => SetProperty(ref this._instanceAttachmentTraveler, value);
        }

        public PartInstance SelectedPartInstance {
            get => this._selectedPartInstance;
            set => SetProperty(ref this._selectedPartInstance, value);
        }

        public Location SelectedLocation {
            get => this._selectedLocation;
            set => SetProperty(ref this._selectedLocation, value);
        }

        public StockType SelectedStockType {
            get => this._selectedPartType;
            set {
                this.CanEditStock = !value.IsDefault;
                if (!value.IsDefault) {
                    this.SelectedPartInstance.MinQuantity = this.SelectedStockType.MinQuantity;
                    this.SelectedPartInstance.SafeQuantity = this.SelectedStockType.SafeQuantity;
                    RaisePropertyChanged("SelectedPartInstance");
                }
                SetProperty(ref this._selectedPartType, value);
            }
        }

        public Condition SelectedCondition {
            get => this._selectedCondition;
            set => SetProperty(ref this._selectedCondition, value);
        }

        public bool IsBubbler {
            get => this._isBubbler;
            set => SetProperty(ref this._isBubbler, value);
        }

        public bool CanEdit {
            get => this._canEdit;
            set => SetProperty(ref this._canEdit, value);
        }

        public double Weight {
            get => this._weight;
            set => this.SetProperty(ref this._weight, value);
        }

        public double Measured {
            get => this._measuredWeight;
            set {
                if (this.IsBubbler && this.SelectedPartInstance != null) {
                    this.SelectedPartInstance.UpdateWeight(value);
                    this.Weight = this.SelectedPartInstance.BubblerParameter.Weight;
                }
                this.SetProperty(ref this._measuredWeight, value);
            }
        }

        public double GrossWeight {
            get => this._grossWeight;
            set {
                if (this.IsBubbler && this.SelectedPartInstance != null) {
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
                if (this.IsBubbler && this.SelectedPartInstance != null) {
                    this.SelectedPartInstance.BubblerParameter.NetWeight = value;
                    this.SelectedPartInstance.UpdateWeight();
                    this.Weight = this.SelectedPartInstance.BubblerParameter.Weight;
                    this.TotalCost = this.SelectedPartInstance.TotalCost;
                }
                SetProperty(ref this._netWeight, value);
            }
        }

        public double UnitCost {
            get => this._unitCost;
            set => SetProperty(ref this._unitCost, value);
        }

        public double TotalCost {
            get => this._totalCost;
            set => SetProperty(ref this._totalCost, value);
        }

        public int Quantity {
            get => this._quantity;
            set {
                //if (!this.IsBubbler && this.SelectedPartInstance != null) {
                //    this.SelectedPartInstance.EditQuantity(value);
                //    this.TotalCost = this.SelectedPartInstance.TotalCost;
                //}
                SetProperty(ref this._quantity, value);
            }
        }

        public ObservableCollection<Transaction> Transactions {
            get => this._transactions;
            set => this.SetProperty(ref this._transactions, value);
        }

        public ObservableCollection<Attachment> Attachments {
            get => this._attachments;
            set => this.SetProperty(ref this._attachments, value);
        }

        public AttachmentDataTraveler InstanceAttachmentTraveler {
            get => this._instanceAttachmentTraveler;
            set => SetProperty(ref this._instanceAttachmentTraveler, value);
        }

        public bool TableLoading {
            get => this._tableLoading;
            set => SetProperty(ref this._tableLoading, value);
        }

        public PriceDataTraveler PriceDataTraveler {
            get => this._priceDataTraveler;
            set => SetProperty(ref this._priceDataTraveler, value);
        }

        public int SelectedTabIndex {
            get => this._selectedTabIndex;
            set => SetProperty(ref this._selectedTabIndex, value);
        }

        public string AddPriceButtonText {
            get => this._addPriceButtonText;
            set => SetProperty(ref this._addPriceButtonText, value);
        }

        public bool HasPrice {
            get => this._hasPrice;
            set => SetProperty(ref this._hasPrice, value);
        }

        public string Comments {
            get => this._comments;
            set => SetProperty(ref this._comments, value);
        }

        public string Description {
            get => this._description;
            set => SetProperty(ref this._description, value);
        }

        public ObservableCollection<Usage> UsageList { 
            get => this._usageList;
            set => SetProperty(ref this._usageList, value);
        }

        public Usage SelectedUsage { 
            get => this._selectedUsage;
            set => SetProperty(ref this._selectedUsage, value);
        }

        public bool CanEditStock {
            get => this._canEditStock;
            set => SetProperty(ref this._canEditStock, value);
        }

        public bool CostReported {
            get => this._costReported;
            set {
                this.SelectedPartInstance.CostReported = value;
                SetProperty(ref this._costReported, value);
            }
        }

        public bool IsReusable {
            get => this._isReusable;
            set => SetProperty(ref this._isReusable, value);
        }

        #endregion

        #region ButtonHandlers

        private async Task NewPriceHandler() {
            await Task.Run(() => {
                this._priceEditInProgress = true;
                this.DispatcherService.BeginInvoke(() => {
                    PriceEditOption option = (this._hasPrice) ? PriceEditOption.ReplaceWithNew : PriceEditOption.NEW;
                    this.NavigatePriceEdit(option);
                });
            });
        }

        private async Task SelectPriceHandler() {
            await Task.Run(() => {
                this._priceEditInProgress = true;
                this.DispatcherService.BeginInvoke(() => {
                    this.NavigatePriceSelect();
                });
            });
        }

        private async Task EditPriceHandler() {
            await Task.Run(() => {
                this._priceEditInProgress = true;
                this.DispatcherService.BeginInvoke(() => {
                    this.NavigatePriceEdit(PriceEditOption.Edit);
                    //this.NavigatePriceEdit(false, true);
                });
            });
        }

        public async Task SaveHandler() {
            if (this.SelectedStockType != null) {
                this.SelectedPartInstance.StockTypeId = this.SelectedStockType.Id;
            }

            if (this.SelectedCondition != null) {
                this.SelectedPartInstance.ConditionId = this.SelectedCondition.Id;
            }

            if (this.SelectedUsage != null) {
                this.SelectedPartInstance.UsageId = this.SelectedUsage.Id;
            }

            this.SelectedPartInstance.LocationId = this.SelectedLocation.Id;
            this.SelectedPartInstance.Description = this.Description;
            this.SelectedPartInstance.Comments = this.Comments;

            PartInstanceDetailsEditInput input = new PartInstanceDetailsEditInput(this.SelectedPartInstance);
            var output = await this._editInstance.Execute(input);
            if (output.Success) {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(output.Message + Environment.NewLine + "Reloading", "Saved", MessageButton.OK, MessageIcon.Information);
                });
                ReloadEventTraveler traveler = new ReloadEventTraveler() {
                    PartId = this.SelectedPartInstance.PartId,
                    PartInstanceId = this.SelectedPartInstance.Id
                };
                this._eventAggregator.GetEvent<PartInstanceEditDoneEvent>().Publish(traveler);
            } else {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage("Error Save Part Instance", "Error", MessageButton.OK, MessageIcon.Error);
                });
            }
        }

        public void ShowLoading() {
            if (this._loadingViewModel == null)
                this._loadingViewModel = new LoadingViewModel() { Message = "Loading Please Wait...." };
            this.WindowService.Show(this._loadingViewModel);
        }

        public async Task DiscardHandler() {
            await Task.Run(() => {
                ReloadEventTraveler traveler = new ReloadEventTraveler() {
                    PartId = this.SelectedPartInstance.PartId,
                    PartInstanceId = this.SelectedPartInstance.Id
                };
                this.DispatcherService.BeginInvoke(() => {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("Warning:")
                    .AppendLine("You will lose all changes if continue")
                    .AppendLine("Continue?");

                    var response=this.MessageBoxService.ShowMessage(builder.ToString(), "Warning", MessageButton.YesNo, MessageIcon.Warning);
                    if (response ==MessageResult.Yes) {
                        this._eventAggregator.GetEvent<PartInstanceEditCancelEvent>().Publish(traveler);
                        this.CanEdit = false;
                    }
                });
            });
        }

        private async Task ExportPriceHistoryHandler(ExportFormat format) {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
                    using (FileStream file = File.Create(path)) {
                        this.PriceHistoryExportService.Export(file, format);
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

        private async Task ExportTransactionsHandler(ExportFormat format) {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
                    using (FileStream file = File.Create(path)) {
                        this.TransactionExportService.Export(file, format);
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

        public bool CanSave() {
            return this.SelectedLocation != null && !this._priceEditInProgress;
        }

        public bool CanModifyPrice() {
            return !this._priceEditInProgress && !this.CanEdit  && this.HasPrice;
        }

        public bool CanAddPrice() {
            return !this._priceEditInProgress && !this.CanEdit;
        }

        #endregion

        #region Callbacks

        private async Task PriceEditDoneHandler() {
            this._priceEditInProgress = false;
            await this.ReloadHandler();
        }

        private async Task PriceEditCancelHandler() {
            this._priceEditInProgress = false;
            await this.ReloadHandler();
        }

        public async Task ReloadHandler() {
            await this._editInstance.LoadAsync();
            var instance = await this._editInstance.GetPartInstance(this._instanceId);
            var categories = await this._editInstance.GetCategories();
            var locations = await this._editInstance.GetLocations();
            var transactions = await this._editInstance.GetTransactions(this._instanceId);
            this.Transactions = new ObservableCollection<Transaction>(transactions);

            this.DispatcherService.BeginInvoke(() => {
                this.SelectedPartInstance = instance;
                this.Conditions = new ObservableCollection<Condition>(categories.OfType<Condition>());
                this.StockTypes = new ObservableCollection<StockType>(categories.OfType<StockType>());
                this.UsageList = new ObservableCollection<Usage>(categories.OfType<Usage>());
                this.Locations = new ObservableCollection<Location>(locations);
                if (this.SelectedPartInstance != null) {
                    this.Comments = this.SelectedPartInstance.Comments;
                    this.Description = this.SelectedPartInstance.Description;
                    this.Quantity = this.SelectedPartInstance.Quantity;

                    if (this.IsBubbler) {
                        this.GrossWeight = this.SelectedPartInstance.BubblerParameter.GrossWeight;
                        this.Measured = this.SelectedPartInstance.BubblerParameter.Measured;
                        this.NetWeight = this.SelectedPartInstance.BubblerParameter.NetWeight;
                    }

                    if (this.SelectedPartInstance.PriceId.HasValue) {
                        this.UnitCost = this.SelectedPartInstance.Price.UnitCost;
                        this.TotalCost = this.SelectedPartInstance.TotalCost;
                        this.HasPrice = true;
                        this.AddPriceButtonText = "Add and Replace Price";
                        this.NavigatePriceEdit(false, false);
                    } else {
                        this.UnitCost = 0;
                        this.TotalCost = 0;
                        this.HasPrice = false;
                        this.AddPriceButtonText = "Add Price";
                    }

                    this.AttachmentDataTraveler = new AttachmentDataTraveler(GetAttachmentBy.PARTINSTANCE, this.SelectedPartInstance.Id);

                    if (this.SelectedPartInstance.Condition != null) {
                        this.SelectedCondition = this.Conditions.FirstOrDefault(e => e.Id == this.SelectedPartInstance.ConditionId);
                    }

                    if (this.SelectedPartInstance.Usage != null) {
                        this.SelectedUsage = this.UsageList.FirstOrDefault(e => e.Id == this.SelectedPartInstance.UsageId);
                    }

                    this.SelectedLocation = this.Locations.FirstOrDefault(e => e.Id == this.SelectedPartInstance.LocationId);
                    this.SelectedStockType = this.StockTypes.FirstOrDefault(e => e.Id == this.SelectedPartInstance.StockTypeId);
                    this.CanEditStock = (this.SelectedPartInstance.StockTypeId == Constants.DefaultStockId) && this.CanEdit;
                    this.IsReusable = this.SelectedPartInstance.IsReusable || this.IsBubbler;
                }
            });
        }

        #endregion

        #region NavigationPrice

        public void NavigatePriceEdit(bool isNewPrice, bool isPriceEdit) {
            NavigationParameters param = new NavigationParameters();
            param.Add(ParameterKeys.PriceId, this.SelectedPartInstance.PriceId);
            param.Add(ParameterKeys.InstanceId, this.SelectedPartInstance.Id);
            param.Add(ParameterKeys.PartId, this.SelectedPartInstance.PartId);
            param.Add(ParameterKeys.IsEdit, isPriceEdit);
            param.Add(ParameterKeys.IsNew, isNewPrice);
            this._regionManager.RequestNavigate(LocalRegions.InstancePriceEditDetailsRegion, ModuleViews.PriceDetailsView, param);
        }

        public void NavigatePriceEdit(PriceEditOption editOption) {
            NavigationParameters param = new NavigationParameters();
            param.Add(ParameterKeys.PriceId, this.SelectedPartInstance.PriceId);
            param.Add(ParameterKeys.InstanceId, this.SelectedPartInstance.Id);
            param.Add(ParameterKeys.PartId, this.SelectedPartInstance.PartId);
            param.Add(ParameterKeys.PriceEditOption, Enum.ToObject(typeof(PriceEditOption),editOption));
            this._regionManager.RequestNavigate(LocalRegions.InstancePriceEditDetailsRegion, ModuleViews.PriceDetailsView, param);
        }

        public void NavigatePriceSelect() {
            NavigationParameters param = new NavigationParameters();
            param.Add(ParameterKeys.PriceId, this.SelectedPartInstance.PriceId);
            param.Add(ParameterKeys.InstanceId, this._instanceId);
            param.Add(ParameterKeys.PartId, this.SelectedPartInstance.PartId);
            this._regionManager.RequestNavigate(LocalRegions.InstancePriceEditDetailsRegion, ModuleViews.SelectPriceView, param);
        }

        public void CleanupRegions() {
            this._regionManager.Regions[LocalRegions.InstancePriceEditDetailsRegion].RemoveAll();
        }

        #endregion

        #region InitAndNavigate

        public async Task InitializedHandler() {
            if (!this._isInitialized) {
                var instance = await this._editInstance.GetPartInstance(this._instanceId);
                var categories = await this._editInstance.GetCategories();
                var locations = await this._editInstance.GetLocations();
                var transactions = await this._editInstance.GetTransactions(this._instanceId);
                this.Transactions = new ObservableCollection<Transaction>(transactions);

                this.DispatcherService.BeginInvoke(() => {
                    this.SelectedPartInstance = instance;
                    this.Conditions = new ObservableCollection<Condition>(categories.OfType<Condition>());
                    this.StockTypes = new ObservableCollection<StockType>(categories.OfType<StockType>());
                    this.UsageList = new ObservableCollection<Usage>(categories.OfType<Usage>());
                    this.Locations = new ObservableCollection<Location>(locations);
                    if (this.SelectedPartInstance != null) {
                        this.Comments = this.SelectedPartInstance.Comments;
                        this.Description = this.SelectedPartInstance.Description;
                        this.Quantity = this.SelectedPartInstance.Quantity;
                        if (this.IsBubbler) {
                            this.GrossWeight = this.SelectedPartInstance.BubblerParameter.GrossWeight;
                            this.Measured = this.SelectedPartInstance.BubblerParameter.Measured;
                            this.NetWeight = this.SelectedPartInstance.BubblerParameter.NetWeight;
                        }

                        if (this.SelectedPartInstance.Price != null) {
                            this.UnitCost = this.SelectedPartInstance.UnitCost;
                            this.TotalCost = this.SelectedPartInstance.TotalCost;
                            this.HasPrice = true;
                            this.AddPriceButtonText = "Add and Replace Price";
                            this.NavigatePriceEdit(false, false);
                        } else {
                            this.UnitCost = 0;
                            this.TotalCost = 0;
                            this.HasPrice = false;
                            this.AddPriceButtonText = "Add Price";
                        }

                        this.AttachmentDataTraveler = new AttachmentDataTraveler(GetAttachmentBy.PARTINSTANCE, this.SelectedPartInstance.Id);
                        if (this.SelectedPartInstance.Condition != null) {
                            this.SelectedCondition = this.Conditions.FirstOrDefault(e => e.Id == this.SelectedPartInstance.ConditionId);
                        }

                        this.SelectedLocation = this.Locations.FirstOrDefault(e => e.Id == this.SelectedPartInstance.LocationId);
                        this.SelectedStockType = this.StockTypes.FirstOrDefault(e => e.Id == this.SelectedPartInstance.StockTypeId);
                        this.CanEditStock = (this.SelectedPartInstance.StockTypeId == Constants.DefaultStockId) && this.CanEdit;
                        this.IsReusable = (this.SelectedPartInstance.IsReusable || this.IsBubbler);
                        this.CostReported = this.SelectedPartInstance.CostReported;

                        if (this.SelectedPartInstance.Usage != null) {
                            this.SelectedUsage = this.UsageList.FirstOrDefault(e => e.Id == this.SelectedPartInstance.UsageId);
                        }
                    }
                    this._isInitialized = true;
                });
            }
        }

        private async Task ClosingHandler() {
            await Task.Run(() => {
                if (this.WindowService.IsWindowAlive) {
                    this._loadingViewModel = null;
                    this.WindowService.Close();
                }
            });
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            this._isInitialized = false;
            this.IsBubbler = Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsBubbler]);
            this._partId = Convert.ToInt32(navigationContext.Parameters[ParameterKeys.PartId]);
            this._instanceId = Convert.ToInt32(navigationContext.Parameters[ParameterKeys.InstanceId]);
            this.CanEdit = Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsEdit]);
            this._eventAggregator.GetEvent<RenameHeaderEvent>().Publish("Details");
        }

        #endregion


    }
}
