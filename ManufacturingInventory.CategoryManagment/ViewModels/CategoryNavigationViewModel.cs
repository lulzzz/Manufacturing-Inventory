using DevExpress.Mvvm;
using ManufacturingInventory.Application.Boundaries.DistributorManagment;
using ManufacturingInventory.Common.Application;
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
using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Application.Boundaries.CategoryBoundaries;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.CategoryManagment.Internal;
using Serilog;

namespace ManufacturingInventory.CategoryManagment.ViewModels {
    public class CategoryNavigationViewModel : InventoryViewModelBase {

        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("CategoryDispatcherService"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("CategoryMessageBoxService"); }

        private ICategoryEditUseCase _categoryEdit;
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private ILogger _logger;

        private ObservableCollection<CategoryDTO> _categories;
        private CategoryDTO _selectedCategory;
        private bool _showTableLoading;

        private bool _isInitialized = false;
        private bool _editInProgress = false;

        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand AddNewCategoryCommand { get; private set; }
        public AsyncCommand DeleteCategoryCommand { get; private set; }
        public AsyncCommand DoubleClickViewCommand { get; private set; }

        public AsyncCommand ViewCategoryDetailsCommand { get; private set; }
        public AsyncCommand EditCategoryCommand { get; private set; }

        public CategoryNavigationViewModel(ICategoryEditUseCase categoryEdit,IEventAggregator eventAggregator,IRegionManager regionManager,ILogger logger) { 
            this._categoryEdit = categoryEdit;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this._logger = logger;
            //this._logger.Info("In CategoryNavigationViewModel!");
            //Log.Logger.Information("In CategoryNavigationView");
            this._logger.Information("In CategoryNavigationView");
            this.InitializeCommand = new AsyncCommand(this.Load);
            this.AddNewCategoryCommand = new AsyncCommand(this.AddNewCategoryHandler, () => !this._editInProgress);
            this.DeleteCategoryCommand = new AsyncCommand(this.DeleteCategoryHandler, () => !this._editInProgress);
            this.EditCategoryCommand = new AsyncCommand(this.EditCategoryHandler, () => !this._editInProgress);
            this.ViewCategoryDetailsCommand = new AsyncCommand(this.ViewCategoryDetailsHandler, () => !this._editInProgress);
            this.DoubleClickViewCommand = new AsyncCommand(this.ViewCategoryDetailsHandler,()=>!this._editInProgress);
            this._eventAggregator.GetEvent<CategoryEditDoneEvent>().Subscribe(async ( categoryId) => await this.CategoryEditDoneHandler(categoryId));
            this._eventAggregator.GetEvent<CategoryEditCancelEvent>().Subscribe(async (categoryId) => await this.CategoryEditCancelHandler(categoryId));

        }

        #region ParameterBinding

        public override bool KeepAlive => false;

        public ObservableCollection<CategoryDTO> Categories {
            get => this._categories;
            set => SetProperty(ref this._categories, value);
        }

        public CategoryDTO SelectedCategory {
            get => this._selectedCategory;
            set => SetProperty(ref this._selectedCategory, value);
        }

        public bool ShowTableLoading {
            get => this._showTableLoading;
            set => SetProperty(ref this._showTableLoading, value);
        }

        #endregion

        #region HandlerRegion

        private async Task ViewCategoryDetailsHandler() {
            await Task.Run(() => {
                if (this.SelectedCategory != null) {
                    this.DispatcherService.BeginInvoke(() => {
                        this.NavigateDetails(this.SelectedCategory.Id, false, false);
                    });
                }
            });
        }

        private async Task AddNewCategoryHandler() {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    this.NavigateDetails(null, false, true);
                });
            });
        }

        private async Task EditCategoryHandler() {
            await Task.Run(() => {
                if (this.SelectedCategory != null) {
                    this.DispatcherService.BeginInvoke(() => {
                        this.NavigateDetails(this.SelectedCategory.Id, true, false);
                    });
                }
            });
        }

        private Task DeleteCategoryHandler() {
            return Task.CompletedTask;
        }

        #endregion

        #region NavigationRegion

        private void NavigateDetails(int? categoryId, bool isEdit, bool isNew) {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add(ParameterKeys.SelectedCategoryId, categoryId);
            parameters.Add(ParameterKeys.IsEdit, isEdit);
            parameters.Add(ParameterKeys.IsNew, isNew);
            this._editInProgress = isEdit || isNew;
            this.CleaupRegions();
            this._regionManager.RequestNavigate(LocalRegions.CategoryDetailsRegion, ModuleViews.CategoryDetailsView, parameters);
        }

        private void CleaupRegions() {
            //this._regionManager.Regions[LocalRegions.CategoryDetailsRegion].RemoveAll();
        }

        #endregion

        #region CallBackRegion

        private async Task CategoryEditDoneHandler(int? categoryId) {
            this._editInProgress = false;
            await this.Reload(categoryId);
        }

        private async Task CategoryEditCancelHandler(int? categoryId) {
            this._editInProgress = false;
            await this.Reload(categoryId); 
        }

        private async Task ShowActionResponse(CategoryBoundaryOutput response) {
            if (response.Success) {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Success", MessageButton.OK, MessageIcon.Information);
                });
                await this.Reload(response.Category.Id);

            } else {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Error", MessageButton.OK, MessageIcon.Error);
                });
                await this.Reload();
            }
        }

        #endregion

        #region InitializeRegion

        private async Task Load() {
            if (!this._isInitialized) {
                this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
                var categories = await this._categoryEdit.GetCategories();
                this.DispatcherService.BeginInvoke(() => {
                    this.Categories = new ObservableCollection<CategoryDTO>(categories);
                    this._editInProgress = false;
                    this.ShowTableLoading = false;
                    this._isInitialized = true;
                });
            }
        }

        private async Task Reload(int? categoryId = null) {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            var categories = await this._categoryEdit.GetCategories();
            this.DispatcherService.BeginInvoke(() => {
                this.Categories = new ObservableCollection<CategoryDTO>(categories);
                if (categoryId.HasValue) {
                    this.SelectedCategory = this.Categories.FirstOrDefault(cat => cat.Id == categoryId);
                    this.NavigateDetails(categoryId, false,false);
                } else {
                    this.CleaupRegions();
                }
                this._editInProgress = false;
                this.ShowTableLoading = false;
            });
        }

        #endregion













    }
}
