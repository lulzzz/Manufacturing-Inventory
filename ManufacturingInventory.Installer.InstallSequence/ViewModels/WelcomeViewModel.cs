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
    public class WelcomeViewModel: InventoryViewModelNavigationBase {
        private IRegionManager _regionManager;
        private IRegionNavigationJournal _journal;
        private IEventAggregator _eventAggregator;
        public PrismCommands.DelegateCommand NextCommand { get; private set; }
        public PrismCommands.DelegateCommand CancelCommand { get; private set; }

        public WelcomeViewModel(IRegionManager regionManager, IEventAggregator eventAggregator) {
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.NextCommand = new PrismCommands.DelegateCommand(this.GoFoward);
            this.CancelCommand = new PrismCommands.DelegateCommand(this.Cancel);
        }

        public override bool KeepAlive {
            get => false;
        }

        private void GoFoward() {
            this._regionManager.RequestNavigate("ContentRegion", "FileLocationView");
            this._journal.GoForward();
        }


        private bool CanGoForward() {
            return this._journal != null && this._journal.CanGoForward;
        }

        private void Cancel() {
            this._eventAggregator.GetEvent<CancelEvent>().Publish();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            this._journal = navigationContext.NavigationService.Journal;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {
            
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

    }
}
