using DevExpress.Mvvm;
using ManufacturingInventory.Application.Boundaries.AlertManagmentUseCase;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Domain.Security.Interfaces;
using ManufacturingInventory.AlertManagment.Internal;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ManufacturingInventory.AlertManagment.ViewModels {
    class AlertsAvailableViewModel : InventoryViewModelBase {
        public override bool KeepAlive => throw new System.NotImplementedException();

        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("AlertsAvailableDispatcher"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("AlertsAvailableMessageBox"); }

        private IAlertsAvailableUseCase _alertService;
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IUserService _userService;

        private ObservableCollection<AlertDto> _availableAlerts;
        private AlertDto _selectedAlert;
        private bool _isLoaded = false;
        private bool _showTableLoading;
        
        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand SubscribeCommand { get; private set; }


        public AlertsAvailableViewModel(IEventAggregator eventAggregator,IRegionManager regionManager,IAlertsAvailableUseCase alertService,IUserService userService) {
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this._alertService = alertService;
            this._userService = userService;
            this._eventAggregator.GetEvent<ReloadEvent>().Subscribe(async () => await this.ReloadAsync());
            this.InitializeCommand = new AsyncCommand(this.LoadAsync);
            this.SubscribeCommand = new AsyncCommand(this.SubscribeHandler);
        }

        public ObservableCollection<AlertDto> AvailableAlerts { 
            get => this._availableAlerts; 
            set => SetProperty(ref this._availableAlerts,value);
        }

        public AlertDto SelectedAlert { 
            get => this._selectedAlert;
            set => SetProperty(ref this._selectedAlert, value);
        }

        public bool ShowTableLoading { 
            get => this._showTableLoading; 
            set => SetProperty(ref this._showTableLoading, value);
        }


        private async Task SubscribeHandler() {
            AlertUseCaseInput input = new AlertUseCaseInput(this._userService.CurrentUserId, this.SelectedAlert, AlertAction.Subscribe);
            var output = await this._alertService.Execute(input);
            if (output.Success) {
                this.MessageBoxService.ShowMessage(output.Message, "Success", MessageButton.OK, MessageIcon.Information);
                this._eventAggregator.GetEvent<ReloadEvent>().Publish();
            } else {
                this.MessageBoxService.ShowMessage(output.Message, "Error", MessageButton.OK, MessageIcon.Error);
            }
        }

        private async Task ReloadAsync() {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            await this._alertService.Load();
            var alerts = await this._alertService.GetAvailableAlerts(this._userService.CurrentUserId);
            this.AvailableAlerts = new ObservableCollection<AlertDto>(alerts);
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = false);
        }

        private async Task LoadAsync() {
            if (!this._isLoaded) {
                this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
                await this._alertService.Load();
                var alerts = await this._alertService.GetAvailableAlerts(this._userService.CurrentUserId);
                this.AvailableAlerts = new ObservableCollection<AlertDto>(alerts);
                this._isLoaded = true;
                this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = false);
            }
        }
    }
}
