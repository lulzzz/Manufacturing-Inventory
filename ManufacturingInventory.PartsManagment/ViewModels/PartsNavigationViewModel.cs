using DevExpress.Mvvm;
using ManufacturingInventory.Application.Boundaries.PartNavigationEdit;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.PartsManagment.Internal;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PrismCommands = Prism.Commands;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class PartsNavigationViewModel : InventoryViewModelBase {
        protected IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("PartsNavigationNotifications"); } }
        protected IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("PartsNavigationDispatcher"); } }

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IPartNavigationEditUseCase _partEdit;

        private ObservableCollection<Part> _parts = new ObservableCollection<Part>();
        private Part _selectedPart = new Part();
        private bool _isLoading = false;
        private bool _isInitialized = false;
        private bool _editInProgress = false;

        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand RefreshDataCommand { get; private set; }
        public AsyncCommand NewPartCommand { get; private set; }
        public PrismCommands.DelegateCommand ViewPartDetailsCommand { get; private set; }
        public PrismCommands.DelegateCommand EditPartCommand { get; private set; }
        public PrismCommands.DelegateCommand DoubleClickViewCommand { get; private set; }

        public PartsNavigationViewModel(IPartNavigationEditUseCase partEdit, IEventAggregator eventAggregator,IRegionManager regionManager) {
            this._partEdit = partEdit;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this.InitializeCommand = new AsyncCommand(this.PopulateAsync);
            this.RefreshDataCommand = new AsyncCommand(this.RefreshDataHandler);
            this.ViewPartDetailsCommand = new PrismCommands.DelegateCommand(this.ViewPartDetailsHandler, this.CanNew);
            this.EditPartCommand = new PrismCommands.DelegateCommand(this.EditPartDetailsHandler, this.CanNew);
            this.NewPartCommand = new AsyncCommand(this.NewPartHandler,this.CanNew);
            this.DoubleClickViewCommand = new PrismCommands.DelegateCommand(this.ViewPartDetailsHandler,this.CanNew);

            this._eventAggregator.GetEvent<PartEditDoneEvent>().Subscribe(async (partId)=>await this.ReloadEditDoneHandler(partId));
            this._eventAggregator.GetEvent<PartEditCancelEvent>().Subscribe(async () => await this.ReloadEditCancelHandle());
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

        private async Task NewPartHandler() {
            await Task.Run(() => {
                if (this.SelectedPart != null) {
                    this._editInProgress = true;
                    Part part = new Part();
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add(ParameterKeys.SelectedPart, part);
                    parameters.Add(ParameterKeys.IsNew, true);
                    parameters.Add(ParameterKeys.IsEdit, false);
                    this.DispatcherService.BeginInvoke(() => {
                        this.CleanupRegions();
                        this._regionManager.RequestNavigate(LocalRegions.PartDetailsRegion, ModuleViews.PartsDetailView, parameters);
                    });
                }
            });
        }

        private bool CanNew() {
            return !this._editInProgress;
        }

        private async Task RefreshDataHandler() {
            this.DispatcherService.BeginInvoke(() => {
                this._editInProgress = false;
                this.IsLoading = true;
                this.CleanupRegions();
            });
            await this._partEdit.LoadAsync();
            var parts =await this._partEdit.GetPartsAsync();

            this.DispatcherService.BeginInvoke(() => {
                this.Parts = new ObservableCollection<Part>(parts);
                this.IsLoading = false;
            });
        }

        private async Task ReloadEditCancelHandle() {

            this.DispatcherService.BeginInvoke(() => {
                this._editInProgress = false;
                this.IsLoading = true;
                this.CleanupRegions();
            });
            await this._partEdit.LoadAsync();
            var parts = await this._partEdit.GetPartsAsync();

            this.DispatcherService.BeginInvoke(() => {
                this.Parts = new ObservableCollection<Part>(parts);
                this.IsLoading = false;
            });
        }

        private async Task ReloadEditDoneHandler(int partId) {

            this.DispatcherService.BeginInvoke(() => {
                this._editInProgress = false;
                this.IsLoading = true;
                this.CleanupRegions();
            });

            await this._partEdit.LoadAsync();
            var parts = await this._partEdit.GetPartsAsync();

            this.DispatcherService.BeginInvoke(() => {
                this.Parts = new ObservableCollection<Part>(parts);
                this.SelectedPart = this.Parts.FirstOrDefault(e => e.Id == partId);
                this.IsLoading = false;
            });
        }

        private async Task PopulateAsync() {
            if (!this._isInitialized) {
                this.DispatcherService.BeginInvoke(() => {
                    this.IsLoading = true;
                    this.CleanupRegions();
                });

                var parts = await this._partEdit.GetPartsAsync();

                this.DispatcherService.BeginInvoke(() => {
                    this.Parts = new ObservableCollection<Part>(parts);
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
            this._regionManager.Regions.Remove(LocalRegions.InstancePriceEditDetailsRegion);
            this._regionManager.Regions.Remove(LocalRegions.InstanceAttachmentRegion);
        }
    }
}
