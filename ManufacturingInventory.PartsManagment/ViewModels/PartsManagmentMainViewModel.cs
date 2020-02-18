using Prism.Commands;
using Prism.Mvvm;
using DevExpress.Xpf.Core;
using ManufacturingInventory.Common.Application;
using Prism.Regions;
using DevExpress.Mvvm;
using PrismCommands = Prism.Commands;
using System;
using Prism.Events;
using ManufacturingInventory.PartsManagment.Internal;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class PartsManagmentMainViewModel : InventoryViewModelBase {
        private IEventAggregator _eventAggregator;

        private string _detailHeaderText;

        public PartsManagmentMainViewModel(IEventAggregator eventAggregator) {
            this._eventAggregator = eventAggregator;
            this._eventAggregator.GetEvent<RenameHeaderEvent>().Subscribe((newHeader) => this.DetailHeaderText = newHeader);
        }

        public string DetailHeaderText {
            get => this._detailHeaderText;
            set => SetProperty(ref this._detailHeaderText, value);
        }

        public override bool KeepAlive => false;
    }
}
