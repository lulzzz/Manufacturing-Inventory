using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Domain.DTOs {
    public class InstanceDto {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PartCategory { get; set; }
        public bool IsBubbler { get; set; }
        public double Quantity { get; set; }

        public InstanceDto() {
            this.Id = 0;
            this.Name = "N/A";
            this.PartCategory = "N/A";
            this.IsBubbler = false;
            this.Quantity = 0; 
        }

        public InstanceDto(PartInstance partInstance) {
            if (partInstance != null) {
                this.Id = partInstance.Id;
                this.Name = partInstance.Name;
                this.PartCategory = partInstance.Part.Name;
                this.IsBubbler = partInstance.IsBubbler;
                this.Quantity = (this.IsBubbler && partInstance.BubblerParameter != null) ? partInstance.BubblerParameter.Weight : partInstance.Quantity;
            }else {
                this.Id = -1;
                this.Name = "Deleted PartInstance";
                this.PartCategory = "PartInstance was deleted";
                this.IsBubbler = false;
                this.Quantity = -1;
            }

        }
    }
}
