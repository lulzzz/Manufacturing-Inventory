using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ManufacturingInventory.Main.Services {
    public class CheckVersionResponse {
        public bool NewVersionAvailable { get; set; }
        public string CurrentVersion { get; set; }
        public string NewVersion { get; set; }

        public CheckVersionResponse(bool available,string current,string newVersion) {
            this.NewVersionAvailable = available;
            this.CurrentVersion = current;
            this.NewVersion = newVersion;
        }
    }

    public class CheckVersion {
        public static string sourceDirectory = @"\\172.20.4.20\Manufacturing Install";
        public static string targetDirectory = @"C:\Program Files (x86)\InventoryTestInstall";

        public CheckVersionResponse Check() {
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
            if (File.Exists(@"\\172.20.4.20\Manufacturing Install\ManufacturingInventory.Main.exe")) {
                var serverVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(@"\\172.20.4.20\Manufacturing Install\ManufacturingInventory.Main.exe").FileVersion;
                if (fileVersion != serverVersion) {
                    return new CheckVersionResponse(true,fileVersion,serverVersion);
                } else {
                    return new CheckVersionResponse(false, fileVersion, serverVersion);
                }
            } else {
                return new CheckVersionResponse(false, fileVersion,"Not Found"); ;
            }
        }

    }
}
