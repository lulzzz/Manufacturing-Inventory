using Prism.Commands;
using Prism.Mvvm;
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

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class PartsDetailViewModel:InventoryViewModelNavigationBase {

        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("PartDetailsNotifications"); } }
        public IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("PartDetailsDispatcher"); } }
        //public IDialogService FileNameDialog { get { return ServiceContainer.GetService<IDialogService>("FileNameDialog"); } }
        public IOpenFileDialogService OpenFileDialogService { get { return ServiceContainer.GetService<IOpenFileDialogService>("OpenFileDialog"); } }
        public ISaveFileDialogService SaveFileDialogService { get { return ServiceContainer.GetService<ISaveFileDialogService>("SaveFileDialog"); } }

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private ManufacturingContext _context;

        private int _selectedTabIndex = 0;
        private Visibility _visibility;
        private bool _isNewPart = false;
        private bool _isEdit = false;
        private bool _isNotBubbler;
        private bool _isBubbler;

        private Part _selectedPart;

        public AsyncCommand UndoTransactionCommand { get; set; }
        public AsyncCommand ViewInstanceDetailsCommand { get; private set; }
        public AsyncCommand LoadCommand { get; private set; }

        public PrismCommands.DelegateCommand EditInstanceCommand { get; private set; }
        public PrismCommands.DelegateCommand ViewInstanceCommand { get; private set; }

        public PartsDetailViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, ManufacturingContext context) {
            this._context = context;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            //this._eventAggregator.GetEvent<LoadPartDetailsEvent>().Subscribe(async ()=>await this.LoadAsync(),ThreadOption.BackgroundThread);
            this.SelectedTabIndex = 0;
        }

        public override bool KeepAlive => false;

        public Part SelectedPart {
            get => this._selectedPart;
            set => SetProperty(ref this._selectedPart, value);
        }


        public Visibility Visibility {
            get => this._visibility;
            set => SetProperty(ref this._visibility, value);
        }

        public int SelectedTabIndex {
            get => this._selectedTabIndex;
            set => SetProperty(ref this._selectedTabIndex, value);
        }


        public bool IsNotBubbler { 
            get => this._isNotBubbler;
            set => SetProperty(ref this._isNotBubbler, value);
        }

        public bool IsBubbler { 
            get => this._isBubbler;
            set => SetProperty(ref this._isBubbler, value);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var part = navigationContext.Parameters[ParameterKeys.SelectedPart] as Part;
            var isNew = Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsNew]);
            var isEdit = Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsEdit]);
            if (part is Part) {
                this.SelectedPart = part;
                this._isEdit = isEdit;
                this._isNewPart = isNew;
                this.Visibility = (isEdit || isNew) ? Visibility.Visible : Visibility.Collapsed;
                this.IsBubbler = this.SelectedPart.HoldsBubblers;
                this.IsNotBubbler = !this.IsBubbler;
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
