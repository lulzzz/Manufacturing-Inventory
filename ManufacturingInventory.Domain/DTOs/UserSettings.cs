using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Domain.DTOs {
    public class UserSettings {
        public string UserName { get; set; }
        public string Key { get; set; }
        public string IV { get; set; }
        public string Password { get; set; }
    }
}
