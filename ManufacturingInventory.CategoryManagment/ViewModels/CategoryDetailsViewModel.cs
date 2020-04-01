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
using System.Text;

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
        private bool _isDefault;

        private bool _partInstancesEnabled;
        private bool _showPartInstanceTableLoading;
        private bool _showPartTableLoading;
        private bool _partsEnabled;
        private bool _canChangeType;

        private bool _canEdit;
        private bool _isStockType;
        private bool _canEditDefault;
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
                    this.SelectedCategory.Description = value;
                }

            }
        }

        public bool IsDefault {
            get => this._isDefault;
            set {
                SetProperty(ref this._isDefault, value);
                if (this.SelectedCategory != null) {
                    this.SelectedCategory.IsDefault = value;
                }
            }
        }

        public bool PartInstancesEnabled { 
            get => this._partInstancesEnabled;
            set=>SetProperty(ref this._partInstancesEnabled, value);
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

        public bool CanEditDefault { 
            get => this._canEditDefault;
            set => SetProperty(ref this._canEditDefault, value); 
        }
        
        public bool ShowPartInstanceTableLoading { 
            get => this._showPartInstanceTableLoading;
            set => SetProperty(ref this._showPartInstanceTableLoading, value);
        }
        
        public bool ShowPartTableLoading { 
            get => this._showPartTableLoading;
            set => SetProperty(ref this._showPartTableLoading, value);
        }

        #endregion

        #region HandlerRegion

        private async Task SaveHandler() {
            EditAction action = (this.IsNew) ? EditAction.Add : EditAction.Update;
            var defaultCategory = await this._categoryEdit.GetDefault(this.SelectedCategory.Type);
            bool cancel = false;
            bool defaultChanged = false; ;
            if (defaultCategory != null) {
                if (this.IsDefault) {
                    if (this.SelectedCategory.Id != defaultCategory.Id) {
                        StringBuilder builder = new StringBuilder();
                        builder.AppendLine("Are you sure you want to make default?");
                        builder.AppendFormat("Doing so will replace {0} as default {1} category", defaultCategory.Name, defaultCategory.Type.GetDescription()).AppendLine();
                        builder.AppendLine("Would you like to continue?");
                        builder.AppendLine("Yes: Continue, Replacing default category");
                        builder.AppendLine("No: Mark as not default and save");
                        builder.AppendLine("Cancel: Do Nothing");
                        builder.AppendLine();
                        var messageResult = this.MessageBoxService.ShowMessage(builder.ToString(), "Warning", MessageButton.YesNoCancel, MessageIcon.Question, MessageResult.No);
                        switch (messageResult) {
                            case MessageResult.Cancel:
                                cancel = true;
                                break;
                            case MessageResult.Yes:
                                defaultChanged = true;
                                break;
                            case MessageResult.No:
                                this.SelectedCategory.IsDefault = false;
                                defaultChanged = false;
                                break;
                        }
                        //if (messageResult == MessageResult.No) {
                        //    this.SelectedCategory.IsDefault = false;
                        //} else if (messageResult == MessageResult.Cancel) {
                        //    cancel = true;
                        //}
                    }
                } else {
                    if (this.SelectedCategory.Id == defaultCategory.Id) {
                        StringBuilder builder = new StringBuilder();
                        builder.AppendLine("Cannot clear default category without replacing with another");
                        builder.AppendLine("Would you like to continue?");
                        builder.AppendLine("Yes: Saves but leaves as default");
                        builder.AppendLine("No: Do Nothing");
                        builder.AppendLine("Cancel: Do Nothing");
                        builder.AppendLine();
                        var messageResult = this.MessageBoxService.ShowMessage(builder.ToString(), "Warning", MessageButton.YesNoCancel, MessageIcon.Question, MessageResult.No);
                        switch (messageResult) {
                            case MessageResult.Cancel:
                                cancel = true;
                                break;
                            case MessageResult.Yes:
                                this.SelectedCategory.IsDefault = true;
                                break;
                            case MessageResult.No:
                                cancel = true;
                                break;
                        }

                        //if (messageResult == MessageResult.Yes) {
                        //    this.SelectedCategory.IsDefault = false;
                        //} else if (messageResult == MessageResult.Cancel) {
                        //    cancel = true;
                        //}
                    }
                }
            }
            if (!cancel) {
                CategoryBoundaryInput input = new CategoryBoundaryInput(action, this.SelectedCategory,defaultChanged);
                var response = await this._categoryEdit.Execute(input);
                await this.ShowActionResponse(response);
            }
        }

        private async Task CancelHandler() {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
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
                    this.ShowTableLoading(CategoryOption.NotSelected,false);
                    this.SetupUI();
                });
            } else {
                var category = await this._categoryEdit.GetCategory(this._categoryId);
                if (category != null) {
                    this.SelectedCategory = category;
                    this.SelectedCategoryType = category.Type.GetCategoryOption();
                    this.DispatcherService.BeginInvoke(() => this.ShowTableLoading(this.SelectedCategoryType,true));
                    var partInstances = await this._categoryEdit.GetCategoryPartInstances(this._categoryId);
                    var parts = await this._categoryEdit.GetCategoryParts(this._categoryId);
                    this.DispatcherService.BeginInvoke(() => {
                        this.SetupUI(parts, partInstances, category);
                        this.ShowTableLoading(this.SelectedCategoryType, false);
                    });
                } else {
                    this.DispatcherService.BeginInvoke(()=> { 
                        this.MessageBoxService.ShowMessage("Error: Category not found", "Error", MessageButton.OK, MessageIcon.Error);
                        this.ShowTableLoading(CategoryOption.NotSelected, false);
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
                this.IsDefault = false;
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
                    this.IsDefault = category.IsDefault;

                    if (this.SelectedCategoryType == CategoryOption.StockType) {
                        this.IsStockType = true;
                        this.Quantity = category.Quantity;
                        this.MinQuantity = category.MinQuantity;
                        this.SafeQuantity = category.SafeQuantity;
                        this.CanEditDefault = false;
                    } else {
                        this.IsStockType = false;
                        this.CanEditDefault=this.CanEdit;
                    }
                    this.DisplayCategorySpecificItems(this.SelectedCategoryType);
                }
            }
        }

        private void ShowTableLoading(CategoryOption categoryOption,bool on_off) {
            switch (categoryOption) {
                case CategoryOption.Condition:
                case CategoryOption.StockType:
                case CategoryOption.Usage:
                    this.ShowPartInstanceTableLoading = on_off;
                    this.ShowPartTableLoading = false;
                    break;
                case CategoryOption.Organization:
                    this.ShowPartTableLoading = on_off;
                    this.ShowPartInstanceTableLoading = false;
                    break;
                case CategoryOption.NotSelected:
                    this.PartsEnabled = false;
                    this.PartInstancesEnabled = false;
                    break;
                default:
                    this.ShowPartInstanceTableLoading = false;
                    this.ShowPartTableLoading = false;
                    break;
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
