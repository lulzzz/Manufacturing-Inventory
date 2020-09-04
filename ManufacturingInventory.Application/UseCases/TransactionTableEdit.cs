using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Application.Boundaries.TransactionTableEdit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Infrastructure.Model.Providers;
using System.Linq;

namespace ManufacturingInventory.Application.UseCases {
    public class TransactionTableEdit : ITransactionTableUndoUseCase {
        private IRepository<Transaction> _transactionRepository;
        private IRepository<PartInstance> _partInstanceRepository;
        private IRepository<Location> _locationRepository;
        private IRepository<BubblerParameter> _bubblerRepository;
        private IRepository<Category> _categoryRepository;
        private IEntityProvider<Part> _partProvider;
        private IUnitOfWork _unitOfWork;
        private ManufacturingContext _context;

        public TransactionTableEdit(ManufacturingContext context) {
            this._context = context; 
            this._transactionRepository = new TransactionRepository(context);
            this._partInstanceRepository = new PartInstanceRepository(context);
            this._locationRepository = new LocationRepository(context);
            this._bubblerRepository = new BubblerParameterRepository(context);
            this._partProvider = new PartProvider(context);
            this._categoryRepository = new CategoryRepository(context);
            this._unitOfWork = new UnitOfWork(context);
        }

        public async Task<TransactionUndoOutput> Execute(TransactionTableUndoInput input) {
            var transaction = await this._transactionRepository.GetEntityAsync(e => e.Id == input.TransactionId);         
            if (transaction != null) {
                var partInstance = await this._partInstanceRepository.GetEntityAsync(e => e.Id == transaction.PartInstanceId);
                if (partInstance != null) {
                    
                    switch (transaction.InventoryAction) {
                        case InventoryAction.OUTGOING:
                            return await this.ExecuteUndoOutgoing(partInstance,transaction);
                        case InventoryAction.INCOMING:
                            var incount=partInstance.Transactions.Where(e => e.InventoryAction == InventoryAction.INCOMING).Count();
                            var outCount = partInstance.Transactions
                                .Where(e => 
                                e.InventoryAction == InventoryAction.OUTGOING || e.InventoryAction == InventoryAction.RETURNING)
                                .Count();

                            if (incount==1 && outCount==0) {                              
                                return await this.ExecuteDelete(partInstance, transaction);
                            }else if(incount==1 && outCount>0) {

                            }else if(incount>1) {
                                //
                            }
                        case InventoryAction.RETURNING:
                            return await this.ExecuteUndoReturn(partInstance, transaction);
                        default:
                            return new TransactionUndoOutput(null, false, "Internal Error: Not a valid inventory action");
                    }
                } else {
                    return new TransactionUndoOutput(null, false, "Internal Error: Could not fin PartInstance");
                }

            } else {
                return new TransactionUndoOutput(null, false, "Error: Internal error, could not find transaction");
            }

        }

        private async Task<TransactionUndoOutput> ExecuteDelete(PartInstance partInstance,Transaction transaction) {
            var stockType = (StockType)await this._categoryRepository.GetEntityAsync(e => e.Id == partInstance.StockTypeId);
            if (stockType != null) {
                if (!stockType.IsDefault) {
                    if (partInstance.IsBubbler) {
                        stockType.Quantity -= (int)partInstance.BubblerParameter.Weight;
                    } else {
                        stockType.Quantity -= partInstance.Quantity;
                    }
                } else {
                    this._context.Alerts.Remove(partInstance.IndividualAlert);
                    partInstance.IndividualAlert = null;
                }       
                var instance = await this._partInstanceRepository.DeleteAsync(partInstance);
                var trans = await this._transactionRepository.DeleteAsync(transaction);
                var updated = await this._categoryRepository.UpdateAsync(stockType);
                if (updated != null && instance != null && trans != null) {
                    var count = await this._unitOfWork.Save();
                    if (count > 0) {
                        return new TransactionUndoOutput(null, true, "Success: Transaction Successfully Undone");
                    } else {
                        await this._unitOfWork.Undo();
                        return new TransactionUndoOutput(null, false, "Internal Error: Could not save");
                    }
                } else {
                    await this._unitOfWork.Undo();
                    return new TransactionUndoOutput(null, false, "Internal Error: Could not update Entities");
                }
            } else {
                await this._unitOfWork.Undo();
                return new TransactionUndoOutput(null, false, "Internal Error: Could not find Stock type");
            }
        }

        private async Task<TransactionUndoOutput> ExecuteUndoAddTo(PartInstance partInstance, Transaction transaction) {
            var stockType = (StockType)await this._categoryRepository.GetEntityAsync(e => e.Id == partInstance.StockTypeId);
            if (stockType != null) {
                partInstance.UpdateQuantity(0 - transaction.Quantity);
                if (!stockType.IsDefault) {
                    stockType.Quantity -= transaction.Quantity;
                }
                var instance = await this._partInstanceRepository.UpdateAsync(partInstance);
                var trans = await this._transactionRepository.DeleteAsync(transaction);
                var updated = await this._categoryRepository.UpdateAsync(stockType);
                if (updated != null && instance != null && trans != null) {
                    var count = await this._unitOfWork.Save();
                    if (count > 0) {
                        return new TransactionUndoOutput(null, true, "Success: Transaction Successfully Undone");
                    } else {
                        await this._unitOfWork.Undo();
                        return new TransactionUndoOutput(null, false, "Internal Error: Could not save");
                    }
                } else {
                    await this._unitOfWork.Undo();
                    return new TransactionUndoOutput(null, false, "Internal Error: Could not update Entities");
                }
            } else {
                await this._unitOfWork.Undo();
                return new TransactionUndoOutput(null, false, "Internal Error: Could not find Stock type");
            }
        }

        private async Task<TransactionUndoOutput> ExecuteUndoOutgoing(PartInstance partInstance, Transaction transaction) {
            var location = await this._locationRepository.GetEntityAsync(e => e.Id == transaction.PartInstance.Part.WarehouseId);

            var stockType = (StockType)await this._categoryRepository.GetEntityAsync(e => e.Id == partInstance.StockTypeId);
            if (stockType != null) {
                if (location != null) {
                    partInstance.LocationId = location.Id;
                }
                if (partInstance.IsBubbler) {
                    partInstance.UpdateQuantity(1);
                    partInstance.UpdateWeight(transaction.MeasuredWeight);
                    partInstance.DateRemoved = null;
                    partInstance.DateInstalled = null;
                    if (!stockType.IsDefault) {
                        stockType.Quantity += (int)partInstance.BubblerParameter.Weight;
                    }
                    var bubbler = await this._bubblerRepository.UpdateAsync(partInstance.BubblerParameter);
                    if (bubbler == null) {
                        await this._unitOfWork.Undo();
                        return new TransactionUndoOutput(null, false, "Internal Error: Could not update BubblerParameter");
                    }
                } else {
                    partInstance.UpdateQuantity(transaction.Quantity);
                    if (!stockType.IsDefault) {
                        stockType.Quantity += transaction.Quantity;
                    }
                }
                var instance = await this._partInstanceRepository.UpdateAsync(partInstance);
                var trans = await this._transactionRepository.DeleteAsync(transaction);
                var updated = await this._categoryRepository.UpdateAsync(stockType);
                if (updated != null && instance != null && trans != null) {
                    var count = await this._unitOfWork.Save();
                    if (count > 0) {
                        return new TransactionUndoOutput(null, true, "Success: Transaction Successfully Undone");
                    } else {
                        await this._unitOfWork.Undo();
                        return new TransactionUndoOutput(null, false, "Internal Error: Could not save");
                    }
                } else {
                    await this._unitOfWork.Undo();
                    return new TransactionUndoOutput(null, false, "Internal Error: Could not update Entities");
                }
            } else {
                await this._unitOfWork.Undo();
                return new TransactionUndoOutput(null, false, "Internal Error: Could not find Stock type");
            }
        }

        private async Task<TransactionUndoOutput> ExecuteUndoReturn(PartInstance partInstance, Transaction transaction) {
            var referenceTransaction = await this._transactionRepository.GetEntityAsync(e => e.Id == transaction.ReferenceTransactionId);
            if (referenceTransaction != null) {
                var location = await this._locationRepository.GetEntityAsync(e => e.Id == referenceTransaction.LocationId);
                var stockType = (StockType)await this._categoryRepository.GetEntityAsync(e => e.Id == partInstance.StockTypeId);
                if (stockType != null) {
                    if (location != null) {
                        partInstance.LocationId = location.Id;
                    }
                    if (partInstance.IsBubbler) {
                        partInstance.UpdateQuantity(-1);
                        partInstance.UpdateWeight(referenceTransaction.MeasuredWeight);
                        partInstance.DateRemoved = null;
                        if (!stockType.IsDefault) {
                            stockType.Quantity -= (int)partInstance.BubblerParameter.Weight;
                        }
                        var bubbler = await this._bubblerRepository.UpdateAsync(partInstance.BubblerParameter);
                        if (bubbler == null) {
                            await this._unitOfWork.Undo();
                            return new TransactionUndoOutput(null, false, "Internal Error: Could not update BubblerParameter");
                        }
                    } else {
                        partInstance.UpdateQuantity(0 - transaction.Quantity);
                        if (!stockType.IsDefault) {
                            stockType.Quantity -= transaction.Quantity;
                        }
                    }
                    var instance = await this._partInstanceRepository.UpdateAsync(partInstance);
                    var trans = await this._transactionRepository.DeleteAsync(transaction);
                    var updated = await this._categoryRepository.UpdateAsync(stockType);
                    if (updated != null && instance != null && trans != null) {
                        var count = await this._unitOfWork.Save();
                        if (count > 0) {
                            return new TransactionUndoOutput(null, true, "Success: Transaction Successfully Undone");
                        } else {
                            await this._unitOfWork.Undo();
                            return new TransactionUndoOutput(null, false, "Internal Error: Could not save");
                        }
                    } else {
                        await this._unitOfWork.Undo();
                        return new TransactionUndoOutput(null, false, "Internal Error: Could not update Entities");
                    }
                } else {
                    await this._unitOfWork.Undo();
                    return new TransactionUndoOutput(null, false, "Internal Error: Could not find Stock type");
                }
            } else {
                return new TransactionUndoOutput(null, false, "Internal Error: Could not find ReferenceTransaction");
            }
        }

        public async Task<IEnumerable<Transaction>> GetTransactions(GetBy by, int id) {
            switch (by) {
                case GetBy.PART:
                    return await this._transactionRepository.GetEntityListAsync(e => e.PartInstance.PartId == id);
                case GetBy.PARTINSTANCE:
                    return await this._transactionRepository.GetEntityListAsync(e => e.PartInstanceId == id);
                default:
                    return null;
            }
        }

        public async Task Load() {
            await this._transactionRepository.LoadAsync();
            await this._partInstanceRepository.LoadAsync();
            await this._locationRepository.LoadAsync();
            await this._partProvider.LoadAsync();
        }
    }
}
