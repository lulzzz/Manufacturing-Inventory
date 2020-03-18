using ManufacturingInventory.Application.Boundaries.DistributorManagment;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Providers;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {
    public class DistributorEdit : IDistributorEditUseCase {
        private ManufacturingContext _context;
        private IRepository<Distributor> _distributorRepository;
        private IEntityProvider<Price> _priceProvider;
        private IEntityProvider<Contact> _contactProvider;
        private IUnitOfWork _unitOfWork;

        public DistributorEdit(ManufacturingContext context) {
            this._context = context;
            this._distributorRepository = new DistributorRepository(context);
            this._priceProvider = new PriceProvider(context);
            this._contactProvider = new ContactProvider(context);
            this._unitOfWork = new UnitOfWork(context);
        }

        public async Task<DistributorEditOutput> Execute(DistributorEditInput input) {
            switch (input.EditAction) {
                case Boundaries.EditAction.Add:
                    return await this.ExecuteNew(input);
                case Boundaries.EditAction.Delete:
                    return await this.ExecuteDelete(input);
                case Boundaries.EditAction.Update:
                    return await this.ExecuteUpdate(input);
                default:
                    return new DistributorEditOutput(null, false, "Invalid Selection");
            }
        }

        public async Task<DistributorEditOutput> ExecuteNew(DistributorEditInput input) {
            var distributor = new Distributor(input.Name, input.Description);
            var added = await this._distributorRepository.AddAsync(distributor);
            if (added != null) {
                var count = await this._unitOfWork.Save();
                if (count > 0) {
                    return new DistributorEditOutput(added,true, "Success: Distributor Added");
                } else {
                    await this._unitOfWork.Undo();
                    return new DistributorEditOutput(null, false, "Error: Add Failed, Please Contact Admin");
                }
            } else {
                await this._unitOfWork.Undo();
                return new DistributorEditOutput(null, false, "Error: Add Failed, Please Contact Admin");
            }
        }

        public async Task<DistributorEditOutput> ExecuteUpdate(DistributorEditInput input) {
            var distributor = await this._distributorRepository.GetEntityAsync(e => e.Id == input.DistributorId);
            distributor.Name = input.Name;
            distributor.Description = input.Description;
            var updated = await this._distributorRepository.UpdateAsync(distributor);
            if (updated != null) {
                var count = await this._unitOfWork.Save();
                if (count > 0) {
                    return new DistributorEditOutput(updated, true, "Success: Distributor Updated");
                } else {
                    await this._unitOfWork.Undo();
                    return new DistributorEditOutput(null, false, "Error: Update Failed, Please Contact Admin");
                }
            } else {
                await this._unitOfWork.Undo();
                return new DistributorEditOutput(null, false, "Error: Update Failed, Please Contact Admin");
            }
        }

        public async Task<DistributorEditOutput> ExecuteDelete(DistributorEditInput input) {
            return new DistributorEditOutput(null, false, "Error: Not Implemented Yet");
        }


        public async Task<IEnumerable<ContactDTO>> GetContacts(int distributorId) {
            return (await this._contactProvider.GetEntityListAsync(e=>e.DistributorId==distributorId)).Select(contact=>new ContactDTO(contact));
        }

        public async Task<Distributor> GetDistributor(int distributorId) {
            return await this._distributorRepository.GetEntityAsync(e => e.Id == distributorId);
        }

        public async Task<IEnumerable<Distributor>> GetDistributors() {
            return await this._distributorRepository.GetEntityListAsync();
        }

        public async Task<IEnumerable<Price>> GetPrices(int distributorId) {
            return await this._priceProvider.GetEntityListAsync(e => e.DistributorId == distributorId);
        }

        public async Task Load() {
            await this._distributorRepository.LoadAsync();
            await this._priceProvider.LoadAsync();
            await this._contactProvider.LoadAsync();
        }
    }
}
