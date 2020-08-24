using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.AlertManagmentUseCase {
    public enum AlertAction {
        Subscribe,
        UnSubscribe,
        ToggleEnable
    }

    public class AlertsExisitingInput {
        public int UserId { get; set; }
        public int AlertId { get; set; }

    }
}
