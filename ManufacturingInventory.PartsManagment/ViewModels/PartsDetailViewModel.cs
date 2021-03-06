﻿using DevExpress.Mvvm;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Application.UI.Services;
using ManufacturingInventory.Domain.Enums;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.PartsManagment.Internal;
using Prism.Events;
using Prism.Regions;
using System;
using System.Windows;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class PartsDetailViewModel:InventoryViewModelNavigationBase {

        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("PartDetailsNotifications"); } }
        public IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("PartDetailsDispatcher"); } }

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;

        private int _selectedTabIndex = 0;
        private Visibility _visibility;
        private bool _isBubbler;

        private Part _selectedPart;
        private DataTraveler _partDataTraveler;
        private AttachmentDataTraveler _attachmentDataTraveler;


        public PartsDetailViewModel(IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this.SelectedTabIndex = 0;
            this._eventAggregator.GetEvent<ChangeSelectedTab>().Subscribe(this.SelectTabIndexHandler);
        }

        public override bool KeepAlive => false;

        public Part SelectedPart {
            get => this._selectedPart;
            set => SetProperty(ref this._selectedPart, value);
        }

        public DataTraveler PartDataTraveler { 
            get => this._partDataTraveler;
            set => SetProperty(ref this._partDataTraveler, value);
        }

        public AttachmentDataTraveler AttachmentDataTraveler {
            get => this._attachmentDataTraveler;
            set => SetProperty(ref this._attachmentDataTraveler, value);
        }

        public Visibility Visibility {
            get => this._visibility;
            set => SetProperty(ref this._visibility, value);
        }

        public int SelectedTabIndex {
            get => this._selectedTabIndex;
            set => SetProperty(ref this._selectedTabIndex, value);
        }

        public bool IsBubbler { 
            get => this._isBubbler;
            set => SetProperty(ref this._isBubbler, value);
        }

        public void SelectTabIndexHandler(int tabIndex) {
            this.SelectedTabIndex = tabIndex;
        }


        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var part = navigationContext.Parameters[ParameterKeys.SelectedPart] as Part;
            if (part is Part) {
                var isNew = Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsNew]);
                var isEdit = Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsEdit]);
                this.SelectedPart = part;
                this.Visibility = (isEdit || isNew) ? Visibility.Visible : Visibility.Collapsed;
                this.IsBubbler = this.SelectedPart.HoldsBubblers;
                this.PartDataTraveler = new DataTraveler() { PartId = this.SelectedPart.Id, HoldsBubblers = this.SelectedPart.HoldsBubblers,IsNew=isNew,IsEdit=isEdit };
                this.AttachmentDataTraveler = new AttachmentDataTraveler(GetAttachmentBy.PART, this.SelectedPart.Id);
            }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            var part = navigationContext.Parameters[ParameterKeys.SelectedPart] as Part;
            if (part is Part) {
                return this.SelectedPart != null && this.SelectedPart.Id != part.Id;
            } else {
                return true;
            }
        }
        
        public override void OnNavigatedFrom(NavigationContext navigationContext) { 
        
        }
    }
}
