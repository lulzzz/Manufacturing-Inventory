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
        private IEntityProvider<PartInstance> _instanceProvider;
        private IEntityProvider<Part> _partProvider;
        private IEntityProvider<Transaction> _transactionProvider;
        private IUnitOfWork _unitOfWork;

        public LocationManagmentUseCase(ManufacturingContext context) {
            this._context = context;
            this._locationRepository = new LocationRepository(context);
            this._instanceProvider = new PartInstanceProvider(context);
            this._partProvider = new PartProvider(context);
            this._transactionProvider = new TransactionProvider(context);
            this._unitOfWork = new UnitOfWork(context);
        }

        public async Task<LocationManagmentOutput> Execute(LocationManagmentInput input) {
            switch (input.EditAction) {
                case Boundaries.EditAction.Add:
                    return await this.ExecuteAdd(input);
                case Boundaries.EditAction.Delete:
                    return await this.ExecuteDelete(input);
                case Boundaries.EditAction.Update:
                    return await this.ExecuteUpdate(input);
                default:
                    return new LocationManagmentOutput(null, false, "Internal Error: Action not implemented");
            }
        }

        private async Task<LocationManagmentOutput> ExecuteAdd(LocationManagmentInput input) {
            Location location;
            switch (input.Location.LocationType) {
                case LocationType.Warehouse:
                    location = new Warehouse();
                    break;
                case LocationType.Consumer:
                    location = new Consumer();
                    break;
                case LocationType.NotSelected:
                    return new LocationManagmentOutput(null, false, "Error: Must Select LocationType");
                default:
                    return new LocationManagmentOutput(null, false, "Internal Error: Action not implemented");
            }
            location.Id = input.Location.Id;
            location.Name = input.Location.Name;
            location.Description = input.Location.Description;
            location.IsDefualt = input.Location.IsDefualt;
            var added = await this._locationRepository.AddAsync(location);
            if (added != null) {
                var count = await this._unitOfWork.Save();
                if (count > 0) {
                    return new LocationManagmentOutput(new LocationDto(added), true, "Success: Location Created, Reloading..");
                } else {
                    return new LocationManagmentOutput(null, false, "Internal Error: Error while saving location, please contact admin");
                }
            } else {
                return new LocationManagmentOutput(null, false, "Internal Error: Error while adding location, please contact admin");
            }
        }

        private async Task<LocationManagmentOutput> ExecuteUpdate(LocationManagmentInput input) {
            var location = await this._locationRepository.GetEntityAsync(e => e.Id == input.Location.Id);
            if (location != null) {
                //location.Id = input.Location.Id;
                location.Name = input.Location.Name;
                location.Description = input.Location.Description;
                location.IsDefualt = input.Location.IsDefualt;
                var updated = await this._locationRepository.UpdateAsync(location);
                if (updated != null) {
                    var count = await this._unitOfWork.Save();
                    if (count > 0) {
                        return new LocationManagmentOutput(new LocationDto(updated), true, "Success: Location Updated, Reloading..");
                    } else {
                        return new LocationManagmentOutput(null, false, "Internal Error: Error while saving location, please contact admin");
                    }
                } else {
                    return new LocationManagmentOutput(null, false, "Internal Error: Error while updating location, please contact admin");
                }
            } else {
                return new LocationManagmentOutput(null, false, "Error: Could not find location to update");
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

        public async Task<LocationDto> GetLocation(int locationId) {
            var temp=await this._locationRepository.GetEntityAsync(e => e.Id == locationId);
            return new LocationDto(temp);
        }

        public async Task<IEnumerable<LocationDto>> GetLocations() {
            return (await this._locationRepository.GetEntityListAsync()).Select(location => new LocationDto(location));
        }

        public async Task<IEnumerable<InstanceDto>> GetLocationInstances(int locationId) {
            return (await this._instanceProvider.GetEntityListAsync(e => e.CurrentLocation.Id == locationId)).Select(instance => new InstanceDto(instance));
        }

        public async Task<IEnumerable<PartDto>> GetLocationParts(int locationId) {
            return (await this._partProvider.GetEntityListAsync(e => e.WarehouseId == locationId)).Select(part => new PartDto(part));
        }

        public async Task<IEnumerable<TransactionDTO>> GetLocationTransactions(int locationId) {
            return (await this._transactionProvider.GetEntityListAsync(e => e.LocationId == locationId)).Select(transaction => new TransactionDTO(transaction));
        }

        public async Task Load() {
            await this._locationRepository.LoadAsync();
            await this._instanceProvider.LoadAsync();
            await this._partProvider.LoadAsync();
        }
    }
}
