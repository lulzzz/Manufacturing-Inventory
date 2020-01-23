using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.Boundaries.PartDetails {
    public interface IPartSummaryEditUseCase:IUseCaseV2<PartSummaryEditInput,PartSummaryEditOutput> {
        Task<IEnumerable<Warehouse>> GetWarehouses();
        Task<IEnumerable<Category>> GetCategories();
        Task<Part> GetPart(int id);
    }
}
