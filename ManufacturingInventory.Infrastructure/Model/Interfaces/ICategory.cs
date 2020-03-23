using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Infrastructure.Model.Interfaces {
    public interface ICategory {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        bool IsDefault { get; set; }
    }
}
