using DevExpress.Mvvm;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Application.UI.Services;
using ManufacturingInventory.Domain.Enums;
using ManufacturingInventory.Infrastructure.Model.Entities;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ManufacturingInventory.Domain.DTOs;
using System;
using ManufacturingInventory.Common.Application.UI.ViewModels;
using System.Windows;
using System.Collections.Generic;
using ManufacturingInventory.Application.Boundaries.CategoryBoundaries;
using ManufacturingInventory.CategoryManagment.Internal;

namespace ManufacturingInventory.CategoryManagment.ViewModels {
    public class CategoryDetailsViewModel : InventoryViewModelNavigationBase {
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("CategoryDetailsDispatcher"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("CategoryDetailsMessageBox"); }

        private ICategoryEditUseCase _categoryEdit;
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;

        private CategoryDTO _selectedCategory;
        private ObservableCollection<Part> _parts;
        private ObservableCollection<PartInstance> _partInstances;
        private CategoryOption _selectedCategoryType;
        private bool _canEdit;
        private bool _isStockType;
        private int _categoryId;
        private bool _isNew;
        private bool _isEdit;

        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }

        public CategoryDetailsViewModel(ICategoryEditUseCase categoryEdit,IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._categoryEdit = categoryEdit;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;

        }

        #region ParameterBinding

        public override bool KeepAlive => false;

        public CategoryDTO SelectedCategory { 
            get => this._selectedCategory;
            set => SetProperty(ref this._selectedCategory, value);
        }

        public ObservableCollection<Part> Parts { 
            get => this._parts;
            set => SetProperty(ref this._parts, value);
        }

        public ObservableCollection<PartInstance> PartInstances { 
            get => this._partInstances;
            set => SetProperty(ref this._partInstances, value);
        }

        public bool CanEdit { 
            get => this._canEdit;
            set => SetProperty(ref this._canEdit, value);
        }

        public bool IsStockType { 
            get => this._isStockType;
            set => SetProperty(ref this._isStockType, value);
        }

        public CategoryOption SelectedCategoryType { 
            get => this._selectedCategoryType;
            set => SetProperty(ref this._selectedCategoryType, value);
        }

        #endregion

        #region HandlerRegion

        private async Task SaveHandler() {

        }

        private async Task CancelHandler() {

        }

        #endregion

        #region InitializeRegion

        private async Task Load() {
            if (this._isNew) {

            } else {
                var category = await this._categoryEdit.GetCategory(this._categoryId);
                if (category.Type == CategoryTypes.StockType) {

                }
            }
        }

        #endregion

        #region NavigationRegion

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var parameters = navigationContext.Parameters;
            this._isNew=Convert.ToBoolean(parameters[ParameterKeys.IsNew]);
            this._isEdit = Convert.ToBoolean(parameters[ParameterKeys.IsNew]);
            if (!this._isNew) {
                this._categoryId = Convert.ToInt32(parameters[ParameterKeys.SelectedCategoryId]);
            }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }

        #endregion



    }
}
