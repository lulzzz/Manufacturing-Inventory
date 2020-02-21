using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.Boundaries.ReturnItem {
    public interface IReturnItemUseCase:IUseCase<ReturnItemInput,ReturnItemOutput> {
        Task<IEnumerable<Warehouse>> GetWarehouses();
        Task<IEnumerable<Condition>> GetConditions();
        Task<PartInstance> GetPartInstance(int instanceId);
        Task<int> GetPartWarehouseId(int partId);
        Task<Tuple<double, double>> GetInstanceNetGross(int partInstanceId);
    }
}
