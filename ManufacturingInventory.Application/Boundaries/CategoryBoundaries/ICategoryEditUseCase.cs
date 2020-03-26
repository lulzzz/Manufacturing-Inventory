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
        //Task<T> GetCategory<T>(int categoryId) where T : Category;
        Task<IEnumerable<PartInstance>> GetCategoryPartInstances(int categoryId);
        Task<IEnumerable<Part>> GetCategoryParts(int categoryId);
        Task<CategoryDTO> GetCategory(int categoryId);
    }
}
