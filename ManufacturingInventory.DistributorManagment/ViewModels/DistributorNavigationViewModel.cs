using DevExpress.Mvvm;
using ManufacturingInventory.Application.Boundaries.DistributorManagment;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.DistributorManagment.Internal;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Common.Application.UI.ViewModels;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using ManufacturingInventory.Application.Boundaries;

namespace ManufacturingInventory.DistributorManagment.ViewModels {
    public class DistributorNavigationViewModel : InventoryViewModelBase {
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("DistributorDispatcherService"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("DistributorMessageBoxService"); }
        protected IDialogService NewDistributorDialog { get => ServiceContainer.GetService<IDialogService>("NewDistributorDialog"); }


        private IDistributorEditUseCase _distributorEdit;
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;

        private ObservableCollection<Distributor> _distributors;
        private Distributor _selectedDistributor;
        private bool _showTableLoading;
        private bool _isInitialized = false;

        private bool _editInProgress;
        private NewDistributorViewModel _newDistributorViewModel;

        public AsyncCommand AddNewDistributorCommand { get; private set; }
        public AsyncCommand EditDistributorCommand { get; private set; }
        public AsyncCommand ViewDistributorDetailsCommand { get; private set; }
        public AsyncCommand DeleteDistributorCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand DoubleClickViewCommand { get; private set; }


        public DistributorNavigationViewModel(IDistributorEditUseCase distributorEditUseCase,IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._distributorEdit = distributorEditUseCase;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.Load);
            this.AddNewDistributorCommand = new AsyncCommand(this.AddNewDistributorHandler, () => !this._editInProgress);
            this.EditDistributorCommand = new AsyncCommand(this.EditDistributorHandler, () => !this._editInProgress);
            this.ViewDistributorDetailsCommand = new AsyncCommand(this.ViewDistributorDetailsHandler, () => !this._editInProgress);
            this.DoubleClickViewCommand = new AsyncCommand(this.ViewDistributorDetailsHandler,()=>!this._editInProgress);
            this.DeleteDistributorCommand = new AsyncCommand(this.DeleteDistributorHandler, () => !this._editInProgress);
            this._eventAggregator.GetEvent<DistributorEditDoneEvent>().Subscribe(async (id) => await this.DistributorEditDoneHandler(id));
            this._eventAggregator.GetEvent<DistributorEditCancelEvent>().Subscribe(async (id) => await this.DistributorEditCancelHandler(id));
        }

        public override bool KeepAlive => false;

        public ObservableCollection<Distributor> Distributors { 
            get => this._distributors;
            set => SetProperty(ref this._distributors, value);
        }

        public Distributor SelectedDistributor {
            get => this._selectedDistributor;
            set => SetProperty(ref this._selectedDistributor, value);
        }

        public bool ShowTableLoading { 
            get => this._showTableLoading;
            set => SetProperty(ref this._showTableLoading, value);
        }





        #region Misc

        private async Task AddNewDistributorHandler() {
            this._editInProgress = true;
            bool createNew = false; ;
            this.DispatcherService.BeginInvoke(() => {
                if (this.ShowNewDistributorDialog()) {
                    createNew = true;
                }
            });
            if (createNew) {
                DistributorEditInput input = new DistributorEditInput(this._newDistributorViewModel.Name, this._newDistributorViewModel.Description, EditAction.Add);
                var response = await this._distributorEdit.Execute(input);
                await this.ShowActionResponse(response);
            }
            this._editInProgress = false;
        }

        private bool ShowNewDistributorDialog() {
            if (this._newDistributorViewModel == null) {
                this._newDistributorViewModel = new NewDistributorViewModel();
            }

            UICommand saveCommand = new UICommand() {
                Caption = "Add Distributor",
                IsCancel = false,
                IsDefault = true,
            };

            UICommand cancelCommand = new UICommand() {
                Id = MessageBoxResult.Cancel,
                Caption = "Cancel",
                IsCancel = true,
                IsDefault = false,
            };

            UICommand result = this.NewDistributorDialog.ShowDialog(
                dialogCommands: new List<UICommand>() { saveCommand, cancelCommand },
                title: "New Distributor",
                viewModel: this._newDistributorViewModel);
            return result == saveCommand;
        }

        private async Task DeleteDistributorHandler() {
            if (this.SelectedDistributor != null) {
                this._editInProgress = true;
                DistributorEditInput input = new DistributorEditInput(this.SelectedDistributor.Id, EditAction.Delete);
                var response = await this._distributorEdit.Execute(input);
                await this.ShowActionResponse(response);
                this._editInProgress = false;
            }
        }

        private async Task ShowActionResponse(DistributorEditOutput response) {
            if (response.Success) {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Success", MessageButton.OK, MessageIcon.Information);
                });
                await this.Reload(response.Distributor.Id);

            } else {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Error", MessageButton.OK, MessageIcon.Error);
                });
                await this.Reload();
            }
        }

        #endregion

        #region CallBackRegion

        private async Task DistributorEditDoneHandler(int? distributorId) {
            this._editInProgress = false;
            await this.Reload(distributorId);
        }

        private async Task DistributorEditCancelHandler(int? distributorId) {
            this._editInProgress = false;
            await this.Reload(distributorId);
        }
        
        #endregion

        #region NavigationRegion

        private async Task ViewDistributorDetailsHandler() {
            if (this.SelectedDistributor != null) {
                await Task.Run(() => this.NavigateDetails(this.SelectedDistributor.Id, false));
                
            }
        }

        private async Task EditDistributorHandler() {
            if (this.SelectedDistributor != null) {
                await Task.Run(() => {
                    this.NavigateDetails(this.SelectedDistributor.Id, true);
                });
            }
        }

        private void NavigateDetails(int distributorId,bool isEdit) {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add(ParameterKeys.SelectedDistributorId, this.SelectedDistributor.Id);
                parameters.Add(ParameterKeys.IsEdit,isEdit);
                this._editInProgress = isEdit;
                this.DispatcherService.BeginInvoke(() => {
                    this.CleaupRegions();
                    this._regionManager.RequestNavigate(LocalRegions.DistributorDetailsRegion, ModuleViews.DistributorDetailsView, parameters);
                });
        }

        private void CleaupRegions() {
            this._regionManager.Regions[LocalRegions.DistributorDetailsRegion].RemoveAll();
            this._regionManager.Regions.Remove(LocalRegions.AttachmentTableRegion);
        }

        #endregion

        #region InitializeRegion

        private async Task Reload(int? distributorId = null) {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            await this._distributorEdit.Load();
            var distributors = await this._distributorEdit.GetDistributors();
            this.DispatcherService.BeginInvoke(() => {
                this.Distributors = new ObservableCollection<Distributor>(distributors);
                if (distributorId.HasValue) {
                    this.SelectedDistributor = this.Distributors.FirstOrDefault(e => e.Id == distributorId);
                    this.NavigateDetails(this.SelectedDistributor.Id, false);
                }
                this.ShowTableLoading = false;
            });
        }

        private async Task Load() {
            if (!this._isInitialized) {
                this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
                var distributors = await this._distributorEdit.GetDistributors();
                this.DispatcherService.BeginInvoke(() => {
                    this.Distributors = new ObservableCollection<Distributor>(distributors);
                    this.ShowTableLoading = false;
                });
                this._isInitialized = true;
            }
        }

        #endregion




    }
}
