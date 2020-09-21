using ManufacturingInventory.Application.Boundaries.LocationManage;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Domain.Enums;
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
    public class LocationManagmentUseCase : ILocationManagmentUseCase {
        private ManufacturingContext _context;
        private IRepository<Location> _locationRepository;
        private IUnitOfWork _unitOfWork;

        public LocationManagmentUseCase(ManufacturingContext context) {
            this._context = context;
            this._locationRepository = new LocationRepository(context);
            this._unitOfWork = new UnitOfWork(context);
        }

        public async Task<LocationManagmentOutput> Execute(LocationManagmentInput input) {
            switch (input.EditAction) {
                case Boundaries.EditAction.Add:
                    return new LocationManagmentOutput(null, false, "Internal Error: Action not supported in this UseCase");
                case Boundaries.EditAction.Delete:
                    return await this.ExecuteDelete(input);
                case Boundaries.EditAction.Update:
                    return new LocationManagmentOutput(null, false, "Internal Error: Action not supported in this UseCase");
                default:
                    return new LocationManagmentOutput(null, false, "Internal Error: Action not implemented");
            }
        }

        private async Task<LocationManagmentOutput> ExecuteDelete(LocationManagmentInput input) {
            var location = await this._locationRepository.GetEntityAsync(e => e.Id == input.Location.Id);
            if (location != null) {
                if (!location.IsDefualt) {
                    var deleted = await this._locationRepository.DeleteAsync(location);
                    if (deleted != null) {
                        var count = await this._unitOfWork.Save();
                        if (count > 0) {
                            return new LocationManagmentOutput(new LocationDto(deleted), true, "Success: Location Updated, Reloading..");
                        } else {
                            return new LocationManagmentOutput(null, false, "Internal Error: Error while saving location, please contact admin");
                        }
                    } else {
                        return new LocationManagmentOutput(null, false, "Internal Error: Error while updating location, please contact admin");
                    }
                } else {
                    return new LocationManagmentOutput(null, false, "Error: Cannot delete default location, please change default location then try again");
                }
            } else {
                return new LocationManagmentOutput(null, false, "Error: Could not find location to update");
            }
        }

        public async Task<IEnumerable<LocationDto>> GetLocations() {
            return (await this._locationRepository.GetEntityListAsync()).Select(location => new LocationDto(location));
        }

        public async Task Load() {
            await this._locationRepository.LoadAsync();
        }
    }
}
