﻿using ManufacturingInventory.Application.Boundaries.PriceEdit;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Providers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ManufacturingInventory.Application.UseCases {
    public class PriceEdit : IPriceEditUseCase {
        private ManufacturingContext _context;
        private IRepository<Price> _priceRepository;
        private IRepository<PartInstance> _partInstanceRepository;
        private IRepository<PartPrice> _partPriceRepository;
        private IRepository<PriceLog> _priceLogRepository;
        private IEntityProvider<Distributor> _distributorProvider;
        private IRepository<Part> _partRepository;
        private IUnitOfWork _unitOfWork;

        public PriceEdit(ManufacturingContext context) {
            this._context = context;
            this._priceRepository = new PriceRepository(context);
            this._partInstanceRepository = new PartInstanceRepository(context);
            this._partPriceRepository = new PartPriceRepository(context);
            this._partRepository = new PartRepository(context);
            this._distributorProvider = new DistributorProvider(context);
            this._priceLogRepository = new PriceLogRepository(context);
            this._unitOfWork = new UnitOfWork(context);
        }

        public async Task<PriceEditOutput> Execute(PriceEditInput input) {
            switch (input.EditAction) {
                case PriceEditOption.NEW:
                    return await this.ExecuteNew(input);
                case PriceEditOption.ReplaceWithNew:
                    return await this.ExecuteReplaceWithNew(input);
                case PriceEditOption.ReplaceWithExisiting:
                    return await this.ExecuteReplaceWithExisiting(input);
                case PriceEditOption.Edit:
                    return await this.ExecuteEdit(input);
                case PriceEditOption.View:
                    return new PriceEditOutput(null, false, "Error: PriceEditOption View Not Supported");
                default:
                    return new PriceEditOutput(null, false, "Error: Unknown PriceEditOption");
            }
        }


        public async Task<PriceEditOutput> ExecuteNew(PriceEditInput input) {
            var part = await this._partRepository.GetEntityAsync(e => e.Id == input.PartId);

            if (part == null) {
                return new PriceEditOutput(null, false, "Error: Part Not Found");
            }

            Price price = new Price();
            price.LeadTime = input.LeadTime;
            price.MinOrder = input.MinOrder;
            price.TimeStamp = input.TimeStamp;
            price.UnitCost = input.UnitCost;
            price.ValidFrom = input.ValidFrom;
            price.ValidUntil = input.ValidUntil;
            price.DistributorId = input.DistributorId;

            if (input.PartInstanceId.HasValue) {
                var partInstance = await this._partInstanceRepository.GetEntityAsync(e => e.Id == input.PartInstanceId.Value);
                if (partInstance != null) {
                    price.PartInstances.Add(partInstance);
                    partInstance.UnitCost = price.UnitCost;
                    if (partInstance.IsBubbler) {
                        partInstance.TotalCost = (price.UnitCost * partInstance.BubblerParameter.NetWeight) * partInstance.Quantity;
                    } else {
                        partInstance.TotalCost = price.UnitCost * partInstance.Quantity;
                    }
                    PriceLog priceLog = new PriceLog(partInstance, price);
                    await this._priceLogRepository.AddAsync(priceLog);
                    await this._partInstanceRepository.UpdateAsync(partInstance);
                } else {
                    return new PriceEditOutput(null, false, "Error: PartInstance Not Found");
                }
            } else {
                return new PriceEditOutput(null, false, "Error: PartInstance Not Found");
            }

            var priceAdded = await this._priceRepository.AddAsync(price);
            PartPrice partPrice = new PartPrice(part, price);
            var partPriceAdded = await this._partPriceRepository.AddAsync(partPrice);
            if (priceAdded != null && partPriceAdded != null) {
                var count = await this._unitOfWork.Save();
                return new PriceEditOutput(priceAdded, true, "Price Added and Saved");
            } else {
                await this._unitOfWork.Undo();
                return new PriceEditOutput(null, false, "Error: New Price Save Failed");
            }
        }

        public async Task<PriceEditOutput> ExecuteReplaceWithNew(PriceEditInput input) {
            var part = await this._partRepository.GetEntityAsync(e => e.Id == input.PartId);

            if (part == null) {
                return new PriceEditOutput(null, false, "Error: Part Not Found");
            }

            Price price = new Price();
            price.LeadTime = input.LeadTime;
            price.MinOrder = input.MinOrder;
            price.TimeStamp = input.TimeStamp;
            price.UnitCost = input.UnitCost;
            price.ValidFrom = input.ValidFrom;
            price.ValidUntil = input.ValidUntil;
            price.DistributorId = input.DistributorId;

            if (input.PartInstanceId.HasValue) {
                var partInstance = await this._partInstanceRepository.GetEntityAsync(e => e.Id == input.PartInstanceId.Value);
                if (partInstance != null) {
                    var priceLog = new PriceLog(partInstance, price);
                    var logs =await this._priceLogRepository.GetEntityListAsync(e => e.PartInstanceId == partInstance.Id);              
                    foreach(var log in logs) {
                        log.IsCurrent = false;
                        this._priceLogRepository.Update(log);
                    }
                    price.PartInstances.Add(partInstance);
                    partInstance.UnitCost = price.UnitCost;
                    if (partInstance.IsBubbler) {
                        partInstance.TotalCost = (price.UnitCost * partInstance.BubblerParameter.NetWeight) * partInstance.Quantity;
                    } else {
                        partInstance.TotalCost = price.UnitCost * partInstance.Quantity;
                    }
                    await this._priceLogRepository.AddAsync(priceLog);
                    await this._partInstanceRepository.UpdateAsync(partInstance);
                } else {
                    return new PriceEditOutput(null, false, "Error: PartInstance Not Found");
                }
            } else {
                return new PriceEditOutput(null, false, "Error: PartInstance Not Found");
            }
            PartPrice partPrice = new PartPrice(part, price);
            var priceAdded = await this._priceRepository.AddAsync(price);
            var partPriceAdded = await this._partPriceRepository.AddAsync(partPrice);
            if (priceAdded != null && partPriceAdded != null) {
                var count = await this._unitOfWork.Save();
                return new PriceEditOutput(priceAdded, true, "Price Added and Saved");
            } else {
                await this._unitOfWork.Undo();
                return new PriceEditOutput(null, false, "Error: New Price Save Failed");
            }
        }

        public async Task<PriceEditOutput> ExecuteEdit(PriceEditInput input) {
            var price = await this._priceRepository.GetEntityAsync(e => e.Id == input.PriceId.Value);
            if (price != null) {
                price.LeadTime = input.LeadTime;
                price.MinOrder = input.MinOrder;
                price.TimeStamp = input.TimeStamp;
                price.UnitCost = input.UnitCost;
                price.ValidFrom = input.ValidFrom;
                price.ValidUntil = input.ValidUntil;
                price.DistributorId = input.DistributorId;
                if (input.PartInstanceId.HasValue) {
                    var partInstance = await this._partInstanceRepository.GetEntityAsync(e => e.Id == input.PartInstanceId.Value);
                    if (partInstance != null) {
                        partInstance.UpdatePrice(price.Id,price.UnitCost);
                        await this._partInstanceRepository.UpdateAsync(partInstance);
                    } else {
                        return new PriceEditOutput(null, false, "Internal Error: PartInstance Not Found");
                    }
                } else {
                    return new PriceEditOutput(null, false, "Internal Error: Part Instance Not Found");
                }
                var updated = await this._priceRepository.UpdateAsync(price);
                if (updated != null) {
                    var count = await this._unitOfWork.Save();
                    return new PriceEditOutput(updated, true, "Price Changes Saved");
                } else {
                    await this._unitOfWork.Undo();
                    return new PriceEditOutput(null, false, "Price Save Failed");
                }
            } else {
                return new PriceEditOutput(null, false, "Error: Price Not Found");
            }
        }

        public async Task<PriceEditOutput> ExecuteReplaceWithExisiting(PriceEditInput input) {
            var price = await this._priceRepository.GetEntityAsync(e => e.Id == input.PriceId.Value);
            if (price != null) {
                if (input.PartInstanceId.HasValue) {
                    var partInstance = await this._partInstanceRepository.GetEntityAsync(e => e.Id == input.PartInstanceId.Value);
                    if (partInstance != null) {
                        var priceLog = new PriceLog(partInstance, price);
                        var currentPriceLog = partInstance.PriceLogs.FirstOrDefault(e => e.IsCurrent);
                        if (currentPriceLog != null) {
                            currentPriceLog.IsCurrent = false;
                            await this._priceLogRepository.UpdateAsync(currentPriceLog);
                        }
                        partInstance.UpdatePrice(price.Id, price.UnitCost);
                        await this._priceLogRepository.AddAsync(priceLog);
                        await this._partInstanceRepository.UpdateAsync(partInstance);
                    } else {
                        return new PriceEditOutput(null, false, "Error: PartInstance Not Found");
                    }
                } else {
                    return new PriceEditOutput(null, false, "Error: PartInstance Not Found");
                }

                var updated = await this._priceRepository.UpdateAsync(price);
                if (updated != null) {
                    var count = await this._unitOfWork.Save();
                    return new PriceEditOutput(updated, true, "Price Changes Saved");
                } else {
                    await this._unitOfWork.Undo();
                    return new PriceEditOutput(null, false, "Price Save Failed");
                }
            } else {
                return new PriceEditOutput(null, false, "Error: Price Not Found");
            }
        }

        public async Task<IEnumerable<Distributor>> GetDistributors() {
            return await this._distributorProvider.GetEntityListAsync();
        }

        public async Task<IEnumerable<Price>> GetPartPrices(int partId) {
            return (await this._partPriceRepository.GetEntityListAsync(e => e.PartId == partId)).Select(e => e.Price);
        }

        public async Task<Price> GetPrice(int priceId) {
            return await this._priceRepository.GetEntityAsync(e => e.Id == priceId);
        }

        public async Task Load() {
            await this._priceRepository.LoadAsync();
        }
    }
}
