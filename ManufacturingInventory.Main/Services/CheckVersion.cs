using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ManufacturingInventory.ManufacturingApplication.Services {

    public static class Constants {
        public static string SourceDirectory { get => @"\\172.20.4.20\ManufacturingInstall\Application"; }
        public static string TempDirectory { get => @"C:\Temp"; }
    }

    public class CheckVersionResponse {
        public bool NewVersionAvailable { get; set; }
        public string CurrentVersion { get; set; }
        public string NewVersion { get; set; }
        public string InstallerLocation { get; set; }

        public CheckVersionResponse(bool available,string current,string newVersion,string installerLocation) {
            this.NewVersionAvailable = available;
            this.CurrentVersion = current;
            this.NewVersion = newVersion;
            this.InstallerLocation = installerLocation;
        }
    }

    public class CheckVersion {
        public static string sourceDirectory = @"\\172.20.4.20\ManufacturingInstall\Application";
        public CheckVersionResponse Check() {
            //var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var assemblyName = AssemblyName.GetAssemblyName(@"C:\Program Files\ManufacturingInventory\ManufacturingApplication.dll");
            //var fileVersion= AssemblyName.GetAssemblyName(assemblyLocation).Version.ToString();
            var fileVersion = assemblyName.Version.ToString();
            var temp = Array.ConvertAll(fileVersion.Split("."),e=>Convert.ToInt32(e));
            var fileVersionValue = CalculateVersion(temp);
            if (Directory.Exists(Constants.SourceDirectory)) {
                DirectoryInfo directoryInfo = new DirectoryInfo(Constants.SourceDirectory);
                var files = directoryInfo.GetFiles();
                if (files.Count() > 0) {
                    Dictionary<int, FileInfo> versionLookup = new Dictionary<int, FileInfo>();
                    foreach (var file in files) {
                        string name = file.Name;
                        string versionString = GetVersionString(file.Name);
                        var values = Array.ConvertAll(versionString.Split("."), e => Convert.ToInt32(e));
                        var version = CalculateVersion(values);
                        versionLookup.Add(version, file);
                    }
                    var newVersionKey = versionLookup.Max(e => e.Key);
                    var newFile = versionLookup[newVersionKey];
                    var serverVersion = GetVersionString(newFile.FullName);
                    if (fileVersionValue < newVersionKey) {
                        return new CheckVersionResponse(true, fileVersion,serverVersion,newFile.FullName);
                    } else {
                        return new CheckVersionResponse(false, fileVersion, serverVersion, newFile.FullName);
                    }
                } else {
                    return new CheckVersionResponse(false, "", "","");
                }
            } else {
                return new CheckVersionResponse(false, "", "","");
            }
        }

        public static string GetVersionString(string fileNameWithVersion) {
            //return fileNameWithVersion.Substring(fileNameWithVersion.IndexOf("-") + 1, (fileNameWithVersion.Length - 4) - (fileNameWithVersion.IndexOf("-") + 1)).Replace("_", ".");
            return fileNameWithVersion.Split('-')[1];
        }

        public static int CalculateVersion(int[] versionArray) {
            int dateTimeValue = 1;
            int majorMinorValue = 0;
            for (int i = 0; i < versionArray.Length; i++) {
                if (i >= 2) {
                    dateTimeValue *= versionArray[i];
                } else {
                    majorMinorValue += versionArray[i];
                }
            }
            return dateTimeValue + majorMinorValue;
        }
    }
}
