using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Domain.Security {
    public class EncryptedResult {
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
        public byte[] Encrypted { get; set; }
    }
}
