using Prism.Regions;
using DevExpress.Mvvm;
using Prism.Events;
using System.Windows;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using System.Linq;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Application.Boundaries.PriceEdit;
using ManufacturingInventory.Common.Application.UI.ViewModels;
using ManufacturingInventory.Application.Boundaries.AttachmentsEdit;
using ManufacturingInventory.Common.Application.ValueConverters;
using System.IO;
using System.Collections.Generic;
using ManufacturingInventory.Application.Boundaries.DistributorManagment;
using ManufacturingInventory.DistributorManagment.Internal;

namespace ManufacturingInventory.DistributorManagment.ViewModels {
    public class DistributorNavigationViewModel : InventoryViewModelBase {
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("DistributorDispatcherService"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("DistributorMessageBoxService"); }

        private IDistributornNavigationUseCase _distributorNavigation;
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;

        private ObservableCollection<Distributor> _distributors;
        private Distributor _selectedDistributor;
        private bool _showTableLoading;

        private bool _editInProgress;
        

        public AsyncCommand AddNewDistributorCommand { get; private set; }
        public AsyncCommand EditDistributorCommand { get; private set; }
        public AsyncCommand ViewDistributorDetailsCommand { get; private set; }
        public AsyncCommand DeleteDistributorCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand DoubleClickViewCommand { get; private set; }


        public DistributorNavigationViewModel(IDistributornNavigationUseCase distributorNavigation,IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._distributorNavigation = distributorNavigation;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.Load);
            this.AddNewDistributorCommand = new AsyncCommand(this.AddNewDistributorHandler, () => !this._editInProgress);
            this.EditDistributorCommand = new AsyncCommand(this.EditDistributorHandler, () => !this._editInProgress);
            this.ViewDistributorDetailsCommand = new AsyncCommand(this.ViewDistributorDetailsHandler, () => !this._editInProgress);
            this.DoubleClickViewCommand = new AsyncCommand(this.ViewDistributorDetailsHandler,()=>!this._editInProgress);
            this._eventAggregator.GetEvent<DistributorEditDoneEvent>().Subscribe(async (id) => await this.DistributorEditDoneHandler(id));
            this._eventAggregator.GetEvent<DistributorEditCancelEvent>().Subscribe(async (id) => await this.DistributorEditCancelHandler(id));
        }

        public override bool KeepAlive => false;

        public ObservableCollection<Distributor> Distributors { 
            get => this._distributors;
            set => SetProperty(ref this._distributors, value);
        }

        public Distributor SelectedDistributor {
            get => this._selectedDistributor;
            set => SetProperty(ref this._selectedDistributor, value);
        }

        public bool ShowTableLoading { 
            get => this._showTableLoading;
            set => SetProperty(ref this._showTableLoading, value);
        }

        private async Task ViewDistributorDetailsHandler() {
            await Task.Run(() => {
                if (this.SelectedDistributor != null) {
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add(ParameterKeys.SelectedDistributorId, this.SelectedDistributor.Id);
                    parameters.Add(ParameterKeys.IsEdit, false);
                    parameters.Add(ParameterKeys.IsNew, false);
                    this.DispatcherService.BeginInvoke(() => {
                        this.CleaupRegions();
                        this._regionManager.RequestNavigate(LocalRegions.DistributorDetailsRegion, ModuleViews.DistributorDetailsView, parameters);
                    });
                }
            });
        }

        private async Task AddNewDistributorHandler() {
            await Task.Run(() => {
                this._editInProgress = true;
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add(ParameterKeys.IsNew, true);
                this.DispatcherService.BeginInvoke(() => {
                    this.CleaupRegions();
                    this._regionManager.RequestNavigate(LocalRegions.DistributorDetailsRegion, ModuleViews.DistributorDetailsView, parameters);
                });
            });
        }

        private async Task EditDistributorHandler() {
            await Task.Run(() => {
                if (this.SelectedDistributor != null) {
                    this._editInProgress = true;
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add(ParameterKeys.SelectedDistributorId, this.SelectedDistributor.Id);
                    parameters.Add(ParameterKeys.IsEdit, true);
                    parameters.Add(ParameterKeys.IsNew, false);
                    this.DispatcherService.BeginInvoke(() => {
                        this.CleaupRegions();
                        this._regionManager.RequestNavigate(LocalRegions.DistributorDetailsRegion, ModuleViews.DistributorDetailsView, parameters);
                    });
                }
            });
        }

        private Task DeleteDisitributorHandler() {
            return Task.CompletedTask;
        }

        private async Task DistributorEditDoneHandler(int? distributorId) {
            this._editInProgress = false;
            await this.Reload(distributorId);
        }

        private async Task DistributorEditCancelHandler(int? distributorId) {
            this._editInProgress = false;
            await this.Reload(distributorId);
        }

        private void CleaupRegions() {
            this._regionManager.Regions[LocalRegions.DistributorDetailsRegion].RemoveAll();
        }

        private async Task Reload(int? distributorId) {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            await this._distributorNavigation.Load();
            var distributors = await this._distributorNavigation.GetDistributors();
            this.DispatcherService.BeginInvoke(() => {
                this.Distributors = new ObservableCollection<Distributor>(distributors);
                if (distributorId.HasValue) {
                    this.SelectedDistributor = this.Distributors.FirstOrDefault(e => e.Id == distributorId);
                }
                this.ShowTableLoading=false;
            });
        }

        private async Task Load() {
            this.DispatcherService.BeginInvoke(() =>this.ShowTableLoading=true);
            var distributors =await this._distributorNavigation.GetDistributors();
            this.DispatcherService.BeginInvoke(() => {
                this.Distributors = new ObservableCollection<Distributor>(distributors);
                this.ShowTableLoading = false;
            });
        }
    }
}
