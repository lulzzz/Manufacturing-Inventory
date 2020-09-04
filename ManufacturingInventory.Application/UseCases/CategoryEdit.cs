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

            if(input.Category.Type==CategoryTypes.StockType && input.Category.IsDefault) {
                return new CategoryBoundaryOutput(null, false, "Error: Cannot change default StockType");
            }

            if (input.IsDefault_Changed) {
                await this.ClearCategoryDefault(input.Category.Type);
            }

            if (input.Category.Type == CategoryTypes.StockType ) {
                CombinedAlert combinedAlert = new CombinedAlert();
                combinedAlert.StockHolder = category as StockType;
                this._context.Add(combinedAlert);
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
            //if (input.Category.IsDefault) {
            //    var category = await this._categoryRepository.GetEntityAsync(e => e.Id == input.Category.Id);
            //    if (category != null) {
            //        switch (input.Category.Type) {
            //            case CategoryTypes.Organization:
            //                ((Organization)category).Parts.Clear();
            //                break;
            //            case CategoryTypes.Condition:
            //                ((Condition)category).PartInstances.Clear();
            //                break;
            //            case CategoryTypes.StockType:
            //                var individual=await 
            //                var partInstances=((StockType)category).PartInstances;
            //                foreach(var partInstance in partInstances) {

            //                }
            //                break;
            //            case CategoryTypes.Usage:
            //                ((Organization)category).Parts.Clear();
            //                break;
            //            default:
            //                await this._unitOfWork.Undo();
            //                return new CategoryBoundaryOutput(null, false, "Internal Error: Invalid category type");
            //        }
            //        var deleted = await this._categoryRepository.DeleteAsync(org);
            //    } else {
            //        return new CategoryBoundaryOutput(null, false, "Error: Could not find category");
            //    }
            //} else {
            //    return new CategoryBoundaryOutput(null, false, "Error: Cannot delete defaul category");
            //}
            return new CategoryBoundaryOutput(null, false, "Error: Not Implemented");
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
                            if (oldCategory.IsDefault) {
                                if (instance.IsBubbler) {
                                    ((StockType)newCategory).Quantity += (int)instance.BubblerParameter.Weight;
                                } else {
                                    ((StockType)newCategory).Quantity += (int)instance.Quantity;
                                }
                                var userAlerts = this._context.UserAlerts.Where(e => e.AlertId == instance.IndividualAlertId);
                                if (userAlerts.Count() > 0) {
                                    this._context.RemoveRange(userAlerts);
                                }
                                this._context.Alerts.Remove(instance.IndividualAlert);
                                instance.IndividualAlert = null;
                            } else {
                                if (instance.IsBubbler) {
                                    ((StockType)oldCategory).Quantity -= (int)instance.BubblerParameter.Weight;
                                } else {
                                    ((StockType)oldCategory).Quantity -= (int)instance.Quantity;
                                }
                                if (newCategory.IsDefault) {
                                    IndividualAlert alert = new IndividualAlert();
                                    alert.PartInstance = instance;
                                    instance.IndividualAlert = alert;
                                    var added=this._context.Alerts.Add(alert);
                                    if (added == null) {
                                        return new CategoryBoundaryOutput(null, false,"Error: Could not creat new IndividualAlert,Please contact administrator");
                                    }
                                } else {
                                    if (instance.IsBubbler) {
                                        //((StockType)oldCategory).Quantity -= (int)instance.BubblerParameter.Weight;
                                        ((StockType)newCategory).Quantity += (int)instance.BubblerParameter.Weight;
                                    } else {
                                        //((StockType)oldCategory).Quantity -= (int)instance.Quantity;
                                        ((StockType)newCategory).Quantity += (int)instance.Quantity;
                                    }
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

        public async Task<CategoryBoundaryOutput> RemovePartFrom(int entityId,CategoryDTO category) {
            if (category.Type == CategoryTypes.Organization) {
                var part = await this._partRepository.GetEntityAsync(e => e.Id == entityId);
                if (part != null) {
                    part.OrganizationId = null;
                    part.Organization = null;
                    var updated = await this._partRepository.UpdateAsync(part);
                    if (updated != null) {
                        await this._unitOfWork.Save();
                        return new CategoryBoundaryOutput(null, true, "Part " + part.Name + " Removed Successfully");
                    } else {
                        await this._unitOfWork.Undo();
                        return new CategoryBoundaryOutput(null, false, "PartInstance " + part.Name + " Failed to Add");
                    }
                } else {
                    await this._unitOfWork.Undo();
                    return new CategoryBoundaryOutput(null, false, "Error: Part Not Found!");
                }
            } else {
                var instance = await this._instanceRepository.GetEntityAsync(e => e.Id == entityId);
                var exisitingCategory = await this._categoryRepository.GetEntityAsync(e => e.Id == category.Id);
                if (instance == null || exisitingCategory == null) {
                    var msg = (instance == null) ? "PartInstance Id " + entityId + "Not Found" : "Category Not Found";
                    return new CategoryBoundaryOutput(null, false, "Error: " + msg);
                }
                switch (category.Type) {
                    case CategoryTypes.Condition:
                        instance.Condition = null;
                        instance.ConditionId = null;
                        break;
                    case CategoryTypes.StockType:
                        if (!exisitingCategory.IsDefault) {
                            var newCategory = await this._context.Categories.OfType<StockType>().Include(e=>e.PartInstances).FirstOrDefaultAsync(e => e.IsDefault);
                            if (newCategory != null) {
                                if (instance.IsBubbler) {
                                    ((StockType)exisitingCategory).Quantity -= (int)instance.BubblerParameter.Weight;
                                } else {
                                    ((StockType)exisitingCategory).Quantity -= (int)instance.Quantity;
                                }
                                IndividualAlert alert = new IndividualAlert();
                                alert.PartInstance = instance;
                                instance.IndividualAlert = alert;
                                var added = this._context.Alerts.Add(alert);
                                instance.StockType = newCategory;
                                instance.StockTypeId = newCategory.Id;
                                var oldUpdated = await this._categoryRepository.UpdateAsync(exisitingCategory);
                                if (added == null || oldUpdated == null) {
                                    await this._unitOfWork.Undo();
                                    return new CategoryBoundaryOutput(null, false, "Error: Could not update alerts and old category,Please contact administrator");
                                }
                            } else {
                                await this._unitOfWork.Undo();
                                return new CategoryBoundaryOutput(null, false, "Error: Could not remove from category, please contatc administrator");
                            }
                        } else {
                            return new CategoryBoundaryOutput(null, false, "Error: Cannot remove from default category"+Environment.NewLine+"To change category select desired category then add part instance");
                        }
                        break;
                    case CategoryTypes.Usage:
                        instance.UsageId = null;
                        instance.Usage = null;
                        break;
                }
                var instanceUpdated = await this._instanceRepository.UpdateAsync(instance);
                if (instanceUpdated != null) {
                    await this._unitOfWork.Save();
                    return new CategoryBoundaryOutput(exisitingCategory, true, "PartInstance " + instance.Name + " Removed Successfully");
                } else {
                    await this._unitOfWork.Undo();
                    return new CategoryBoundaryOutput(null, false, "PartInstance " + instance.Name + " Failed to Remove");
                }
            }
        }

        public async Task<IEnumerable<PartInstance>> GetAvailablePartInstances(CategoryDTO category) {
            switch (category.Type) {
                case CategoryTypes.Condition: {
                    return await this._instanceRepository.GetEntityListAsync(e => e.ConditionId != category.Id);
                }
                case CategoryTypes.StockType: {
                    if (category.IsDefault) {
                        return await this._instanceRepository.GetEntityListAsync(e => e.StockTypeId != category.Id);
                    } else {
                        return await this._instanceRepository.GetEntityListAsync(e => e.StockTypeId != category.Id && e.IsBubbler == category.HoldsBubblers);
                    }
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
