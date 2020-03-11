using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.Boundaries.DistributorManagment {
    public interface IDistributornNavigationUseCase {
        Task<IEnumerable<Distributor>> GetDistributors();
        Task Load();
    }
}
