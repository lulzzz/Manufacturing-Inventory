using Prism.Commands;
using Prism.Mvvm;
using DevExpress.Xpf.Core;
using ManufacturingInventory.Common.Application;
using Prism.Regions;
using DevExpress.Mvvm;
using PrismCommands = Prism.Commands;
using System;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    class PartsManagmentMainViewModel : InventoryViewModelBase {
        public override bool KeepAlive => false;
    }
}
