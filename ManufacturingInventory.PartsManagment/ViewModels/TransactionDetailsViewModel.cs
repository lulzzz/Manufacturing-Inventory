using System;
using System.Collections.Generic;
using System.Text;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Model.Entities;
using ManufacturingInventory.PartsManagment.Internal;
using Prism.Events;
using Prism.Regions;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class TransactionDetailsViewModel : InventoryViewModelNavigationBase {

        private Transaction _selectedTransaction;
        private bool _isEdit;

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;

        public TransactionDetailsViewModel(IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
        }

        public override bool KeepAlive => false;

        public Transaction SelectedTransaction { 
            get => this._selectedTransaction;
            set => SetProperty(ref this._selectedTransaction, value); 
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }


        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var transaction = navigationContext.Parameters[ParameterKeys.SelectedTransaction] as Transaction;
            var isEdit = Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsEdit]);
            if (transaction is Transaction) {
                this.SelectedTransaction = transaction;
                this._isEdit = isEdit;
                //this.Visibility = (isEdit || isNew) ? Visibility.Visible : Visibility.Collapsed;
                //this._eventAggregator.GetEvent<LoadPartDetailsEvent>().Publish();
            }
        }
    }
}
