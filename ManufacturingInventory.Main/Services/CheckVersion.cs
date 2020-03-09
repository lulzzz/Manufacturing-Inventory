using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ManufacturingInventory.ManufacturingApplication.Services {

    public static class Constants {
        public static string InstallLocationDefault { get => @"C:\Program Files (x86)\Manufacturing Inventory"; }
        public static string SourceDirectory { get => @"\\172.20.4.20\Manufacturing Install\Application"; }
        public static string TempDirectory { get => @"C:\Temp"; }
    }

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
            var fileVersion = FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
            var temp = GetVersionString(fileVersion).Split(".");          
            var fileVersionValue= Array.ConvertAll(temp, e => Convert.ToInt32(e)).Sum();
            if (Directory.Exists(Constants.SourceDirectory)) {
                DirectoryInfo directoryInfo = new DirectoryInfo(Constants.SourceDirectory);
                var files = directoryInfo.GetFiles();
                if (files.Count() > 0) {
                    Dictionary<int, FileInfo> versionLookup = new Dictionary<int, FileInfo>();
                    foreach (var file in files) {
                        string name = file.Name;
                        string versionString = GetVersionString(file.Name);
                        var values = versionString.Split(".");
                        var version = Array.ConvertAll(values, e => Convert.ToInt32(e)).Sum();
                        versionLookup.Add(version, file);
                    }
                    var newVersionKey = versionLookup.Max(e => e.Key);
                    var newFile = versionLookup[newVersionKey];
                    var serverVersion = FileVersionInfo.GetVersionInfo(newFile.FullName).FileVersion;
                    if (fileVersionValue < newVersionKey) {
                        return new CheckVersionResponse(true, fileVersion,serverVersion);
                    } else {
                        return new CheckVersionResponse(false, fileVersion, serverVersion);
                    }

                } else {
                    return new CheckVersionResponse(false, "", "");
                }
            } else {
                return new CheckVersionResponse(false, "", "");
            }
        }

        public static string GetVersionString(string fileNameWithVersion) {
            return fileNameWithVersion.Substring(fileNameWithVersion.IndexOf("-") + 1, (fileNameWithVersion.Length - 4) - (fileNameWithVersion.IndexOf("-") + 1)).Replace("_", ".");
        }
    }

    public static class VersionChecker {
        public static CheckVersion CheckInstalledVersion() {
            if (Directory.Exists(Constants.SourceDirectory)) {
                DirectoryInfo directoryInfo = new DirectoryInfo(Constants.SourceDirectory);
                var files = directoryInfo.GetFiles();
                if (files.Count() > 0) {
                    Dictionary<int, FileInfo> versionLookup = new Dictionary<int, FileInfo>();
                    foreach (var file in files) {
                        string name = file.Name;
                        string versionString = GetVersionString(file.Name);
                        var values = versionString.Split(".");
                        var version = Array.ConvertAll(values, e => Convert.ToInt32(e)).Sum();
                        versionLookup.Add(version, file);
                    }
                    var newVersionKey = versionLookup.Max(e => e.Key);
                    var newFile = versionLookup[newVersionKey];
                    var newVersionStr = GetVersionString(newFile.Name);
                    var dest = Path.Combine(Constants.InstallLocationDefault, "ManufacturingApplication.exe");
                    if (File.Exists(dest)) {
                        var installedVersionStr = FileVersionInfo.GetVersionInfo(dest).FileVersion;
                        if (installedVersionStr == newVersionStr) {
                            return 
                        } else {
                            return new VersionCheckerResponce(, newVersionStr, "New Version Available", newFile.FullName);
                        }
                    } else {
                        return new VersionCheckerResponce(InstallStatus.NotInstalled, newVersionStr, "Not Installed", newFile.FullName);
                    }
                } else {
                    return new VersionCheckerResponce(InstallStatus.ServerFilesMissing, "", "No Files Found In Directory", "");
                }
            } else {
                return new VersionCheckerResponce(InstallStatus.ServerFilesMissing, "", "Directory Not Found", "");
            }
        }

        public static string GetVersionString(string fileNameWithVersion) {
            return fileNameWithVersion.Substring(fileNameWithVersion.IndexOf("-") + 1, (fileNameWithVersion.Length - 4) - (fileNameWithVersion.IndexOf("-") + 1)).Replace("_", ".");
        }
    }
}
