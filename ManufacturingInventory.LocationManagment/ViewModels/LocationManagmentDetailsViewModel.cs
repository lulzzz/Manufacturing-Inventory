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

        private bool _partInstancesEnabled;
        private bool _canEditDefault;
        private bool _showTableLoading;

        private bool _canEdit;
        private bool _isNew;
        private bool _isEdit;    

        private int _locationId;
        private bool _isLoaded=false;

        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand LocationTypeChangedCommand { get; private set; }

        public override bool KeepAlive => false;

        public LocationDto SelectedLocation {
            get => this._selectedLocation;
            set => SetProperty(ref this._selectedLocation,value);
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

        public bool PartInstancesEnabled { 
            get => this._partInstancesEnabled; 
            set => SetProperty(ref this._partInstancesEnabled, value);
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
                await this._locationService.Load();
                if (this._isNew) {

                } else {
                    var location = await this._locationService.GetLocation(this._locationId);
                    if (location != null) {
                        if (location.LocationType == LocationType.Consumer) {
                            this.PartInstancesEnabled = true;
                            var partInstances = await this._locationService.GetLocationInstances(this._locationId);

                        }

                    } else {
                        this.DispatcherService.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Error: Location not found", "Error", MessageButton.OK, MessageIcon.Error);
                        });
                    }
                }
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var parameters = navigationContext.Parameters;
            this._isNew = Convert.ToBoolean(parameters[ParameterKeys.IsNew]);
            this._isEdit = Convert.ToBoolean(parameters[ParameterKeys.IsEdit]);
            if (!this._isNew) {
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
