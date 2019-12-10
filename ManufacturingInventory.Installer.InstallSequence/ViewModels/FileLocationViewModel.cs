using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Mvvm;
using ManufacturingInventory.InstallSequence.Infrastructure;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using PrismCommands = Prism.Commands;

namespace ManufacturingInventory.InstallSequence.ViewModels {
    public class FileLocationViewModel : InventoryViewModelNavigationBase {
        private string _installLocation;
        private IRegionManager _regionManager;
        private IRegionNavigationJournal _journal;
        private IEventAggregator _eventAggregator;

        protected IFolderBrowserDialogService FolderBrowser { get => ServiceContainer.GetService<IFolderBrowserDialogService>("FolderBrowserDialog"); }
        public PrismCommands.DelegateCommand ChangeInstallLocationCommand { get; private set; }
        public PrismCommands.DelegateCommand BackCommand { get; private set; }
        public PrismCommands.DelegateCommand NextCommand { get; private set; }
        public PrismCommands.DelegateCommand CancelCommand { get; private set; }

        public FileLocationViewModel(IRegionManager regionManager,IEventAggregator eventAggregator) {
            this.InstallLocation = Constants.InstallLocationDefault;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.ChangeInstallLocationCommand = new PrismCommands.DelegateCommand(this.ChangeFolder);
            this.NextCommand = new PrismCommands.DelegateCommand(this.GoForward);
            this.CancelCommand = new PrismCommands.DelegateCommand(this.Cancel);
        }

        public string InstallLocation {
            get => this._installLocation;
            set => SetProperty(ref this._installLocation, value);
        }

        public override bool KeepAlive {
            get => false;
        }

        private void ChangeFolder() {
            this.FolderBrowser.StartPath = this.InstallLocation;
            if (this.FolderBrowser.ShowDialog()) {
                this.InstallLocation = FolderBrowser.ResultPath;
            }
        }

        private void Cancel() {
            this._eventAggregator.GetEvent<CancelEvent>().Publish();
        }

        private void GoForward() {
            var parameters = new NavigationParameters();
            parameters.Add("InstallLocation", this.InstallLocation);
            this._regionManager.RequestNavigate("ContentRegion","InstallingView", parameters);
            this._journal.GoForward();
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            this._journal = navigationContext.NavigationService.Journal;
        }
    }
}
