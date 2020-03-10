using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.InstallSequence.Infrastructure {
    public static class Constants {

        //public static string InstallLocationDefault { get => @"C:\Program Files (x86)\Manufacturing Inventory"; }
        //public static string SourceDirectory { get => @"\\172.20.4.20\Manufacturing Install\Application"; }

        public static string InstallLocationDefault { get => @"C:\Program Files (x86)\Manufacturing Inventory"; }
        public static string SourceDirectory { get => @"\\172.20.4.20\Manufacturing Install\Application"; }
        public static string TempDirectory { get => @"C:\Temp"; }
        public static string InstallArchive { get => @"\\172.20.4.20\Manufacturing Install\First Time Installer\InventoryInstaller.zip"; }
    }
}
