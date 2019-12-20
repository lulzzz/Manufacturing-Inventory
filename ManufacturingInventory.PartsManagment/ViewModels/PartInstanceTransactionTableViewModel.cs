using Prism.Commands;
using Prism.Mvvm;
using DevExpress.Xpf.Core;
using ManufacturingInventory.Common.Application;
using Prism.Regions;
using DevExpress.Mvvm;
using PrismCommands = Prism.Commands;
using Prism.Events;
using ManufacturingInventory.Common.Model;
using System.Windows;
using ManufacturingInventory.Common.Model.Entities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ManufacturingInventory.PartsManagment.Internal;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Prism;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class PartInstanceTransactionTableViewModel : InventoryViewModelBase{
        private ObservableCollection<Transaction> _transaction = new ObservableCollection<Transaction>();
        private IModuleCommands _moduleCommands;



        public PartInstanceTransactionTableViewModel(IModuleCommands moduleCommands) {
            this._moduleCommands = moduleCommands;
        }

        public override bool KeepAlive => throw new NotImplementedException();

        public ObservableCollection<Transaction> Transactions {
            get => this._transaction;
            set => SetProperty(ref this._transaction, value);
        }
    }
}
