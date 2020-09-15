using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManufacturingInventory.Domain.DTOs {
    public class PartDto {
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<InstanceDto> PartInstances { get; set; }

        public PartDto(Part part) {
            this.Id = part.Id;
            this.Name = part.Name;
            this.PartInstances = part.PartInstances.Select(instance => new InstanceDto(instance));
        }

        public PartDto() {
            this.Id = -1;
            this.Name = "N/A";
        }
    }
}
