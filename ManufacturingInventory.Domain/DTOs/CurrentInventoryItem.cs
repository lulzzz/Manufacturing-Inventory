﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Domain.DTOs {
    public class CurrentInventoryItem {
        public int Id { get; set; }
        public DateTime Today { get; set; }
        public string PartCategory { get; set; }
        public string Part { get; set; }
        public DateTime DateIn { get; set; }
        public int Age { get; set; }
        public double Quantity { get; set; }
        public double Cost { get; set; }
    }
}
