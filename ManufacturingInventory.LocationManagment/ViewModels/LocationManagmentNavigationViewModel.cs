using DevExpress.Mvvm;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Application.Boundaries.CategoryBoundaries;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Common.Application.UI.ViewModels;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.LocationManagment;
using Serilog;
using ManufacturingInventory.Application.Boundaries;
using ManufacturingInventory.Application.Boundaries.LocationManage;
using ManufacturingInventory.LocationManagment.Internal;

namespace ManufacturingInventory.LocationManagment.ViewModels {
    public class LocationManagmentNavigationViewModel : InventoryViewModelBase {
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("LocationNavDispatcherService"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("LocationNavMessageBoxService"); }
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private ILocationManagmentUseCase _locationService;

        private ObservableCollection<LocationDto> _locations;
        private LocationDto _selectedLocation;
        private bool _showTableLoading;
        private bool _isLoaded=false;
        private bool _editInProgress=false;

        public AsyncCommand DeleteLocationCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand AddNewLocationCommand { get; private set; }
        public AsyncCommand ViewDetailsCommand { get; private set; }
        public AsyncCommand EditLocationCommand { get; private set; }
        public AsyncCommand DoubleClickViewCommand { get; private set; }

        public LocationManagmentNavigationViewModel(IEventAggregator eventAggregator,IRegionManager regionManager,ILocationManagmentUseCase locationService) {
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this._locationService = locationService;

            this.DeleteLocationCommand = new AsyncCommand(this.DeleteLocationHandler,!this._editInProgress);
            this.ViewDetailsCommand = new AsyncCommand(this.ViewDetailsHandler,!this._editInProgress);
            this.AddNewLocationCommand = new AsyncCommand(this.AddNewLocationHandler, !this._editInProgress);
            this.EditLocationCommand = new AsyncCommand(this.EditLocationHandler, !this._editInProgress);
            this.DoubleClickViewCommand = new AsyncCommand(this.ViewDetailsHandler, !this._editInProgress);
            this.InitializeCommand = new AsyncCommand(this.LoadHandler);
        }

        public override bool KeepAlive => false;

        public ObservableCollection<LocationDto> Locations {
            get => this._locations;
            set => SetProperty(ref this._locations, value);
        }

        public bool ShowTableLoading {
            get => this._showTableLoading;
            set => SetProperty(ref this._showTableLoading, value);
        }

        public LocationDto SelectedLocation { 
            get => _selectedLocation;
            set => SetProperty(ref this._selectedLocation, value);
        }

        public async Task AddNewLocationHandler() {
            if (this.SelectedLocation != null) {
                await Task.Run(() => {
                    this._editInProgress = true;
                    this.DispatcherService.BeginInvoke(() => {
                        this.NavigateDetails(false, true);
                    });               
                });
            }
        }

        public async Task DeleteLocationHandler() {
            LocationManagmentInput input = new LocationManagmentInput(this.SelectedLocation, EditAction.Delete);
            var response = await this._locationService.Execute(input);
            await this.ShowActionResponse(response);
        }

        public async Task EditLocationHandler() {
            if (this.SelectedLocation != null) {
                await Task.Run(() => {
                    this._editInProgress = true;
                    this.DispatcherService.BeginInvoke(() => {
                        this.NavigateDetails(true, false, this.SelectedLocation.Id);
                    });          
                });
            }
        }

        private void NavigateDetails(bool isEdit,bool isNew, int? locationId=null) {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add(ParameterKeys.SelectedLocationId, locationId);
            parameters.Add(ParameterKeys.IsEdit, isEdit);
            parameters.Add(ParameterKeys.IsNew, isNew);
            this._editInProgress = isEdit || isNew;
            this._regionManager.RequestNavigate(LocalRegions.LocationDetailsRegion, ModuleViews.LocationManagmentDetailsView, parameters);
        }
        
        public async Task ViewDetailsHandler() {
            if (this.SelectedLocation != null) {
                await Task.Run(() => {
                    this.DispatcherService.BeginInvoke(() => {
                        this.NavigateDetails(false, false, this.SelectedLocation.Id);
                    });
                });
            }
        }

        private async Task ShowActionResponse(LocationManagmentOutput response, bool wasDelete = false) {
            if (response.Success) {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Success", MessageButton.OK, MessageIcon.Information);
                });
                if (!wasDelete) {
                    await this.Reload(response.Location.Id);
                } else {
                    await this.Reload();
                }
            } else {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Error", MessageButton.OK, MessageIcon.Error);
                });
                await this.Reload();
            }
        }

        private async Task EditCompletedCallBack(int locationId) {
            this._editInProgress = false;
            await this.Reload(locationId);
        }

        private async Task EditCancelOrErrorCallBack() {
            await this.Reload();
        }

        private void CleanUpRegions() {
            this._regionManager.Regions[LocalRegions.LocationDetailsRegion].RemoveAll();
        }

        public async Task Reload(int? locationId=null) {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            await this._locationService.Load();
            var locations = await this._locationService.GetLocations();
            this.Locations = new ObservableCollection<LocationDto>(locations);
            if (locationId.HasValue) {
                this.SelectedLocation = this.Locations.FirstOrDefault(e => e.Id == locationId);
            }
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = false);
        }

        public async Task LoadHandler() {
            if (!this._isLoaded) {
                this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
                await this._locationService.Load();
                var locations = await this._locationService.GetLocations();
                this.Locations = new ObservableCollection<LocationDto>(locations);
                this._isLoaded = true;
                this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = false);
            }
        }

    }
}
