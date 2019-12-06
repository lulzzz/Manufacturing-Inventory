using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Mvvm;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.InstallSequence.Infrastructure;
using Prism.Commands;
using Prism.Regions;
using Prism.Events;
using PrismCommands = Prism.Commands;

namespace ManufacturingInventory.InstallSequence.ViewModels {
    public class FinishedViewModel: InventoryViewModelNavigationBase {
        private string _finishedText;
        private string _finishedInstruction;
        private IRegionManager _regionManager;
        private IRegionNavigationJournal _journal;
        private IEventAggregator _eventAggregator;

        public PrismCommands.DelegateCommand FinishedCommand { get; private set; }

        public FinishedViewModel(IRegionManager regionManager, IEventAggregator eventAggregator) {
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.FinishedCommand = new PrismCommands.DelegateCommand(this.FinishedHandler);
        }

        public override bool KeepAlive {
            get => false;
        }

        public string FinishedText { 
            get => this._finishedText;
            set => SetProperty(ref this._finishedText, value); 
        }

        public string FinishedInstruction { 
            get => this._finishedInstruction; 
            set => SetProperty(ref this._finishedInstruction,value); 
        }

        public void FinishedHandler() {
            this._eventAggregator.GetEvent<FinishedEvent>().Publish();
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) { 
        
        }
        
        public override void OnNavigatedTo(NavigationContext navigationContext) {
            this._journal=navigationContext.NavigationService.Journal;

            var success = (bool)navigationContext.Parameters["Success"];
            if (success) {
                this.FinishedText = "Success!";
                this.FinishedInstruction = "Software is now installed and should be on start menu";
            } else {
                this.FinishedText = "Failed to Install";
                this.FinishedInstruction = "Please Contact Administrator";
            }
        }
    }
}
