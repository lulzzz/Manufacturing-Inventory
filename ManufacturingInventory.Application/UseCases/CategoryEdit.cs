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

namespace ManufacturingInventory.Application.UseCases {
    public class CategoryEdit : ICategoryEditUseCase {
        private ManufacturingContext _context;
        private IRepository<Category> _categoryRepository;
        private IEntityProvider<PartInstance> _partInstanceProvider;
        private IEntityProvider<Part> _partProvider;
        private IUnitOfWork _unitOfWork;

        public CategoryEdit(ManufacturingContext context) {
            this._context = context;
            this._categoryRepository = new CategoryRepository(context);
            this._partInstanceProvider = new PartInstanceProvider(context);
            this._partProvider = new PartProvider(context);
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
            
            return new CategoryBoundaryOutput(null, false, "Error: Update Action Not Implemented Yet");
        }

        public async Task<CategoryBoundaryOutput> ExecuteUpdate(CategoryBoundaryInput input) {
            return new CategoryBoundaryOutput(null, false, "Error: Update Action Not Implemented Yet");
        }

        public async Task<CategoryBoundaryOutput> ExecuteDelete(CategoryBoundaryInput input) {
            return new CategoryBoundaryOutput(null, false, "Error: Delete Action Not Implemented Yet");
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategories() {
            return (await this._categoryRepository.GetEntityListAsync()).Select(category => new CategoryDTO(category));
        }
        public async Task<T> GetCategory<T>(int categoryId) where T:Category {
            var category = (await this._categoryRepository.GetEntityAsync(e => e.Id == categoryId));

            return (category != null && category.GetType().Equals(typeof(T))) ? (T)category : null;
            //Type type = typeof(T);
            //var cat=(T)Activator.CreateInstanceFrom(category);
           
        }
        public async Task<IEnumerable<PartInstance>> GetCategoryPartInstances(int categoryId) {
            return await this._partInstanceProvider.GetEntityListAsync(e => (e.StockTypeId == categoryId || e.UsageId == categoryId || e.ConditionId == categoryId));
        }

        public async Task<IEnumerable<Part>> GetCategoryParts(int categoryId) {
            return await this._partProvider.GetEntityListAsync(e => e.OrganizationId == categoryId);
        }
    }
}
