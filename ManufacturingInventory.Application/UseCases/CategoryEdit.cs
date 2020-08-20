using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.Boundaries.CategoryBoundaries;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Providers;
using System.Linq;
using System.Reflection;
using ManufacturingInventory.Infrastructure.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;

namespace ManufacturingInventory.Application.UseCases {
    public class CategoryEdit : ICategoryEditUseCase {
        private ManufacturingContext _context;
        private IRepository<Category> _categoryRepository;
        private IRepository<PartInstance> _instanceRepository;
        private IRepository<Part> _partRepository;
        private IUnitOfWork _unitOfWork;

        public CategoryEdit(ManufacturingContext context) {
            this._context = context;
            this._categoryRepository = new CategoryRepository(context);
            this._instanceRepository = new PartInstanceRepository(context);
            this._partRepository = new PartRepository(context);
            this._unitOfWork = new UnitOfWork(context);
        }

        public async Task<CategoryBoundaryOutput> Execute(CategoryBoundaryInput input) {
            switch (input.EditAction) {
                case Boundaries.EditAction.Add:
                    return await this.ExecuteAdd(input);
                case Boundaries.EditAction.Delete:
                    return await this.ExecuteDelete(input);
                case Boundaries.EditAction.Update:
                    return await this.ExecuteUpdate(input);
                default:
                    return new CategoryBoundaryOutput(null, false, "Error: Invalid Edit Action");
            }
        }

        public async Task<CategoryBoundaryOutput> ExecuteAdd(CategoryBoundaryInput input) {
            var category = input.Category.GetCategory();
            if (category == null) {

                return new CategoryBoundaryOutput(null, false,"Internal Error: Could not Cast to Specified Category, Please Contact Admin");
            }
            if (input.IsDefault_Changed) {
                await this.ClearCategoryDefault(input.Category.Type);
            }
            if (input.Category.Type == CategoryTypes.StockType) {
                CombinedAlert combinedAlert = new CombinedAlert();
                combinedAlert.StockHolder = category as StockType;
            }
            var added = await this._categoryRepository.AddAsync(category);
            if (added != null) {
                var count = await this._unitOfWork.Save();
                if (count > 0) {
                    return new CategoryBoundaryOutput(category, true, "Success: Category Added");
                } else {
                    await this._unitOfWork.Undo();
                    return new CategoryBoundaryOutput(null, false, "Error:  Failed to Add new Category");
                }
            } else {
                await this._unitOfWork.Undo();
                return new CategoryBoundaryOutput(null, false, "Error:  Failed to Add new Category");
            }
        }

        public async Task<CategoryBoundaryOutput> ExecuteUpdate(CategoryBoundaryInput input) {
            var category = await this._categoryRepository.GetEntityAsync(e => e.Id == input.Category.Id);
            if (category != null) {
                if (input.IsDefault_Changed) {
                    await this.ClearCategoryDefault(input.Category.Type);
                }
                switch (input.Category.Type) {
                    case CategoryTypes.Organization:
                    case CategoryTypes.Condition:
                    case CategoryTypes.Usage:
                        category.Set(input.Category);
                        break;
                    case CategoryTypes.StockType:
                        category.Set(input.Category);
                        ((StockType)category).Quantity = input.Category.Quantity;
                        ((StockType)category).MinQuantity = input.Category.MinQuantity;
                        ((StockType)category).SafeQuantity = input.Category.SafeQuantity;
                        ((StockType)category).HoldsBubblers = input.Category.HoldsBubblers;
                        break;
                    case CategoryTypes.InvalidType:
                        return new CategoryBoundaryOutput(null, false, "Error: Invalid Type, Please make sure category type is selected");
                    default:
                        return new CategoryBoundaryOutput(null, false, "Internal Error: No Type, Please contact Admin");
                }
                var updated = await this._categoryRepository.UpdateAsync(category);
                if (updated != null) {
                    var count = await this._unitOfWork.Save();
                    if (count != 0) {
                        return new CategoryBoundaryOutput(category, true, "Success: Category Updated");
                    } else {
                        await this._unitOfWork.Undo();
                        return new CategoryBoundaryOutput(null, false, "Error:  Save Failed");
                    }
                } else {
                    await this._unitOfWork.Undo();
                    return new CategoryBoundaryOutput(null, false, "Error: Update Failed");
                }

            } else {
                return new CategoryBoundaryOutput(null, false, "Error: Category Not Found,Please Contact Admin");
            }
        }

        public async Task<CategoryBoundaryOutput> ExecuteDelete(CategoryBoundaryInput input) {
            var category = await this._categoryRepository.GetEntityAsync(e => e.Id == input.Category.Id);
            if (category != null) {
                var deleted = await this._categoryRepository.DeleteAsync(category);
                if (deleted != null) {
                    var count = await this._unitOfWork.Save();
                    if (count != 0) {
                        return new CategoryBoundaryOutput(category, true, "Success: Category Deleted");
                    } else {
                        await this._unitOfWork.Undo();
                        return new CategoryBoundaryOutput(null, false, "Error: Save Failed");
                    }
                } else {
                    await this._unitOfWork.Undo();
                    return new CategoryBoundaryOutput(null, false, "Error: Delete Failed");
                }
            } else {
                return new CategoryBoundaryOutput(null, false, "Error: Category Not Found,Please Contact Admin");
            }
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategories() {
            return (await this._categoryRepository.GetEntityListAsync()).Select(category => new CategoryDTO(category));
        }

        public async Task<CategoryDTO> GetCategory(int categoryId) {
            var category = await this._categoryRepository.GetEntityAsync(e => e.Id == categoryId);
            if (category != null) {
                return new CategoryDTO(category);
            } else {
                return null;
            }
        }

        public async Task<IEnumerable<PartInstance>> GetCategoryPartInstances(CategoryDTO category) {
            switch (category.Type) {
                case CategoryTypes.Condition:
                    return await this._instanceRepository.GetEntityListAsync(e => e.ConditionId == category.Id);
                case CategoryTypes.StockType:
                    return await this._instanceRepository.GetEntityListAsync(e => e.StockTypeId == category.Id);
                case CategoryTypes.Usage:
                    return await this._instanceRepository.GetEntityListAsync(e => e.UsageId == category.Id);
                default:
                    return null;
            }
        }

        public async Task<IEnumerable<Part>> GetCategoryParts(int categoryId) {
            return await this._partRepository.GetEntityListAsync(e => e.OrganizationId == categoryId);
        }

        public async Task<CategoryDTO> GetDefault(CategoryTypes type) {
            var categories = await this.GetCategories();
            switch (type) {
                case CategoryTypes.Organization:
                    return (categories).FirstOrDefault(e => e.IsDefault && e.Type == CategoryTypes.Organization);
                case CategoryTypes.Condition:
                    return (categories).FirstOrDefault(e => e.IsDefault && e.Type == CategoryTypes.Condition);
                case CategoryTypes.StockType:
                    return (categories).FirstOrDefault(e => e.IsDefault && e.Type == CategoryTypes.StockType);
                case CategoryTypes.Usage:
                    return (categories).FirstOrDefault(e => e.IsDefault && e.Type==CategoryTypes.Usage);
                case CategoryTypes.InvalidType:
                    return null;
                default:
                    return null;
            }
        }

        private async Task ClearCategoryDefault(CategoryTypes type) {
            switch (type) {
                case CategoryTypes.Organization:
                    var defaultOrganization=await this._context.Categories.OfType<Organization>().FirstOrDefaultAsync(e=>e.IsDefault);
                    if (defaultOrganization != null) {
                        defaultOrganization.IsDefault = false;
                        await this._categoryRepository.UpdateAsync(defaultOrganization);
                    }
                    break;
                case CategoryTypes.Condition:
                    var defaultCondition = await this._context.Categories.OfType<Condition>().FirstOrDefaultAsync(e => e.IsDefault);
                    if (defaultCondition != null) {
                        defaultCondition.IsDefault = false;
                        await this._categoryRepository.UpdateAsync(defaultCondition);
                    }
                    break;
                case CategoryTypes.StockType:
                    //var defaultStockType = await this._context.Categories.OfType<Organization>().FirstOrDefaultAsync(e => e.IsDefault);
                    //if (defaultStockType != null) {
                    //    defaultStockType.IsDefault = false;
                    //    await this._categoryRepository.UpdateAsync(defaultStockType);
                    //}
                    break;
                case CategoryTypes.Usage:
                    var defaultUsage = await this._context.Categories.OfType<Usage>().FirstOrDefaultAsync(e => e.IsDefault);
                    if (defaultUsage != null) {
                        defaultUsage.IsDefault = false;
                        await this._categoryRepository.UpdateAsync(defaultUsage);
                    }
                    break;
                case CategoryTypes.InvalidType:
                    break;
                default:
                    break;
            }
        }

        public async Task<CategoryBoundaryOutput> AddPartTo(int entityId,CategoryDTO category) {
            if (category.Type == CategoryTypes.Organization) {
                var part = await this._partRepository.GetEntityAsync(e => e.Id == entityId);
                var newCategory = await this._categoryRepository.GetEntityAsync(e => e.Id == category.Id);

                if (part == null || newCategory == null) {
                    var msg = (part == null) ? "Part Not Found" : "Category Not Found";
                    return new CategoryBoundaryOutput(null, false, "Error: " + msg);
                }
                part.OrganizationId = newCategory.Id;
                var updated = await this._partRepository.UpdateAsync(part);
                if (updated != null) {
                    await this._unitOfWork.Save();
                    return new CategoryBoundaryOutput(newCategory, true, "PartInstance " + part.Name + " Added Successfully");
                } else {
                    await this._unitOfWork.Undo();
                    return new CategoryBoundaryOutput(null, false, "PartInstance " + part.Name + " Failed to Add");
                }
            } else {
                var instance = await this._instanceRepository.GetEntityAsync(e => e.Id == entityId);
                var newCategory = await this._categoryRepository.GetEntityAsync(e => e.Id == category.Id);
                if (instance == null || newCategory == null) {
                    var msg = (instance == null) ? "PartInstance Id "+ entityId + "Not Found" : "Category Not Found";
                    return new CategoryBoundaryOutput(null, false, "Error: " + msg);
                }
                bool error=false;
                switch (category.Type) {
                    case CategoryTypes.Condition:
                        instance.ConditionId = newCategory.Id;
                        break;
                    case CategoryTypes.StockType:
                        var oldCategory = await this._categoryRepository.GetEntityAsync(e => e.Id == instance.StockTypeId);
                        if (oldCategory != null) {
                            instance.StockTypeId = newCategory.Id;
                            if (((StockType)newCategory).HoldsBubblers && instance.IsBubbler) {
                                if (!newCategory.IsDefault) {
                                    ((StockType)newCategory).Quantity += (int)instance.BubblerParameter.Weight;
                                    if (oldCategory.IsDefault) {
                                        var userAlerts=this._context.UserAlerts.Where(e => e.AlertId == instance.IndividualAlertId);
                                        if (userAlerts.Count() > 0) {
                                            this._context.RemoveRange(userAlerts);
                                        }
                                        this._context.Alerts.Remove(instance.IndividualAlert);
                                        instance.IndividualAlert = null;
                                    }
                                }
                                if (!oldCategory.IsDefault) {
                                    ((StockType)oldCategory).Quantity -= (int)instance.BubblerParameter.Weight;
                                }
                            } else {
                                if (!newCategory.IsDefault) {
                                    ((StockType)newCategory).Quantity += instance.Quantity;
                                    if (oldCategory.IsDefault) {
                                        var userAlerts = this._context.UserAlerts.Where(e => e.AlertId == instance.IndividualAlertId);
                                        if (userAlerts.Count() > 0) {
                                            this._context.RemoveRange(userAlerts);
                                        }

                                        this._context.Alerts.Remove(instance.IndividualAlert);
                                        instance.IndividualAlert = null;
                                    }
                                }
                                if (!oldCategory.IsDefault) {
                                    ((StockType)oldCategory).Quantity -= instance.Quantity;
                                }
                            }
                            var oldUpdated=await this._categoryRepository.UpdateAsync(oldCategory);
                            error=!(oldUpdated != null); 
                        } else {
                            error = true;
                        }
                        break;
                    case CategoryTypes.Usage:
                        instance.UsageId = newCategory.Id;
                        break;
                }
                if (!error) {
                    var catUpdated = await this._categoryRepository.UpdateAsync(newCategory);
                    var instanceUpdated = await this._instanceRepository.UpdateAsync(instance);
                    if (instanceUpdated != null && catUpdated != null) {
                        await this._unitOfWork.Save();
                        return new CategoryBoundaryOutput(newCategory, true, "PartInstance " + instance.Name + " Added Successfully");
                    } else {
                        await this._unitOfWork.Undo();
                        return new CategoryBoundaryOutput(null, false, "PartInstance " + instance.Name + " Failed to Add");
                    }
                } else {
                    await this._unitOfWork.Undo();
                    return new CategoryBoundaryOutput(null, false, "PartInstance " + instance.Name + " Failed to Add");
                }
            }
        }

        public async Task<IEnumerable<CategoryBoundaryOutput>> AddPartTo(IEnumerable<int> entityIds, CategoryDTO category) {
            List<CategoryBoundaryOutput> outputList = new List<CategoryBoundaryOutput>();
            foreach(int entityId in entityIds) {
                var output = await this.AddPartTo(entityId, category);
                outputList.Add(output);
            }
            return outputList;
        }

        public async Task<IEnumerable<PartInstance>> GetAvailablePartInstances(CategoryDTO category) {
            switch (category.Type) {
                case CategoryTypes.Condition: {
                    return await this._instanceRepository.GetEntityListAsync(e => e.ConditionId != category.Id);
                }
                case CategoryTypes.StockType: {

                    return await this._instanceRepository.GetEntityListAsync(e => e.StockTypeId != category.Id && e.IsBubbler==category.HoldsBubblers);
                }
                case CategoryTypes.Usage: {
                    return await this._instanceRepository.GetEntityListAsync(e => e.UsageId != category.Id);
                }
                default:
                    return null;
            }
        }

        public async Task<IEnumerable<Part>> GetAvailableParts(int categoryId) {
            return await this._partRepository.GetEntityListAsync(e => e.OrganizationId != categoryId);
        }
    }
}
