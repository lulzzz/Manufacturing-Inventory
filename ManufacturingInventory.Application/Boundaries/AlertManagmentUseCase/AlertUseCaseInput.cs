using ManufacturingInventory.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.AlertManagmentUseCase {

    public class AlertUseCaseInput {

        public AlertUseCaseInput(int userId, AlertDto alertDto,AlertAction alertAction) {
            this.UserId = userId;
            this.AlertDto = alertDto;
            this.AlertAction = alertAction;
        }

        public int UserId { get; set; }
        public AlertDto AlertDto { get; set; }
        public AlertAction AlertAction { get; set; }

    }
}
