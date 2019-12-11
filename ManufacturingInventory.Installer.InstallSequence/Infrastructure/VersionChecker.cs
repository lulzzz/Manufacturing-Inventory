using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ManufacturingInventory.InstallSequence.Infrastructure;

namespace ManufacturingInventory.InstallSequence.Infrastructure {
    public enum InstallStatus {
        InstalledNewVersion,
        InstalledUpToDate,
        NotInstalled,
        ServerFilesMissing
    }

    public class VersionCheckerResponce {
        public InstallStatus InstallStatus { get; set; }
        public string Message { get; set; }
        public string Version { get; set; }

        public VersionCheckerResponce(InstallStatus status,string version,string message) {
            this.InstallStatus = status;
            this.Message = message;
            this.Version = version;
        }
    }

    public static class VersionChecker {
        public static VersionCheckerResponce CheckInstalledVersion() {
            var source = Path.Combine(Constants.SourceDirectory, "ManufacturingInventory.ManufacturingApplication.exe");
            var dest= Path.Combine(Constants.InstallLocationDefault, "ManufacturingInventory.ManufacturingApplication.exe");
            if (File.Exists(source)) {
                var serverVersion = FileVersionInfo.GetVersionInfo(source).FileVersion;
                if (File.Exists(dest)) {
                    var installedVersion = FileVersionInfo.GetVersionInfo(dest).FileVersion;
                    if (installedVersion != serverVersion) {
                        return new VersionCheckerResponce(InstallStatus.InstalledNewVersion,serverVersion, "New Version Available");
                    } else {
                        return new VersionCheckerResponce(InstallStatus.InstalledUpToDate,installedVersion, "Software Is Up To Date");
                    }
                } else {
                    return new VersionCheckerResponce(InstallStatus.NotInstalled, string.Empty, "Not Installed");
                }
            } else {
                return new VersionCheckerResponce(InstallStatus.ServerFilesMissing,string.Empty,"Server files missing");
            }
        }
    }
}
