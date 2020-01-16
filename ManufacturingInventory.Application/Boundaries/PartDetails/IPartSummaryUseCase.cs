using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.Boundaries.PartDetails {
    public interface IPartSummaryUseCase:IUseCase<PartSummaryInput,PartSummaryOutput> {
        Task<IEnumerable<Warehouse>> GetWarehouses();
        Task<IEnumerable<Organization>> GetOrganizations();
        Task<IEnumerable<Usage>> GetUsages();
    }
}
