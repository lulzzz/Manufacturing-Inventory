using ManufacturingInventory.Application.Boundaries.PartNavigationEdit;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {
    public class PartNavigationEdit : IPartNavigationEditUseCase {
        private ManufacturingContext _context;
        private IRepository<Part> _repository;

        public PartNavigationEdit(ManufacturingContext context) {
            this._context = context;
            this._repository = new PartRepository(context);
        }

        public Task<PartNavigationEditOutput> Execute(PartNavigationEditInput input) {
            return null;
        }
        
        public async Task<Part> GetPart(int id) {
            return await this._repository.GetEntityAsync(e => e.Id == id);
        }
                
        public async Task<IEnumerable<Part>> GetPartsAsync() {
            return await this._repository.GetEntityListAsync();
        }

        public IEnumerable<Part> GetParts() {
            return this._repository.GetEntityList();
        }
    }
}
