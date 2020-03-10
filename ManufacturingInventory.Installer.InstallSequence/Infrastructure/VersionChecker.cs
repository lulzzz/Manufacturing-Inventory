using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public string VersionPath { get; set; }
        public bool FirstTime { get; set; }

        public VersionCheckerResponce(InstallStatus status,string version,string message,string versionPath) {
            this.InstallStatus = status;
            this.Message = message;
            this.Version = version;
            this.VersionPath = versionPath;
        }
    }

    public static class VersionChecker {
        public static VersionCheckerResponce CheckInstalledVersion() {
            if (Directory.Exists(Constants.SourceDirectory)) {
                var versionLookup = GetServerVersions();
                if (versionLookup!=null) {
                    var newVersionKey = versionLookup.Max(e => e.Key);
                    var newFile = versionLookup[newVersionKey];
                    var newVersionStr = GetVersionString(newFile.Name);
                    var dest = Path.Combine(Constants.InstallLocationDefault, "ManufacturingApplication.exe");
                    if (File.Exists(dest)) {
                        var installedVersionStr = FileVersionInfo.GetVersionInfo(dest).FileVersion;
                        var installedVersionStrArray = installedVersionStr.Split(".");
                        var installedVersionValue = CalculateVersion(installedVersionStrArray);
                        if (installedVersionValue < newVersionKey) {
                            return new VersionCheckerResponce(InstallStatus.InstalledNewVersion, newVersionStr, "New Version Available", newFile.FullName);
                        } else {
                            return new VersionCheckerResponce(InstallStatus.InstalledUpToDate, installedVersionStr, "Application Is Up To Date", newFile.FullName);
                        }
                    } else {
                        return new VersionCheckerResponce(InstallStatus.NotInstalled,newVersionStr,"Not Installed", newFile.FullName);
                    }
                } else {
                    return new VersionCheckerResponce(InstallStatus.ServerFilesMissing,"", "No Files Found In Directory","");
                }
            } else {
                return new VersionCheckerResponce(InstallStatus.ServerFilesMissing, "", "Directory Not Found","");
            }
        }

        public static string GetVersionString(string fileNameWithVersion) {
            return fileNameWithVersion.Substring(fileNameWithVersion.IndexOf("-") + 1, (fileNameWithVersion.Length - 4) - (fileNameWithVersion.IndexOf("-") + 1)).Replace("_", ".");
        }

        public static int CalculateVersion(string[] versionStringArray) {
            var versionArrray = Array.ConvertAll(versionStringArray, e => Convert.ToInt32(e));
            int dateTimeValue = 1;
            int majorMinorValue = 0;
            for (int i = 0; i < versionArrray.Length; i++) {
                if (i >= 2) {
                    dateTimeValue *= versionArrray[i];
                } else {
                    majorMinorValue += versionArrray[i];
                }
            }
            return dateTimeValue + majorMinorValue;
        }

        public static Dictionary<int, FileInfo> GetServerVersions() {
            DirectoryInfo directoryInfo = new DirectoryInfo(Constants.SourceDirectory);
            var files = directoryInfo.GetFiles();
            if (files.Count() > 0) {
                Dictionary<int, FileInfo> versionLookup = new Dictionary<int, FileInfo>();
                foreach (var file in files) {
                    string name = file.Name;
                    string versionString = GetVersionString(file.Name);
                    var values = versionString.Split(".");
                    var version = CalculateVersion(values);
                    versionLookup.Add(version, file);
                }
                return versionLookup;
            } else {
                return null;
            }

        }
    }
}
