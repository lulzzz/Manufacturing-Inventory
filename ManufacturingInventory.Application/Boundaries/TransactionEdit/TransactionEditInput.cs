using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.TransactionEdit {
    public enum TransactionEditAction {
        RETURN,
        UNDO
    }

    public class TransactionEditInput {

        public TransactionEditInput(TransactionEditAction editAction, int transactionId) {
            this.EditAction = editAction;
            this.TransactionId = transactionId;
        }

        public TransactionEditAction EditAction { get; set; }
        public int TransactionId { get; set; }
    }
}
