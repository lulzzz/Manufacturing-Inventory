using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;

namespace ManufacturingInventory.Application.Boundaries.DistributorManagment { 
    public interface IDistributorEditUseCase:IUseCase<DistributorEditInput,DistributorEditOutput> {
        Task<IEnumerable<Price>> GetPrices(int distributorId);
        Task<Distributor> GetDistributor(int distributorId);
        Task<IEnumerable<Contact>> GetContacts(int distributorId);
        Task Load();
    }
}
