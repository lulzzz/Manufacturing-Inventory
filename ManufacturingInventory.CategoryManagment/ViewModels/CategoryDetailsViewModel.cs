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
using ManufacturingInventory.Domain.Extensions;
using ManufacturingInventory.Application.Boundaries;

namespace ManufacturingInventory.CategoryManagment.ViewModels {
    public class CategoryDetailsViewModel : InventoryViewModelNavigationBase {
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("CategoryDetailsDispatcher"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("CategoryDetailsMessageBox"); }

        private ICategoryEditUseCase _categoryEdit;
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;

        private CategoryDTO _selectedCategory = new CategoryDTO();
        private ObservableCollection<Part> _parts=new ObservableCollection<Part>();
        private ObservableCollection<PartInstance> _partInstances=new ObservableCollection<PartInstance>();
        private CategoryOption _selectedCategoryType;
        private string _categoryTypeLabel;
        private int _quantity;
        private int _safeQuantity;
        private int _minQuantity;
        private string _name;
        private string _description;

        private bool _partInstancesEnabled;
        private bool _showTableLoading;
        private bool _partsEnabled;
        private bool _canChangeType;

        private bool _canEdit;
        private bool _isStockType;
        private int _categoryId;
        private bool _isNew;
        private bool _isEdit;

        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand CategoryTypeChangedCommand { get; private set; }

        public CategoryDetailsViewModel(ICategoryEditUseCase categoryEdit,IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._categoryEdit = categoryEdit;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.Load);
            this.SaveCommand = new AsyncCommand(this.SaveHandler);
            this.CancelCommand = new AsyncCommand(this.CancelHandler);
            this.CategoryTypeChangedCommand = new AsyncCommand(this.CategoryTypeChangedHandler);
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

        public string CategoryTypeLabel {
            get => this._categoryTypeLabel;
            set => SetProperty(ref this._categoryTypeLabel, value);
        }

        public int Quantity { 
            get => this._quantity;
            set {
                SetProperty(ref this._quantity, value);
                if (this.SelectedCategory != null) {
                    this.SelectedCategory.Quantity = value;
                }

            }
        }

        public int SafeQuantity {
            get => this._safeQuantity;
            set {
                SetProperty(ref this._safeQuantity, value);
                if (this.SelectedCategory != null) {
                    this.SelectedCategory.SafeQuantity = value;
                }

            }
        }

        public int MinQuantity {
            get => this._minQuantity;
            set {
                SetProperty(ref this._minQuantity, value);
                if (this.SelectedCategory != null) {
                    this.SelectedCategory.MinQuantity = value;
                }

            }
        }

        public string Name { 
            get => this._name;
            set {
                SetProperty(ref this._name, value);
                if (this.SelectedCategory != null) {
                    this.SelectedCategory.Name = value;
                }
            } 
        }

        public string Description {
            get => this._description;
            set {
                SetProperty(ref this._description, value);
                if (this.SelectedCategory != null) {

                }
                this.SelectedCategory.Description = value;
            }
        }

        public bool PartInstancesEnabled { 
            get => this._partInstancesEnabled;
            set=>SetProperty(ref this._partInstancesEnabled, value);
        }

        public bool ShowTableLoading { 
            get => this._showTableLoading;
            set=>SetProperty(ref this._showTableLoading, value);
        }

        public bool PartsEnabled { 
            get => this._partsEnabled;
            set=>SetProperty(ref this._partsEnabled, value);
        }

        public bool IsNew { 
            get => this._isNew; 
            set => SetProperty(ref this._isNew,value);
        }

        public bool CanChangeType { 
            get => this._canChangeType; 
            set => SetProperty(ref this._canChangeType,value); 
        }

        #endregion

        #region HandlerRegion

        private async Task SaveHandler() {
            EditAction action = (this.IsNew) ? EditAction.Add : EditAction.Update;
            CategoryBoundaryInput input = new CategoryBoundaryInput(action, this.SelectedCategory);
            var response=await this._categoryEdit.Execute(input);
            await this.ShowActionResponse(response);
        }

        private async Task CancelHandler() {
            await Task.Run(() => {
                var response = this.MessageBoxService.ShowMessage("Are you sure you want to cancel? All Changes will be lost" 
                    + Environment.NewLine + "Yes: Continue, No: Do nothing", "Warning", MessageButton.YesNo, MessageIcon.Warning, MessageResult.No);
                if (response == MessageResult.Yes) {
                    if (this._isEdit) {
                        this._eventAggregator.GetEvent<CategoryEditCancelEvent>().Publish(this.SelectedCategory.Id);
                    } else {
                        this._eventAggregator.GetEvent<CategoryEditCancelEvent>().Publish(null);
                    }

                }
            });
        }

        private async Task CategoryTypeChangedHandler() {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    this.SelectedCategory.Type = this.SelectedCategoryType.GetCategoryType();
                    this.DisplayCategorySpecificItems(this.SelectedCategoryType);
                    if (this.IsNew && this.SelectedCategoryType != CategoryOption.NotSelected) {
                        this.CanEdit = true;
                    }
                    this.IsStockType = this.SelectedCategoryType == CategoryOption.StockType;
                });
            });

        }

        private async Task ShowActionResponse(CategoryBoundaryOutput response) {
            await Task.Run(() => {
                if (response.Success) {
                    this.DispatcherService.BeginInvoke(() => {
                        this.MessageBoxService.ShowMessage(response.Message, "Success", MessageButton.OK, MessageIcon.Information);
                    });
                    this._eventAggregator.GetEvent<CategoryEditDoneEvent>().Publish(response.Category.Id);
                } else {
                    this.DispatcherService.BeginInvoke(() => {
                        this.MessageBoxService.ShowMessage(response.Message, "Error", MessageButton.OK, MessageIcon.Error);
                    });

                }
            });
        }

        #endregion

        #region InitializeRegion

        private async Task Load() {
            if (this.IsNew) {
                this.DispatcherService.BeginInvoke(() => {
                    this.ShowTableLoading = false;
                    this.SetupUI();
                });
            } else {
                this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
                var category = await this._categoryEdit.GetCategory(this._categoryId);
                var partInstances = await this._categoryEdit.GetCategoryPartInstances(this._categoryId);
                var parts = await this._categoryEdit.GetCategoryParts(this._categoryId);
                if (category != null) {
                    this.DispatcherService.BeginInvoke(() => {
                        this.SetupUI(parts, partInstances, category);
                        this.ShowTableLoading = false;
                    });
                } else {
                    this.DispatcherService.BeginInvoke(()=> { 
                        this.MessageBoxService.ShowMessage("Error: Category not found", "Error", MessageButton.OK, MessageIcon.Error);
                        this.ShowTableLoading = false;
                    });
                }
            }
        }

        private void DisplayCategorySpecificItems(CategoryOption categoryOption) {
            switch (categoryOption) {
                case CategoryOption.Condition:
                case CategoryOption.StockType:
                case CategoryOption.Usage:
                    this.PartsEnabled = false;
                    this.PartInstancesEnabled = true;
                    break;
                case CategoryOption.Organization:
                    this.PartsEnabled = true;
                    this.PartInstancesEnabled = false;
                    break;
                case CategoryOption.NotSelected:
                    this.PartsEnabled = false;
                    this.PartInstancesEnabled = false;
                    break;
                default:
                    this.PartsEnabled = false;
                    this.PartInstancesEnabled = false;
                    break;
            }
        }

        private void SetupUI(IEnumerable<Part> parts=null,IEnumerable<PartInstance> partInstances=null,CategoryDTO category=null) {
            if (this.IsNew) {
                this.CategoryTypeLabel = "Select Category Type To Create";
                this.CanEdit = false;
                this.CanChangeType = true;
                this.SelectedCategory = new CategoryDTO();
                this.SelectedCategoryType = CategoryOption.NotSelected;
                this.DisplayCategorySpecificItems(CategoryOption.NotSelected);
            } else {
                if (category != null) {
                    this.CanEdit = this._isEdit;
                    if (partInstances != null) {
                        this.PartInstances = new ObservableCollection<PartInstance>(partInstances);
                    }

                    if (parts != null) {
                        this.Parts = new ObservableCollection<Part>(parts);
                    }

                    this.CanChangeType = (this.Parts.Count == 0 && this.PartInstances.Count == 0 && this._isEdit);
                    this.CategoryTypeLabel = (this.CanChangeType) ? "Select to Change Type" : "Category Type";
                    this.SelectedCategory = category;
                    this.Name = category.Name;
                    this.Description = category.Description;
                    this.SelectedCategoryType = category.Type.GetCategoryOption();
                    if (this.SelectedCategoryType == CategoryOption.StockType) {
                        this.IsStockType = true;
                        this.Quantity = category.Quantity;
                        this.MinQuantity = category.MinQuantity;
                        this.SafeQuantity = category.SafeQuantity;
                    } else {
                        this.IsStockType = false;
                    }
                    this.DisplayCategorySpecificItems(this.SelectedCategoryType);
                }
            }
        }

        #endregion

        #region NavigationRegion

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var parameters = navigationContext.Parameters;
            this.IsNew=Convert.ToBoolean(parameters[ParameterKeys.IsNew]);
            this._isEdit = Convert.ToBoolean(parameters[ParameterKeys.IsEdit]);
            if (!this.IsNew) {
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
