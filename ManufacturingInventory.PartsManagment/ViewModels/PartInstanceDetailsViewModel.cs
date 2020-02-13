using DevExpress.Mvvm;
using ManufacturingInventory.Application.Boundaries.PartInstanceDetailsEdit;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Application.UI.Services;
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

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IPartInstanceDetailsEditUseCase _editInstance;

        private bool _isEdit = false;
        private bool _isNew = false;
        private bool _isBubbler = false;
        private bool _isInitialized = false;
        private bool _canEdit = false;

        private bool _priceEditInProgress = false;
        private int _partId;

        private ObservableCollection<Condition> _conditions;
        private ObservableCollection<Location> _locations;
        private ObservableCollection<PartType> _partTypes;
        private ObservableCollection<Transaction> _transactions;
        private ObservableCollection<Attachment> _attachments;

        private PartInstance _selectedPartInstance;
        private Location _selectedLocation;
        private Condition _selectedCondition;
        private PartType _selectedPartType;
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

        public AsyncCommand<ExportFormat> ExportPriceHistoryCommand { get; private set; }
        public AsyncCommand<ExportFormat> ExportTransactionsCommand { get; private set; }

        public PartInstanceDetailsViewModel(IPartInstanceDetailsEditUseCase editInstance, IEventAggregator eventAggregator,IRegionManager regionManager) {
            this._editInstance = editInstance;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.InitializedHandler);
            this.EditPriceCommand = new AsyncCommand(this.EditPriceHandler, this.CanModifyPrice);
            this.NewPriceCommand = new AsyncCommand(this.NewPriceHandler,this.CanModifyPrice);
            this.SelectPriceCommand = new AsyncCommand(this.SelectPriceHandler, this.CanModifyPrice);
            this.SaveCommand = new AsyncCommand(this.SaveHandler,this.CanSave);
            this.CancelCommand = new AsyncCommand(this.DiscardHandler);
            this.ExportTransactionsCommand = new AsyncCommand<ExportFormat>(this.ExportTransactionsHandler);
            this.ExportPriceHistoryCommand = new AsyncCommand<ExportFormat>(this.ExportPriceHistoryHandler);
            this._eventAggregator.GetEvent<PriceEditDoneEvent>().Subscribe(async () => await this.EditPriceDone());
        }

        public override bool KeepAlive => false;

        public ObservableCollection<Condition> Conditions { 
            get => this._conditions;
            set => SetProperty(ref this._conditions, value);
        }

        public ObservableCollection<Location> Locations { 
            get => this._locations;
            set => SetProperty(ref this._locations, value);
        }

        public ObservableCollection<PartType> PartTypes {
            get => this._partTypes;
            set => SetProperty(ref this._partTypes, value);
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

        public PartType SelectedPartType { 
            get => this._selectedPartType;
            set => SetProperty(ref this._selectedPartType, value);
        }

        public Condition SelectedCondition {
            get => this._selectedCondition;
            set => SetProperty(ref this._selectedCondition, value);
        }

        public bool IsBubbler {
            get => this._isBubbler;
            set => SetProperty(ref this._isBubbler, value);
        }

        public bool IsEdit {
            get => this._isEdit;
            set => SetProperty(ref this._isEdit, value);
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
                if (this.IsBubbler && this.SelectedPartInstance!=null) {
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
                if (this.IsBubbler && this.SelectedPartInstance!=null) {
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
                if (!this.IsBubbler  && this.SelectedPartInstance!=null) {
                    if (this.SelectedPartInstance.Price != null) {
                        this.SelectedPartInstance.Quantity = value;
                        this.TotalCost = this.SelectedPartInstance.Price.UnitCost * value;
                    }
                }
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
            set => SetProperty(ref this._selectedTabIndex,value); 
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

        public async Task SaveHandler() {
            if (this.SelectedPartType != null) {
                this.SelectedPartInstance.PartTypeId = this.SelectedPartType.Id;
            }

            if (this.SelectedCondition != null) {
                this.SelectedPartInstance.ConditionId = this.SelectedCondition.Id;
            }

            this.SelectedPartInstance.LocationId = this.SelectedLocation.Id;
            this.SelectedPartInstance.Description = this.Description;
            this.SelectedPartInstance.Comments = this.Comments;

            PartInstanceDetailsEditInput input = new PartInstanceDetailsEditInput(this.SelectedPartInstance, this._isNew);
            var output = await this._editInstance.Execute(input);
            if (output.Success) {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(output.Message + Environment.NewLine + "Reloading", "Saved", MessageButton.OK, MessageIcon.Information);
                });
                ReloadEventTraveler traveler = new ReloadEventTraveler() {
                    PartId = this.SelectedPartInstance.PartId,
                    PartInstanceId = this.SelectedPartInstance.Id
                };
                this._eventAggregator.GetEvent<ReloadEvent>().Publish(traveler);
                this.CanEdit = false;
            } else {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage("Error Save Part Instance", "Error", MessageButton.OK, MessageIcon.Error);
                });
            }

        }

        public Task DiscardHandler() {
            return Task.Factory.StartNew(() => {
                ReloadEventTraveler traveler = new ReloadEventTraveler() {
                    PartId = this.SelectedPartInstance.PartId,
                    PartInstanceId = this.SelectedPartInstance.Id
                };
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage("", "", MessageButton.OK, MessageIcon.Error);
                    this._eventAggregator.GetEvent<ReloadEvent>().Publish(traveler);
                    this.IsEdit = false;
                });
            });
        }

        private Task NewPriceHandler() {
            return Task.Factory.StartNew(() => {
                this._priceEditInProgress = true;
                this.DispatcherService.BeginInvoke(() => {
                    this.NavigatePriceEdit(true, false);
                });
            });
        }

        private Task SelectPriceHandler() {
            return Task.Factory.StartNew(() => {
                this._priceEditInProgress = true;
                this.DispatcherService.BeginInvoke(() => {
                    this.NavigatePriceSelect();
                });
            });
        }

        private Task EditPriceHandler() {
            return Task.Factory.StartNew(() => {
                this._priceEditInProgress = true;
                this.DispatcherService.BeginInvoke(() => {
                    this.NavigatePriceEdit(false, true);
                });
            });
        }

        private async Task EditPriceDone() {
            await this._editInstance.LoadAsync();
            this.SelectedPartInstance = await this._editInstance.GetPartInstance(this.SelectedPartInstance.Id);
            this._isInitialized = false;
            this._priceEditInProgress = false;
            await this.InitializedHandler();

        }

        public bool CanSave() {
            return this.SelectedLocation != null && !this._priceEditInProgress;
        }

        public bool CanModifyPrice() {
            return !this._priceEditInProgress && !this.CanEdit;
        }

        public MessageResult ShowMessage(MessageType messageType,string message) {
            switch (messageType) {
                case MessageType.ERROR: {
                    return this.MessageBoxService.ShowMessage(message, "Error", MessageButton.OK, MessageIcon.Error);
                }
                case MessageType.WARNING: {
                    return this.MessageBoxService.ShowMessage(message, "Warning", MessageButton.OK, MessageIcon.Warning);
                }
                case MessageType.INFORMATION: {
                    return this.MessageBoxService.ShowMessage(message, "Error", MessageButton.OK, MessageIcon.Information);
                }
                default: return MessageResult.None;
            }
        }

        public void NavigatePriceEdit(bool isNewPrice,bool isPriceEdit) {
            NavigationParameters param = new NavigationParameters();
            param.Add(ParameterKeys.PriceId, this.SelectedPartInstance.PriceId);
            param.Add(ParameterKeys.InstanceId, this.SelectedPartInstance.Id);
            param.Add(ParameterKeys.PartId, this.SelectedPartInstance.PartId);
            param.Add(ParameterKeys.IsEdit, isPriceEdit);
            param.Add(ParameterKeys.IsNew, isNewPrice);
            this._regionManager.RequestNavigate(LocalRegions.InstancePriceEditDetailsRegion, ModuleViews.PriceDetailsView, param);
        }

        public void NavigatePriceSelect() {
            NavigationParameters param = new NavigationParameters();
            param.Add(ParameterKeys.PriceId, this.SelectedPartInstance.PriceId);
            param.Add(ParameterKeys.InstanceId, this.SelectedPartInstance.Id);
            param.Add(ParameterKeys.PartId, this.SelectedPartInstance.PartId);
            this._regionManager.RequestNavigate(LocalRegions.InstancePriceEditDetailsRegion, ModuleViews.SelectPriceView, param);
        }

        public void CleanupRegions() {
           this._regionManager.Regions[LocalRegions.InstancePriceEditDetailsRegion].RemoveAll();
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

        public async Task InitializedHandler() {
            if (!this._isInitialized) {
                var categories = await this._editInstance.GetCategories();
                var locations = await this._editInstance.GetLocations();
                if (!this._isNew) {
                    var transactions = await this._editInstance.GetTransactions(this.SelectedPartInstance.Id);
                    this.Transactions = new ObservableCollection<Transaction>(transactions);
                }

                this.DispatcherService.BeginInvoke(() => {
                    this.Conditions = new ObservableCollection<Condition>(categories.OfType<Condition>());
                    this.PartTypes = new ObservableCollection<PartType>(categories.OfType<PartType>());
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

                        if (this.SelectedPartInstance.CurrentLocation != null) {
                            this.SelectedLocation = this.Locations.FirstOrDefault(e => e.Id == this.SelectedPartInstance.LocationId);
                        }

                        if (this.SelectedPartInstance.PartType != null) {
                            this.SelectedPartType = this.PartTypes.FirstOrDefault(e => e.Id == this.SelectedPartInstance.PartTypeId);
                        }

                    } else {
                        if (this._isNew) {
                            this.UnitCost = 0;
                            this.TotalCost = 0;
                            this.HasPrice = false;
                            this.AddPriceButtonText = "Add Price";
                            if (this.IsBubbler) {
                                this.GrossWeight = 0;
                                this.Measured = 0;
                                this.NetWeight = 0;
                                this.Quantity = 1;
                            } else {
                                this.Quantity = 0;
                            }
                        }
                    }
                    this._isInitialized = true;
                });
            }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            this._isInitialized = false;
            this._isNew = Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsNew]);
            this.IsBubbler= Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsBubbler]);
            this._partId = Convert.ToInt32(navigationContext.Parameters[ParameterKeys.PartId]);
            if (!this._isNew){
                this.SelectedPartInstance = navigationContext.Parameters[ParameterKeys.SelectedPartInstance] as PartInstance;
                this.IsEdit = Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsEdit]);
            } else {
                this.SelectedPartInstance = new PartInstance();
                this.SelectedPartInstance.PartId = this._partId;
                this.SelectedPartInstance.BubblerParameter = new BubblerParameter(0, 0, 0);
                this.SelectedPartInstance.IsBubbler = IsBubbler;
                this.SelectedPartInstance.Quantity = 1;
            }
            this.CanEdit = this.IsEdit || this._isNew;
        }
    }
}
