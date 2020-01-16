using Prism.Commands;
using Prism.Mvvm;
using DevExpress.Xpf.Core;
using ManufacturingInventory.Common.Application;
using Prism.Regions;
using DevExpress.Mvvm;
using PrismCommands = Prism.Commands;
using System;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ManufacturingInventory.PartsManagment.Internal;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ManufacturingInventory.Infrastructure.Model.Entities;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class PartsNavigationViewModel : InventoryViewModelBase {
        protected IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("PartsNavigationNotifications"); } }
        protected IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("PartsNavigationDispatcher"); } }

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        //private ManufacturingContext _context;
        IPartManagerService _partManager;

        private ObservableCollection<Part> _parts = new ObservableCollection<Part>();
        private Part _selectedPart = new Part();
        private bool _isLoading = false;
        private bool _isInitialized = false;
        private bool _editInProgress = false;

        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand RefreshDataCommand { get; private set; }
        public PrismCommands.DelegateCommand ViewPartDetailsCommand { get; private set; }
        public PrismCommands.DelegateCommand EditPartCommand { get; private set; }

        public PartsNavigationViewModel(IPartManagerService partManager,IEventAggregator eventAggregator,IRegionManager regionManager) {
            this._partManager = partManager; 
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this.InitializeCommand = new AsyncCommand(this.PopulateAsync);
            this.RefreshDataCommand = new AsyncCommand(this.RefreshDataHandler);
            this.ViewPartDetailsCommand = new PrismCommands.DelegateCommand(this.ViewPartDetailsHandler);
            this.EditPartCommand = new PrismCommands.DelegateCommand(this.EditPartDetailsHandler);
        }

        public override bool KeepAlive => false;

        public ObservableCollection<Part> Parts {
            get => this._parts;
            set => SetProperty(ref this._parts, value, "Parts");
        }

        public Part SelectedPart {
            get => this._selectedPart;
            set => SetProperty(ref this._selectedPart, value, "SelectedPart");
        }

        public bool IsLoading {
            get => this._isLoading;
            set => SetProperty(ref this._isLoading, value);
        }

        private void ViewPartDetailsHandler() {
            if (this.SelectedPart != null) {
                this.CleanupRegions();
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add(ParameterKeys.SelectedPart, this.SelectedPart);
                parameters.Add(ParameterKeys.IsEdit, false);
                parameters.Add(ParameterKeys.IsNew, false);
                this._regionManager.RequestNavigate(LocalRegions.PartDetailsRegion, ModuleViews.PartsDetailView, parameters);
            }
        }

        private void EditPartDetailsHandler() {
            if (this.SelectedPart != null) {
                this._editInProgress = true;
                this.CleanupRegions();
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add(ParameterKeys.SelectedPart, this.SelectedPart);
                parameters.Add(ParameterKeys.IsNew, false);
                parameters.Add(ParameterKeys.IsEdit, true);
                this._regionManager.RequestNavigate(LocalRegions.PartDetailsRegion, ModuleViews.PartsDetailView, parameters);
            }
        }

        private void NewPartHandler() {
            if (this.SelectedPart != null) {
                this._editInProgress = true;
                this.CleanupRegions();
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add(ParameterKeys.SelectedPart, this.SelectedPart);
                parameters.Add(ParameterKeys.IsNew, true);
                parameters.Add(ParameterKeys.IsEdit, false);
                this._regionManager.RequestNavigate(LocalRegions.PartDetailsRegion, ModuleViews.PartsDetailView, parameters);
            }
        }

        private async Task RefreshDataHandler() {
            this.DispatcherService.BeginInvoke(() => {
                this.IsLoading = true;
                this.CleanupRegions();
            });
            await this._partManager.LoadAllAsync();
            var tempParts = await this._partManager.PartService.GetEntityListAsync();
            //var tempParts = await this._context.Parts
            //    .AsNoTracking()
            //    .Include(e => e.Organization)
            //    .Include(e => e.Warehouse)
            //    .Include(e => e.Usage).ToListAsync();

            this.DispatcherService.BeginInvoke(() => {
                this.Parts = new ObservableCollection<Part>(tempParts);
                this.IsLoading = false;
            });
        }

        private async Task PopulateAsync() {
            if (!this._isInitialized) {
                this.DispatcherService.BeginInvoke(() => {
                    this.IsLoading = true;
                    this.CleanupRegions();
                });

                var tempParts = await this._partManager.PartService.GetEntityListAsync();

                this.DispatcherService.BeginInvoke(() => {
                    this.Parts = new ObservableCollection<Part>(tempParts);
                    this.IsLoading = false;
                });
                this._isInitialized = true;
            }
        }

        public void CleanupRegions() {
            this._regionManager.Regions[LocalRegions.PartDetailsRegion].RemoveAll();
            this._regionManager.Regions[LocalRegions.DetailsRegion].RemoveAll();
            this._regionManager.Regions.Remove(LocalRegions.AttachmentTableRegion);
            this._regionManager.Regions.Remove(LocalRegions.PartInstanceTableRegion);
            this._regionManager.Regions.Remove(LocalRegions.TransactionTableRegion);
            this._regionManager.Regions.Remove(LocalRegions.PartSummaryRegion);
            //if (this._regionManager.Regions.ContainsRegionWithName(LocalRegions.AttachmentTableRegion)) {
            //    this._regionManager.Regions[LocalRegions.AttachmentTableRegion].RemoveAll();
            //}

            //if (this._regionManager.Regions.ContainsRegionWithName(LocalRegions.PartInstanceTableRegion)) {
            //    this._regionManager.Regions[LocalRegions.PartInstanceTableRegion].RemoveAll();
            //}

            //if (this._regionManager.Regions.ContainsRegionWithName(LocalRegions.TransactionTableRegion)) {
            //    this._regionManager.Regions[LocalRegions.TransactionTableRegion].RemoveAll();
            //}

            //if (this._regionManager.Regions.ContainsRegionWithName(LocalRegions.PartSummaryRegion)) {
            //    this._regionManager.Regions[LocalRegions.PartSummaryRegion].RemoveAll();
            //}
        }
    }
}
