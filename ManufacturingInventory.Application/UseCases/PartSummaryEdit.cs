using ManufacturingInventory.Application.Boundaries.PartDetails;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Providers;
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
        private IUnitOfWork _unitOfWork;

        public PartSummaryEdit(ManufacturingContext context) {
            this._context = context;
            this._partRepository =new PartRepository(context);
            this._locationProvider = new LocationProvider(context);
            this._categoryProvider = new CategoryProvider(context);
            this._unitOfWork =new UnitOfWork(context);
        }

        //public PartSummaryEditOutput Execute(PartSummaryEditInput input) {
        //    if (input.IsNew) {
        //        return this.ExecuteNewPart(input);
        //    } else {
        //        return this.ExecuteEditPart(input);
        //    }
        //}

        public async Task<PartSummaryEditOutput> Execute(PartSummaryEditInput input) {
            if (input.IsNew) {
                return await this.ExecuteNewPart(input);
            } else {
                return await this.ExecuteEditPart(input);
            }
        }

        //private PartSummaryEditOutput ExecuteNewPart(PartSummaryEditInput input) {
        //    var instanceCheck = this._partRepository.GetEntity(e => e.Id == input.PartId);
        //    if (instanceCheck != null) {
        //        return new PartSummaryEditOutput(null, false, "Part Already Exist");
        //    }
        //    Part part = new Part();
        //    part.Name = input.Name;
        //    part.Description = input.Description;
        //    part.HoldsBubblers = input.HoldsBubblers;
        //    part.DefaultToCostReported = input.CostReportedDefault;

        //    var warehouse = this._locationProvider.GetEntity(e => e.Id == input.WarehouseId);
        //    var org = this._categoryProvider.GetEntity(e => e.Id == input.OrganizationId);
        //    var usage = this._categoryProvider.GetEntity(e => e.Id == input.UsageId);

        //    part.Warehouse =(Warehouse) warehouse;
        //    part.Usage =(Usage) usage;
        //    part.Organization = (Organization)org;

        //    var newPart=this._partRepository.Add(part);
        //    if (newPart != null) {
        //        this._unitOfWork.Save();
        //        return new PartSummaryEditOutput(newPart, true, "Part "+newPart.Name+" Created Successfully");
        //    } else {
        //        this._unitOfWork.Undo();
        //        return new PartSummaryEditOutput(null, false, "Error Saving New Part, Please Contact Admin");
        //    }
        //}

        //private PartSummaryEditOutput ExecuteEditPart(PartSummaryEditInput input) {
        //    var instance = this._partRepository.GetEntity(e => e.Id == input.PartId);
        //    if (instance == null) {
        //        return new PartSummaryEditOutput(null, false, "Part Not Found");
        //    }

        //    instance.Name = input.Name;
        //    instance.Description = input.Description;
        //    instance.HoldsBubblers = input.HoldsBubblers;


        //    var warehouse = this._locationProvider.GetEntity(e => e.Id == input.WarehouseId);
        //    var org = this._categoryProvider.GetEntity(e => e.Id == input.OrganizationId);
        //    var usage = this._categoryProvider.GetEntity(e => e.Id == input.UsageId);

        //    instance.Warehouse = (Warehouse)warehouse;
        //    instance.Usage = (Usage)usage;
        //    instance.Organization = (Organization)org;

        //    var updated = this._partRepository.Update(instance);
        //    if (updated != null) {
        //        this._unitOfWork.Save();
        //        return new PartSummaryEditOutput(updated, true, "Part " + updated.Name + " Updated Successfully");
        //    } else {
        //        this._unitOfWork.Undo();
        //        return new PartSummaryEditOutput(null, false, "Error Saving Part, Please Contact Admin");
        //    }
        //}

        private async Task<PartSummaryEditOutput> ExecuteNewPart(PartSummaryEditInput input) {
            var instanceCheck = await this._partRepository.GetEntityAsync(e => e.Id == input.PartId);

            if (instanceCheck != null) {
                return new PartSummaryEditOutput(null, false, "Part Already Exist");
            }

            Part part = new Part();
            part.Name = input.Name;
            part.Description = input.Description;
            part.HoldsBubblers = input.HoldsBubblers;
            part.DefaultToCostReported = input.CostReportedDefault;

            if (input.WarehouseId != 0) {
                part.WarehouseId = input.WarehouseId;
            }

            //if (input.UsageId != 0) {
            //    part.UsageId = input.UsageId;
            //}

            if (input.OrganizationId != 0) {
                part.OrganizationId = input.OrganizationId;
            }

            var newPart = await this._partRepository.AddAsync(part);
            if (newPart != null) {
                await this._unitOfWork.Save();
                return new PartSummaryEditOutput(newPart, true, "Part " + newPart.Name + " Created Successfully");
            } else {
                await this._unitOfWork.Undo();
                return new PartSummaryEditOutput(null, false, "Error Saving New Part, Please Contact Admin");
            }
        }

        private async Task<PartSummaryEditOutput> ExecuteEditPart(PartSummaryEditInput input) {
            var part = await this._partRepository.GetEntityAsync(e => e.Id == input.PartId);
            if (part == null) {
                return new PartSummaryEditOutput(null, false, "Part Not Found");
            }

            part.Name = input.Name;
            part.Description = input.Description;
            part.HoldsBubblers = input.HoldsBubblers;
            part.DefaultToCostReported = input.CostReportedDefault;

            if (input.WarehouseId != 0) {
                part.WarehouseId = input.WarehouseId;
            }

            //if(input.UsageId != 0) {
            //    part.UsageId = input.UsageId;
            //}

            if(input.OrganizationId != 0) {
                part.OrganizationId = input.OrganizationId;
            }

            var updated = await this._partRepository.UpdateAsync(part);
            if (updated != null) {
                await this._unitOfWork.Save();
                return new PartSummaryEditOutput(updated, true, "Part " + updated.Name + " Updated Successfully");
            } else {
                await this._unitOfWork.Save();
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
