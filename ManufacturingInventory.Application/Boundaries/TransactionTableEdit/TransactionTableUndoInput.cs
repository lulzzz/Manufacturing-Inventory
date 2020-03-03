using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.TransactionTableEdit {

    public class TransactionTableUndoInput {

        public TransactionTableUndoInput(int transactionId) {
            this.TransactionId = transactionId;
        }
        public int TransactionId { get; set; }
    }
}
