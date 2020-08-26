using ManufacturingInventory.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.AlertManagmentUseCase {

    public class AlertUseCaseInput {

        public AlertUseCaseInput(int userId, AlertDto alertDto) {
            this.UserId = userId;
            this.AlertDto = alertDto;
        }

        public int UserId { get; set; }
        public AlertDto AlertDto { get; set; }

    }
}
