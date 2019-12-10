using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using ManufacturingInventory.InstallSequence.Infrastructure;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using PrismCommands = Prism.Commands;


namespace ManufacturingInventory.InstallSequence.ViewModels {
    public class InstallingViewModel: InventoryViewModelNavigationBase {
        private string _installLocation;
        private string _installLog;
        private bool _isIndeterminate;
        private IRegionManager _regionManager;
        private IRegionNavigationJournal _journal;
        private IEventAggregator _eventAggregator;
        private IInstaller _installer;
        private int _itemCount;
        private int _maxProgress;

        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("DispatcherService"); }


        public PrismCommands.DelegateCommand BackCommand { get; private set; }
        public PrismCommands.DelegateCommand NextCommand { get; private set; }
        public PrismCommands.DelegateCommand CancelCommand { get; private set; }
        public AsyncCommand InstallCommand { get; private set; }

        public InstallingViewModel(IRegionManager regionManager, IEventAggregator eventAggregator,IInstaller installer) {
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._installer = installer;
            this.IsIndeterminate = false;
            this.ItemCount = 0;
            this.NextCommand = new PrismCommands.DelegateCommand(this.GoForward);
            this.BackCommand = new PrismCommands.DelegateCommand(this.GoBack);
            this.CancelCommand = new PrismCommands.DelegateCommand(this.Cancel);
            this.InstallCommand = new AsyncCommand(this.InstallHandler);
            this._eventAggregator.GetEvent<IncrementProgress>().Subscribe(this.IncrementProgressHandler);
        }

        public override bool KeepAlive {
            get => false;
        }

        public string InstallLog { 
            get => this._installLog;
            set => SetProperty(ref this._installLog, value); 
        }
        public bool IsIndeterminate { 
            get => this._isIndeterminate;
            set => SetProperty(ref this._isIndeterminate, value);
        }
        public int ItemCount { 
            get => this._itemCount; 
            set => SetProperty(ref this._itemCount, value);
        }

        public int MaxProgress { 
            get => this._maxProgress; 
            set => SetProperty(ref this._maxProgress,value);
        }

        private async Task InstallHandler() {
            this.MaxProgress = this._installer.CalculateWork();
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            bool success;
            if (!string.IsNullOrEmpty(this._installLocation)) {
                success = await this._installer.InstallAsync(token);
            } else {
                success = await this._installer.InstallAsync(token,this._installLocation);
            }
            this.InstallLog = (success) ? "Install Complete" : "Install Failed";
        }

        private void IncrementProgressHandler() {
            this.DispatcherService.BeginInvoke(()=> this.ItemCount = this.ItemCount + 1);
        }

        private void Cancel() {
            this._eventAggregator.GetEvent<CancelEvent>().Publish();
        }

        private void GoBack() {
            _journal.GoBack();
        }

        private void GoForward() {
            var parameters = new NavigationParameters();
            parameters.Add("Success", true);
            this._regionManager.RequestNavigate("ContentRegion", "FinishedView", parameters);
            this._journal.GoForward();
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) { 
        
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            this._journal = navigationContext.NavigationService.Journal;
            var installLocation = navigationContext.Parameters["InstallLocation"] as string;
            this._installLocation = installLocation;
        }
    }
}
