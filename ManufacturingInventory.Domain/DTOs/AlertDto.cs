using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManufacturingInventory.Infrastructure.Model.Entities;

namespace ManufacturingInventory.Domain.DTOs {
    public class AlertDto {
        public int AlertId { get; set; }
        
        public AlertType AlertType { get; set; }
        public double Quantity { get; set; }
        public double MinQuantity { get; set; }
        public double SafeQuantity { get; set; }
        public List<InstanceDto> PartInstances { get; set; }

        public AlertDto() {
            this.PartInstances = new List<InstanceDto>();
            this.Quantity = 0;
            this.MinQuantity = 0;
            this.SafeQuantity = 0;
            this.AlertType = AlertType.IndividualAlert;
            this.AlertId = 0;
        }

        public AlertDto(Alert alert) {
            switch (alert.AlertType) {
                case AlertType.IndividualAlert:
                    var individual = (IndividualAlert)alert;
                    var instance = new InstanceDto(individual.PartInstance);
                    this.Quantity = instance.Quantity;
                    this.MinQuantity = individual.PartInstance.MinQuantity;
                    this.SafeQuantity = individual.PartInstance.SafeQuantity;
                    this.PartInstances = new List<InstanceDto>();
                    this.PartInstances.Add(instance);
                    this.AlertType = alert.AlertType;
                    break;
                case AlertType.CombinedAlert:
                    this.AlertType = alert.AlertType;
                    var combinedAlert = (CombinedAlert)alert;
                    var stockType = combinedAlert.StockHolder;                    
                    this.AlertId = combinedAlert.Id;
                    this.Quantity = stockType.Quantity;
                    this.MinQuantity = stockType.MinQuantity;
                    this.SafeQuantity = stockType.SafeQuantity;
                    this.PartInstances = stockType.PartInstances.Select(partInstance => new InstanceDto(partInstance)).ToList();
                    break;
            }
        }
    }
}
