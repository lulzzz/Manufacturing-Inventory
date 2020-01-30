using ManufacturingInventory.Application.Boundaries.PriceEdit;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {
    public class PriceEdit : IPriceEditUseCase {
        private ManufacturingContext _context;
        private IRepository<Price> _priceRepository;

        public Task<PriceEditOutput> Execute(PriceEditInput input) => throw new NotImplementedException();
        public Task<IEnumerable<Attachment>> GetAttachments() => throw new NotImplementedException();
        public Task<IEnumerable<Distributor>> GetDistributors() => throw new NotImplementedException();
        public Task Load() => throw new NotImplementedException();
    }
}
