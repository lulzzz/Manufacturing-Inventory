using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Application.Boundaries.TransactionTableEdit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {
    public class TransactionTableEdit : ITransactionTableUndoUseCase {
        private IRepository<Transaction> _transactionRepository;
        private IRepository<PartInstance> _partInstanceRepository;
        private IUnitOfWork _unitOfWork;
        private ManufacturingContext _context;

        public TransactionTableEdit(ManufacturingContext context) {
            this._context = context; 
            this._transactionRepository = new TransactionRepository(context);
            this._partInstanceRepository = new PartInstanceRepository(context);
            this._unitOfWork = new UnitOfWork(context);
        }

        public async Task<TransactionUndoOutput> Execute(TransactionTableUndoInput input) {
            //var transaction = await this._transactionRepository.GetEntityAsync(e => e.Id == input.TransactionId);
            //if (transaction != null) {
            //    var partInstance = await this._partInstanceRepository.GetEntityAsync(e => e.Id == transaction.PartInstanceId);
            //    if (partInstance != null) {
            //        if (partInstance.IsBubbler) {
            //            switch (transaction.InventoryAction) {
            //                case InventoryAction.OUTGOING: {
            //                    if (!partInstance.DateRemoved.HasValue) {
            //                        partInstance.DateInstalled = null;
            //                    } 
            //                    partInstance.Quantity = 1;
            //                    partInstance.BubblerParameter.UpdateWeight(transaction.MeasuredWeight);
            //                    partInstance.LocationId =partInstance.Part.WarehouseId.Value;
            //                    break;
            //                }
            //                case InventoryAction.INCOMING: {
            //                    return new TransactionUndoOutput(null, false, "Error: Undo Incoming Transaction not Implemented Yet");
            //                }
            //                case InventoryAction.RETURNING: {
            //                    partInstance.BubblerParameter.UpdateWeight(transaction.ReferenceTransaction.Weight);
            //                    partInstance.Quantity = 0;
            //                    partInstance.DateRemoved = null;
            //                    partInstance.LocationId = transaction.ReferenceTransaction.LocationId;
            //                    break;
            //                }
            //                default: {
            //                    return new TransactionUndoOutput(null, false, "Error: Invalid InventoryAction, Contact Admin");
            //                }
            //            }
            //            //this._transactionRepository.DeleteAsync()
            //        } else {

            //        }
            //    } else {
            //        return new TransactionUndoOutput(null, false, "Error: Could no find Part Instance, Contact Admin");
            //    }
            //} else {
            //    return new TransactionUndoOutput(null, false, "Error:  Could not find transaction, Contact Admin");
            //}
            return new TransactionUndoOutput(null,false,"Error: Not Implemented Yet");
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
        }
    }
}
