using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.Boundaries.CheckIn {
    public interface ICheckInUseCase:IUseCase<CheckInInput, CheckInOutput> {
        Task<IEnumerable<Warehouse>> GetWarehouses();
        Task<IEnumerable<Category>> GetCategories();
        Task<IEnumerable<Distributor>> GetDistributors();
        Task<IEnumerable<Price>> GetAvailablePrices(int partId);
        Task<Price> GetPrice(int priceId);
        Task<Part> GetPart(int partId);
        Task<PartInstance> GetExisitingPartInstance(int instanceId);
    }
}
