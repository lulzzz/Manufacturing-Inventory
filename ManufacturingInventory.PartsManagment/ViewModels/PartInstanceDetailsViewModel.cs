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
using Condition = ManufacturingInventory.Common.Model.Entities.Condition;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class PartInstanceDetailsViewModel : InventoryViewModelNavigationBase {

        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("PartInstanceDetailsMessageService"); }
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("PartInstanceDetailDispatcher"); }

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private ManufacturingContext _context;

        private bool _isEdit = false;
        private bool _isNew = false;
        private bool _isBubbler = false;
        private bool _isNotBubbler = false;
        private bool _isInitialized = false;

        private ObservableCollection<Condition> _conditions;
        private ObservableCollection<Location> _locations;
        private ObservableCollection<PartType> _partTypes;
        private Visibility _costVisibility;
        private Visibility _stockVisibility;
        private Visibility _weightVisibility;
        private Visibility _saveCancelVisibility;
        private PartInstance _selectedPartInstance;
        private Location _selectedLocation;
        private Condition _selectedCondition;
        private PartType _selectedPartType;
        private int _selectedConditionIndex;
        private int _selectedPartTypeIndex;
        private int _selectedLocationIndex;

        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }

        public PartInstanceDetailsViewModel(ManufacturingContext context,IEventAggregator eventAggregator,IRegionManager regionManager) {
            this._regionManager = regionManager;
            this._context = context;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.InitializedHandler);
            this.SaveCommand = new AsyncCommand(this.SaveHandler);
            this.CancelCommand = new AsyncCommand(this.DiscardHandler);
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

        public int SelectedConditionIndex { 
            get => this._selectedConditionIndex;
            set => SetProperty(ref this._selectedConditionIndex, value);
        }

        public int SelectedPartTypeIndex { 
            get => this._selectedPartTypeIndex;
            set => SetProperty(ref this._selectedPartTypeIndex, value);
        }

        public int SelectedLocationIndex { 
            get => this._selectedLocationIndex;
            set => SetProperty(ref this._selectedLocationIndex, value);
        }

        public bool IsNotBubbler {
            get => this._isNotBubbler;
            set => SetProperty(ref this._isNotBubbler, value);
        }

        public bool IsBubbler {
            get => this._isBubbler;
            set => SetProperty(ref this._isBubbler, value);
        }

        public bool IsEdit {
            get => this._isEdit;
            set => SetProperty(ref this._isEdit, !value);
        }

        public Visibility CostVisibility { 
            get => this._costVisibility;
            set => SetProperty(ref this._costVisibility, value);
        }

        public Visibility StockVisibility { 
            get => this._stockVisibility;
            set => SetProperty(ref this._stockVisibility, value);
        }

        public Visibility WeightVisibility { 
            get => this._weightVisibility;
            set => SetProperty(ref this._weightVisibility, value);
        }

        public Visibility SaveCancelVisibility { 
            get => this._saveCancelVisibility;
            set => SetProperty(ref this._saveCancelVisibility, value);
        }

        public async Task InitializedHandler() {
            if (!this._isInitialized) {
                var conditions = await this._context.Categories.OfType<Condition>().ToListAsync();
                this.Conditions = new ObservableCollection<Condition>(conditions);

                var types = await this._context.Categories.OfType<PartType>().ToListAsync();
                this.PartTypes = new ObservableCollection<PartType>(types);

                var locations = await this._context.Locations.ToListAsync();
                this.Locations = new ObservableCollection<Location>(locations);
                if (this.SelectedPartInstance != null) {

                    if (this.SelectedPartInstance.Condition != null) {
                        this.SelectedCondition = this.Conditions.FirstOrDefault(e => e.Id == this.SelectedPartInstance.ConditionId);
                    }

                    if (this.SelectedPartInstance.CurrentLocation != null) {
                        this.SelectedLocation = this.Locations.FirstOrDefault(e => e.Id == this.SelectedPartInstance.LocationId);
                    }

                    if (this.SelectedPartInstance.PartType != null) {
                        this.SelectedPartType = this.PartTypes.FirstOrDefault(e => e.Id == this.SelectedPartInstance.PartTypeId);
                    }
                }
                this._isInitialized = true;
            }
        }

        public async Task SaveHandler() {
            var partInstance = this._context.PartInstances.Include(e => e.BubblerParameter).FirstOrDefault(e => e.Id == this.SelectedPartInstance.Id);
            if (partInstance != null) {
                if (this.SelectedPartInstance.Condition != null) {
                    if (this.SelectedCondition.Id != this.SelectedPartInstance.ConditionId) {
                        partInstance.ConditionId = this.SelectedCondition.Id;
                    }
                }

                if (this.SelectedPartInstance.CurrentLocation != null) {
                    if (this.SelectedPartInstance.LocationId != this.SelectedLocation.Id) {
                        partInstance.LocationId = this.SelectedLocation.Id;
                    }
                }

                if (this.SelectedPartInstance.PartType != null) {
                    if (this.SelectedPartInstance.PartTypeId != this.SelectedPartType.Id) {
                        partInstance.PartTypeId = this.SelectedPartType.Id;
                    }
                }
                if (this._isBubbler) {
                    partInstance.BubblerParameter.Measured = this.SelectedPartInstance.BubblerParameter.Measured;
                    partInstance.BubblerParameter.NetWeight = this.SelectedPartInstance.BubblerParameter.NetWeight;
                    partInstance.BubblerParameter.Tare = this.SelectedPartInstance.BubblerParameter.Tare;
                    partInstance.BubblerParameter.GrossWeight = this.SelectedPartInstance.BubblerParameter.GrossWeight;
                    if (this.SelectedPartInstance.CostReported) {
                        partInstance.UnitCost = this.SelectedPartInstance.UnitCost;
                        partInstance.TotalCost = partInstance.UnitCost * partInstance.BubblerParameter.NetWeight;
                    }
                } else {
                    partInstance.Quantity = this.SelectedPartInstance.Quantity;
                    partInstance.UnitCost = this.SelectedPartInstance.UnitCost;
                    partInstance.TotalCost = this.SelectedPartInstance.Quantity * this.SelectedPartInstance.UnitCost;

                }
                this._context.Entry<PartInstance>(partInstance).State = EntityState.Modified;
                try {
                    await this._context.SaveChangesAsync();
                    this.DispatcherService.BeginInvoke(() => {
                        this.MessageBoxService.ShowMessage("Save Done, Reloading", "Saved", MessageButton.OK, MessageIcon.Information);
                    });
                    ReloadEventTraveler traveler = new ReloadEventTraveler() {
                        PartId = this.SelectedPartInstance.PartId,
                        PartInstanceId = this.SelectedPartInstance.Id
                    };
                    this._eventAggregator.GetEvent<ReloadEvent>().Publish(traveler);
                    this.SaveCancelVisibility = Visibility.Collapsed;
                    this.IsEdit = false;
                } catch {
                    this.DispatcherService.BeginInvoke(() => {
                        this.MessageBoxService.ShowMessage("Error Save Part Instance", "Error", MessageButton.OK, MessageIcon.Error);
                    });
                }
            }
        }

        public async Task DiscardHandler() {

        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) { 

        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var partInstance = navigationContext.Parameters[ParameterKeys.SelectedPartInstance] as PartInstance;
            if (partInstance is PartInstance) {
                this._isInitialized = false;
                this.SelectedPartInstance = partInstance;
                var edit = (bool)navigationContext.Parameters[ParameterKeys.IsEdit];
                this._isNew = (bool)navigationContext.Parameters[ParameterKeys.IsNew];
                this.IsBubbler = this.SelectedPartInstance.IsBubbler;
                this.IsNotBubbler = !this.IsBubbler;

                this.StockVisibility = (this._isBubbler) ? Visibility.Collapsed : Visibility.Visible;
                this.CostVisibility = (this.SelectedPartInstance.CostReported) ? Visibility.Visible : Visibility.Collapsed;
                this.WeightVisibility = (this._isBubbler) ? Visibility.Visible : Visibility.Collapsed;
                this.SaveCancelVisibility = (edit || this._isNew) ? Visibility.Visible : Visibility.Collapsed;

                this.IsEdit = edit;
            }
        }
    }
}
