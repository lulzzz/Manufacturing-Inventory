using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using PrismCommands = Prism.Commands;

namespace ManufacturingInventory.Common.Application.UI.ViewModels {
    public class ProgressViewModel:InventoryViewModelBase {
        private bool _isIndeterminate;
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private int _itemCount;
        private int _maxProgress;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private string _progressLabel;

        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("DispatcherService"); }

        public ProgressViewModel(IEventAggregator eventAggregator,IRegionManager regionManager) {
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            //this._eventAggregator.GetEvent<IncrementProgress>().Subscribe(this.IncrementProgressHandler);
        }

        public string ProgressLabel {
            get => this._progressLabel;
            set => SetProperty(ref this._progressLabel, value);
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
            set => SetProperty(ref this._maxProgress, value);
        }

        public override bool KeepAlive => false;

        public Task StartHandler() {
            return Task.CompletedTask;
        }

        private void IncrementProgressHandler(string logLine) {
            this.ItemCount = this.ItemCount + 1;
            string line = DateTime.Now + ": " + logLine + Environment.NewLine;
        }

    }
}
