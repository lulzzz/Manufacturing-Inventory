using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManufacturingInventory.Infrastructure.Model.Entities;

namespace ManufacturingInventory.Domain.DTOs {
    public class AlertDto {
        public int AlertId { get; set; }
        public string AlertIdentifier { get; set; }
        public AlertType AlertType { get; set; }
        public double Quantity { get; set; }
        public double MinQuantity { get; set; }
        public double SafeQuantity { get; set; }
        public bool IsEnabled { get; set; }
        public List<InstanceDto> PartInstances { get; set; }

        public AlertDto() {
            this.AlertIdentifier = "Not Set";
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
                    this.IsEnabled = false;
                    var individual = (IndividualAlert)alert;
                    var instance = new InstanceDto(individual.PartInstance);
                    this.AlertIdentifier = instance.Name;
                    this.Quantity = instance.Quantity;
                    this.MinQuantity = individual.PartInstance.MinQuantity;
                    this.SafeQuantity = individual.PartInstance.SafeQuantity;
                    this.PartInstances = new List<InstanceDto>();
                    this.PartInstances.Add(instance);
                    this.AlertType = alert.AlertType;
                    this.AlertId = individual.Id;
                    break;
                case AlertType.CombinedAlert:
                    this.IsEnabled = false;
                    this.AlertType = alert.AlertType;
                    var combinedAlert = (CombinedAlert)alert;
                    this.AlertIdentifier = combinedAlert.StockHolder.Name;
                    var stockType = combinedAlert.StockHolder;                    
                    this.AlertId = combinedAlert.Id;
                    this.Quantity = stockType.Quantity;
                    this.MinQuantity = stockType.MinQuantity;
                    this.SafeQuantity = stockType.SafeQuantity;
                    this.PartInstances = stockType.PartInstances.Select(partInstance => new InstanceDto(partInstance)).ToList();
                    break;
            }
        }

        public AlertDto(UserAlert userAlert) {
            this.IsEnabled = userAlert.IsEnabled;
            this.AlertType = userAlert.Alert.AlertType;
            this.AlertId = userAlert.AlertId;
            switch (userAlert.Alert.AlertType) {
                case AlertType.IndividualAlert:
                    var individual = (IndividualAlert)userAlert.Alert;
                    var instance = new InstanceDto(individual.PartInstance);
                    this.AlertIdentifier = instance.Name;
                    this.Quantity = instance.Quantity;
                    this.MinQuantity = individual.PartInstance.MinQuantity;
                    this.SafeQuantity = individual.PartInstance.SafeQuantity;
                    this.PartInstances = new List<InstanceDto>();
                    this.PartInstances.Add(instance);
                    break;
                case AlertType.CombinedAlert:
                    var combinedAlert = (CombinedAlert)userAlert.Alert;
                    this.AlertIdentifier = combinedAlert.StockHolder.Name;
                    var stockType = combinedAlert.StockHolder;
                    this.Quantity = stockType.Quantity;
                    this.MinQuantity = stockType.MinQuantity;
                    this.SafeQuantity = stockType.SafeQuantity;
                    this.PartInstances = stockType.PartInstances.Select(partInstance => new InstanceDto(partInstance)).ToList();
                    break;
            }
        }
    }
}
