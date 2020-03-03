using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.Boundaries.PartInstanceTableView {
    public interface IPartInstanceTableViewUseCase{
        Task<IEnumerable<PartInstance>> GetPartInstances(int partId);
        Task<PartInstance> GetPartInstance(int partInstanceId);
        Task<Transaction> GetLastOutgoing(int partInstanceId);
        Task Load();
    }
}
