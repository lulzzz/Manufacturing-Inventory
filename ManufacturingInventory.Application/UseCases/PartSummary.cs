using ManufacturingInventory.Application.Boundaries.PartDetails;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {
    public class PartSummary : IPartSummaryUseCase {
        private ManufacturingContext _context;
        private IRepository<Part> _partRepository;
        private IEntityProvider<Location> _locationProvider;
        private IEntityProvider<Category> _categoryProvider;
        private IUnitOfWork _unitOfWork;

        public PartSummary(ManufacturingContext context) {
            this._context = context;
            this._partRepository = new PartRepository(this._context);
            this._locationProvider = new LocationProvider(this._context);
            this._categoryProvider = new CategoryProvider(this._context);
            this._unitOfWork = new UnitOfWork(this._context);
        }

        public async Task<PartSummaryOutput> Execute(PartSummaryInput input) {
            return null;
        }

        public Task<IEnumerable<Organization>> GetOrganizations() => throw new NotImplementedException();
        public Task<IEnumerable<Usage>> GetUsages() => throw new NotImplementedException();
        public Task<IEnumerable<Warehouse>> GetWarehouses() => throw new NotImplementedException();
    }
}
