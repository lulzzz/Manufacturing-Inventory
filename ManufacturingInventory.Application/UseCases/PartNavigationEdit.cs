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
        private IRepository<Part> _repository;
        private IUnitOfWork _unitOfWork;

        public PartNavigationEdit(IRepository<Part> repository, IUnitOfWork unitOfWork) {
            this._repository = repository;
            this._unitOfWork = unitOfWork;
        }

        public Task<PartNavigationEditOutput> Execute(PartNavigationEditInput input) {
            return null;
        }
        
        public async Task<Part> GetPart(int id) {
            return await this._repository.GetEntityAsync(e => e.Id == id);
        }
                
        public async Task<IEnumerable<Part>> GetParts() {
            return await this._repository.GetEntityListAsync();
        }
    }
}
