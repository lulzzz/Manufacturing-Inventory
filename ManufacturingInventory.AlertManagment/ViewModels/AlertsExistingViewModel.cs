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
    public class AlertsExistingViewModel : InventoryViewModelBase {
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("AlertsExistingDispatcher"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("AlertsExistingMessageBox"); }

        private IAlertsExistingUseCase _alertService;
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private IUserService _userService;

        private bool _isLoaded = false;
        private ObservableCollection<AlertDto> _exisitingAlerts;
        private AlertDto _selectedAlert;
        private bool _showTableLoading;

        public AsyncCommand ToggleEnableCommand { get; private set; }
        public AsyncCommand UnsubscribeCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }

        public AlertsExistingViewModel(IEventAggregator eventAggregator,IRegionManager regionManager,IAlertsExistingUseCase alertService, IUserService userService) {
            this._userService = userService;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._alertService = alertService;
            this._eventAggregator.GetEvent<ReloadEvent>().Subscribe(async () => { await this.ReloadAsync();});
            this.ToggleEnableCommand = new AsyncCommand(this.ToggleEnableHandler);
            this.UnsubscribeCommand = new AsyncCommand(this.UnsubscribeHandler);
            this.InitializeCommand = new AsyncCommand(this.LoadAsync);
        }

        public override bool KeepAlive => false;

        public ObservableCollection<AlertDto> ExisitingAlerts { 
            get => this._exisitingAlerts;
            set => SetProperty(ref this._exisitingAlerts, value);
        }

        public AlertDto SelectedAlert {
            get => this._selectedAlert;
            set => SetProperty(ref this._selectedAlert, value);
        }

        public bool ShowTableLoading { 
            get => this._showTableLoading; 
            set => SetProperty(ref this._showTableLoading,value);
        }

        private async Task ToggleEnableHandler() {
            AlertUseCaseInput input = new AlertUseCaseInput(this._userService.CurrentUserId, this.SelectedAlert, AlertAction.ToggleEnable);
            var output = await this._alertService.Execute(input);
            if (output.Success) {
                this.MessageBoxService.ShowMessage(output.Message, "Success", MessageButton.OK, MessageIcon.Information);
                this._eventAggregator.GetEvent<ReloadEvent>().Publish();
            } else {
                this.MessageBoxService.ShowMessage(output.Message, "Error", MessageButton.OK, MessageIcon.Error);
            }
        }

        private async Task UnsubscribeHandler() {
            AlertUseCaseInput input = new AlertUseCaseInput(this._userService.CurrentUserId, this.SelectedAlert, AlertAction.UnSubscribe);
            var output = await this._alertService.Execute(input);
            if (output.Success) {
                this.MessageBoxService.ShowMessage(output.Message, "Success", MessageButton.OK, MessageIcon.Information);
                this._eventAggregator.GetEvent<ReloadEvent>().Publish();
            } else {
                this.MessageBoxService.ShowMessage(output.Message,"Error", MessageButton.OK, MessageIcon.Error);
            }
        }

        private async Task ReloadAsync() {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            await this._alertService.Load();
            var alerts = await this._alertService.GetExistingAlerts(this._userService.CurrentUserId);
            this.ExisitingAlerts = new ObservableCollection<AlertDto>(alerts);
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = false);
        }

        private async Task LoadAsync() {
            if (!this._isLoaded) {
                this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
                await this._alertService.Load();
                var alerts = await this._alertService.GetExistingAlerts(this._userService.CurrentUserId);
                this.ExisitingAlerts = new ObservableCollection<AlertDto>(alerts);
                this._isLoaded = true;
                this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = false);
            }

        }

    }
}
