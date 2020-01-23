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
        private ManufacturingContext _context;
        private IRepository<Part> _partRepository;
        private IEntityProvider<Location> _locationProvider;
        private IEntityProvider<Category> _categoryProvider;
        private IUnitOfWorkV2 _unitOfWork;

        public PartSummaryEdit(ManufacturingContext context) {
            this._context = context;
            this._partRepository =new PartRepository(context);
            this._locationProvider = new LocationProvider(context);
            this._categoryProvider = new CategoryProvider(context);
            this._unitOfWork =new UnitOfWorkV2(context);
        }

        public PartSummaryEditOutput Execute(PartSummaryEditInput input) {
            if (input.IsNew) {
                return this.ExecuteNewPart(input);
            } else {
                return this.ExecuteEditPart(input);
            }
        }

        public async Task<PartSummaryEditOutput> ExecuteAsync(PartSummaryEditInput input) {
            if (input.IsNew) {
                return await this.ExecuteNewPartAsync(input);
            } else {
                return await this.ExecuteEditPartAsync(input);
            }
        }

        private PartSummaryEditOutput ExecuteNewPart(PartSummaryEditInput input) {
            var instanceCheck = this._partRepository.GetEntity(e => e.Id == input.PartId);
            if (instanceCheck != null) {
                return new PartSummaryEditOutput(null, false, "Part Already Exist");
            }
            Part part = new Part();
            part.Name = input.Name;
            part.Description = input.Description;
            part.HoldsBubblers = input.HoldsBubblers;

            part.WarehouseId = input.WarehouseId;
            part.UsageId = input.UsageId;
            part.OgranizationId = input.OrganizationId;

            var newPart=this._partRepository.Add(part);
            if (newPart != null) {
                this._unitOfWork.Save();
                return new PartSummaryEditOutput(newPart, true, "Part "+newPart.Name+" Created Successfully");
            } else {
                this._unitOfWork.Undo();
                return new PartSummaryEditOutput(null, false, "Error Saving New Part, Please Contact Admin");
            }
        }

        private PartSummaryEditOutput ExecuteEditPart(PartSummaryEditInput input) {
            var instance = this._partRepository.GetEntity(e => e.Id == input.PartId);
            if (instance == null) {
                return new PartSummaryEditOutput(null, false, "Part Not Found");
            }

            instance.Name = input.Name;
            instance.Description = input.Description;
            instance.HoldsBubblers = input.HoldsBubblers;
            instance.WarehouseId = input.WarehouseId;
            instance.UsageId = input.UsageId;
            instance.OgranizationId = input.OrganizationId;

            var updated = this._partRepository.Update(instance);
            if (updated != null) {
                this._unitOfWork.Save();
                return new PartSummaryEditOutput(updated, true, "Part " + updated.Name + " Updated Successfully");
            } else {
                this._unitOfWork.Undo();
                return new PartSummaryEditOutput(null, false, "Error Saving Part, Please Contact Admin");
            }
        }

        private async Task<PartSummaryEditOutput> ExecuteNewPartAsync(PartSummaryEditInput input) {
            var instanceCheck = await this._partRepository.GetEntityAsync(e => e.Id == input.PartId);

            if (instanceCheck != null) {
                return new PartSummaryEditOutput(null, false, "Part Already Exist");
            }
            Part part = new Part();
            part.Name = input.Name;
            part.Description = input.Description;
            part.HoldsBubblers = input.HoldsBubblers;

            if (input.WarehouseId.HasValue) {

            }

            if (input.OrganizationId.HasValue) {

            }

            if (input.UsageId.HasValue) {

            }

            part.WarehouseId = input.WarehouseId;
            part.UsageId = input.UsageId;
            part.OgranizationId = input.OrganizationId;

            var newPart = await this._partRepository.AddAsync(part);
            if (newPart != null) {
                await this._unitOfWork.SaveAsync();
                return new PartSummaryEditOutput(newPart, true, "Part " + newPart.Name + " Created Successfully");
            } else {
                await this._unitOfWork.UndoAsync();
                return new PartSummaryEditOutput(null, false, "Error Saving New Part, Please Contact Admin");
            }
        }

        private async Task<PartSummaryEditOutput> ExecuteEditPartAsync(PartSummaryEditInput input) {
            var instance = await this._partRepository.GetEntityAsync(e => e.Id == input.PartId);
            if (instance == null) {
                return new PartSummaryEditOutput(null, false, "Part Not Found");
            }

            instance.Name = input.Name;
            instance.Description = input.Description;
            instance.HoldsBubblers = input.HoldsBubblers;
            instance.WarehouseId = input.WarehouseId;
            instance.UsageId = input.UsageId;
            instance.OgranizationId = input.OrganizationId;

            var updated = await this._partRepository.UpdateAsync(instance);
            if (updated != null) {
                await this._unitOfWork.SaveAsync();
                return new PartSummaryEditOutput(updated, true, "Part " + updated.Name + " Updated Successfully");
            } else {
                await this._unitOfWork.UndoAsync();
                return new PartSummaryEditOutput(null, false, "Error Saving Part, Please Contact Admin");
            }
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
