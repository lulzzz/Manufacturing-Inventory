using ManufacturingInventory.Application.Boundaries.PartDetails;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {
    public class PartSummaryEdit : IPartSummaryEditUseCase {
        private IRepository<Part> _partRepository;
        private IEntityProvider<Location> _locationProvider;
        private IEntityProvider<Category> _categoryProvider;
        private IUnitOfWork _unitOfWork;

        public PartSummaryEdit(IRepository<Part> partRepository, 
            IEntityProvider<Location> locationProvider, 
            IEntityProvider<Category> categoryProvider,
            IUnitOfWork unitOfWork) {

            this._partRepository = partRepository;
            this._locationProvider = locationProvider;
            this._categoryProvider = categoryProvider;
            this._unitOfWork = unitOfWork;
        }

        public async Task<PartSummaryEditOutput> Execute(PartSummaryEditInput input) {
            //Needs Implementation
            return null;
        }

        public async Task<Part> GetPart(int id) {
            return await this._partRepository.GetEntityAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Category>> GetCategories() {
            return await this._categoryProvider.GetEntityListAsync();
        }

        public async Task<IEnumerable<Warehouse>> GetWarehouses() {
            return (await this._locationProvider.GetEntityListAsync()).OfType<Warehouse>();
        }
    }
}
