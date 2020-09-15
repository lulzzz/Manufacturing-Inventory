using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.LocationManage {
    public class LocationManagmentOutput : IOutput {
        public bool Success { get; set; }
        public string Message { get; set; }
        public LocationDto Location { get; set; }

        public LocationManagmentOutput(LocationDto location,bool success, string message) {
            this.Success = success;
            this.Message = message;
            this.Location = location;
        }

    }
}
