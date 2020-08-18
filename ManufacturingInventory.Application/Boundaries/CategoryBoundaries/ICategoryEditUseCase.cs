using ManufacturingInventory.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using ManufacturingInventory.Application.UseCases;
using System.Threading.Tasks;
using ManufacturingInventory.Infrastructure.Model.Entities;

namespace ManufacturingInventory.Application.Boundaries.CategoryBoundaries {
    public interface ICategoryEditUseCase:IUseCase<CategoryBoundaryInput,CategoryBoundaryOutput> {
        Task<IEnumerable<CategoryDTO>> GetCategories();
        Task<IEnumerable<PartInstance>> GetCategoryPartInstances(CategoryDTO category);
        Task<IEnumerable<PartInstance>> GetAvailablePartInstances(CategoryDTO category);
        Task<IEnumerable<Part>> GetAvailableParts(int categoryId;
        Task<IEnumerable<Part>> GetCategoryParts(int categoryId);
        Task<CategoryDTO> GetCategory(int categoryId);
        Task<CategoryDTO> GetDefault(CategoryTypes type);
        Task<CategoryBoundaryOutput> AddPartTo(int entityId, CategoryDTO category);
        
    }
}
