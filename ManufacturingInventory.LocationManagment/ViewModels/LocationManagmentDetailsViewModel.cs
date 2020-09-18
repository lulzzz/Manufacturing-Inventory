using DevExpress.Mvvm;
using ManufacturingInventory.Application.Boundaries;
using ManufacturingInventory.Application.Boundaries.CategoryBoundaries;
using ManufacturingInventory.LocationManagment.Internal;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Application.UI.ViewModels;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Domain.Enums;
using ManufacturingInventory.Domain.Extensions;
using ManufacturingInventory.Infrastructure.Model.Entities;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ManufacturingInventory.Application.Boundaries.LocationManage;

namespace ManufacturingInventory.LocationManagment.ViewModels {
    public class LocationManagmentDetailsViewModel : InventoryViewModelNavigationBase {

        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("LocationDetailsDispatcher"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("LocationDetailsMessageBox"); }

        private ILocationManagmentUseCase _locationService;
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;

        private string _locationTypeName;
        private string _name;
        private string _description;
        private bool _isDefault;
        private LocationDto _selectedLocation;
        private LocationType _selectedLocationType;
        private bool _showViewLoading;
        private ObservableCollection<PartDto> _parts;
        private ObservableCollection<InstanceDto> _partInstances;
        private ObservableCollection<TransactionDTO> _transactions;

        private bool _partsEnabled;
        private bool _canEditDefault;
        private bool _canChangeType;
        private bool _showTableLoading;

        private bool _canEdit;
        private bool _isNew;
        private bool _isEdit;    

        private int _locationId;
        private bool _isLoaded=false;

        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }
        //public AsyncCommand LocationTypeChangedCommand { get; private set; }

        public LocationManagmentDetailsViewModel(IEventAggregator eventAggregator,IRegionManager regionManager,ILocationManagmentUseCase locationService) {
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._locationService = locationService;

            this.SaveCommand = new AsyncCommand(this.SaveHandler);
            this.CancelCommand = new AsyncCommand(this.CancelHandler);
            this.InitializeCommand = new AsyncCommand(this.LoadAsync);
        }

        public override bool KeepAlive => false;

        public LocationDto SelectedLocation {
            get => this._selectedLocation;
            set => SetProperty(ref this._selectedLocation,value);
        }

        public ObservableCollection<PartDto> Parts {
            get => this._parts;
            set => SetProperty(ref this._parts, value);
        }

        public ObservableCollection<InstanceDto> PartInstances {
            get => this._partInstances;
            set => SetProperty(ref this._partInstances, value);
        }

        public ObservableCollection<TransactionDTO> Transactions {
            get => this._transactions;
            set => SetProperty(ref this._transactions, value);
        }

        public string LocationTypeName { 
            get => this._locationTypeName; 
            set => SetProperty(ref this._locationTypeName, value); 
        }

        public string Name { 
            get => this._name; 
            set => SetProperty(ref this._name,value);
        }

        public string Description { 
            get => this._description;
            set => SetProperty(ref this._description, value);
        }

        public bool IsDefault { 
            get => this._isDefault; 
            set => SetProperty(ref this._isDefault, value);
        }

        public bool PartsEnabled { 
            get => this._partsEnabled; 
            set => SetProperty(ref this._partsEnabled, value);
        }

        public bool CanEditDefault { 
            get => this._canEditDefault; 
            set => SetProperty(ref this._canEditDefault, value);
        }

        public bool CanEdit { 
            get => this._canEdit; 
            set => SetProperty(ref this._canEdit, value);
        }

        public bool ShowTableLoading { 
            get => this._showTableLoading;
            set => SetProperty(ref this._showTableLoading, value);
        }

        public bool IsNew { 
            get => this._isNew;
            set => SetProperty(ref this._isNew, value);
        }

        public bool CanChangeType {
            get => this._canChangeType;
            set => SetProperty(ref this._canChangeType, value);
        }

        public LocationType SelectedLocationType {
            get => this._selectedLocationType;
            set => SetProperty(ref this._selectedLocationType, value);
        }

        public bool ShowViewLoading {
            get => this._showViewLoading;
            set => SetProperty(ref this._showViewLoading, value);
        }

        private async Task SaveHandler() {
            this._selectedLocation.Name = this.Name;
            this._selectedLocation.Description = this.Description;
            this._selectedLocation.LocationType = this.SelectedLocationType;
            this._selectedLocation.IsDefualt = this.IsDefault;
            EditAction action = (this.IsNew) ? EditAction.Add : EditAction.Update;
            LocationManagmentInput input = new LocationManagmentInput(this._selectedLocation, action);
            var output = await this._locationService.Execute(input);
            await this.ShowActionResponse(output);
        }

        private async Task CancelHandler() {
            this._eventAggregator.GetEvent<LocationEditCancelOrErrorEvent>().Publish();
        }

        private async Task ShowActionResponse(LocationManagmentOutput response) {
            await Task.Run(() => {
                if (response.Success) {
                    this.DispatcherService.BeginInvoke(() => {
                        this.MessageBoxService.ShowMessage(response.Message, "Success", MessageButton.OK, MessageIcon.Information);
                    });
                    this._eventAggregator.GetEvent<LocationEditDoneEvent>().Publish(this._locationId);
                } else {
                    this.DispatcherService.BeginInvoke(() => {
                        this.MessageBoxService.ShowMessage(response.Message, "Error", MessageButton.OK, MessageIcon.Error);
                    });
                }
            });
        }

        private async Task LoadAsync() {
            if (!this._isLoaded) {
                this.DispatcherService.BeginInvoke(()=>{ this.ShowViewLoading = true; });
                await this._locationService.Load();
                if (this.IsNew) {
                    this.SelectedLocation = new LocationDto();
                    this.PartsEnabled = false;
                    this.CanEdit = true;
                    this.IsDefault = false;
                    this.SelectedLocationType = LocationType.NotSelected;
                    this.CanChangeType = true;
                } else {
                    var location = await this._locationService.GetLocation(this._locationId);
                    if (location != null) {
                        this.CanEdit = this._isEdit;
                        this.CanChangeType = false;
                        this.PartsEnabled = location.LocationType == LocationType.Warehouse;
                        var transactions = await this._locationService.GetLocationTransactions(this._locationId);
                        var partInstances = await this._locationService.GetLocationInstances(this._locationId);
                        this.PartInstances = new ObservableCollection<InstanceDto>(partInstances);
                        this.Transactions = new ObservableCollection<TransactionDTO>(transactions);
                        if (this.PartsEnabled) {
                            var parts = await this._locationService.GetLocationParts(this._locationId);
                            this.Parts = new ObservableCollection<PartDto>(parts);
                        }
                        this.SelectedLocation = location;
                        this.IsDefault = this.SelectedLocation.IsDefualt;
                        this.SelectedLocationType = this.SelectedLocation.LocationType;
                        this.Name = this.SelectedLocation.Name;
                        this.Description = this.SelectedLocation.Description;
                    } else {
                        this.DispatcherService.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Error: Location not found", "Error", MessageButton.OK, MessageIcon.Error);
                        });
                        this._eventAggregator.GetEvent<LocationEditCancelOrErrorEvent>().Publish();
                    }
                }
                this.DispatcherService.BeginInvoke(() => { this.ShowViewLoading = false; });
                this._isLoaded = true;
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var parameters = navigationContext.Parameters;
            this.IsNew = Convert.ToBoolean(parameters[ParameterKeys.IsNew]);
            this._isEdit = Convert.ToBoolean(parameters[ParameterKeys.IsEdit]);
            if (!this.IsNew) {
                this._locationId = Convert.ToInt32(parameters[ParameterKeys.SelectedLocationId]);
            }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }
    }
}
