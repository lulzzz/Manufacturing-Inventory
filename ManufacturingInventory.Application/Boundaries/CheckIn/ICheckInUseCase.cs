using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.Boundaries.CheckIn {
    public interface ICheckInUseCase:IUseCase<CheckInInput, CheckInOutput> {
        Task<IEnumerable<Location>> GetLocations();
        Task<IEnumerable<Category>> GetCategories();
        Task<IEnumerable<Distributor>> GetDistributors();
    }
}
