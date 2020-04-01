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

namespace ManufacturingInventory.Application.UseCases {
    public class CategoryEdit : ICategoryEditUseCase {
        private ManufacturingContext _context;
        private IRepository<Category> _categoryRepository;
        private IRepository<PartInstance> _partInstanceProvider;
        private IRepository<Part> _partProvider;
        private IUnitOfWork _unitOfWork;

        public CategoryEdit(ManufacturingContext context) {
            this._context = context;
            this._categoryRepository = new CategoryRepository(context);
            this._partInstanceProvider = new PartInstanceRepository(context);
            this._partProvider = new PartRepository(context);
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

        public async Task<IEnumerable<PartInstance>> GetCategoryPartInstances(int categoryId) {
            return await this._partInstanceProvider.GetEntityListAsync(e => (e.StockTypeId == categoryId || e.UsageId == categoryId || e.ConditionId == categoryId));
        }

        public async Task<IEnumerable<Part>> GetCategoryParts(int categoryId) {
            return await this._partProvider.GetEntityListAsync(e => e.OrganizationId == categoryId);
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
        
        //private bool UpdateRelatedToDefault(Category category,CategoryTypes type) {
        //    switch (type) {
        //        case CategoryTypes.Organization: {

        //            ((Organization)category).Parts.ToList().ForEach(part => { 

        //            });
        //        }
        //        case CategoryTypes.Condition: {

        //        }
        //        case CategoryTypes.StockType: {

        //        }
        //        case CategoryTypes.Usage: {

        //        }
        //        case CategoryTypes.InvalidType:
        //            return false;
        //        default:
        //            return false;
        //    }
        //}
    }
}
