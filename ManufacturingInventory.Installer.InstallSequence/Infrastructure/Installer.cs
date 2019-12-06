using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ManufacturingInventory.InstallSequence.Infrastructure {
    public interface IInstaller {
        bool Install(string location = null);
    }

    public class Installer:IInstaller {
        private string sourceDirectory = Constants.SourceDirectory;
        private string targetDirectory = Constants.InstallLocationDefault;

        public EventHandler ProgressIncrease;

        public bool Install(string location=null) {
            bool success = true;

            if (!string.IsNullOrEmpty(location)) {
                this.sourceDirectory = location;
            }

            DirectoryInfo diSource = new DirectoryInfo(this.sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(this.targetDirectory);
            try {
                CopyAll(diSource, diTarget);
            } catch {
                success = false;
            }

            if (success) {
                try {
                    AddStartMenuShortCut();
                } catch {
                    success = false;
                }
            }
            return success;
        }
        
        private int CalculateWork(DirectoryInfo source, DirectoryInfo target) {
            var fileCount = source.GetFiles().Length;
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories()) {
                fileCount += diSourceSubDir.GetFiles().Length;
                //DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            }
            return fileCount;
        }

        private void CopyAll(DirectoryInfo source, DirectoryInfo target) {
            if (source.FullName.ToLower() == target.FullName.ToLower()) {
                return;
            }

            if (Directory.Exists(target.FullName) == false) {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles()) {
                var dest = Path.Combine(target.ToString(), fi.Name);
                if (System.IO.File.Exists(dest)) {
                    FileInfo info = new FileInfo(dest);
                    if (fi.LastWriteTime != info.LastWriteTime) {
                        fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
                    } else {
                    }
                } else {
                    fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
                }
            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories()) {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public void AddStartMenuShortCut() {
            string pathToExe = Path.Combine(this.targetDirectory, "ManufacturingInventory.Main.exe");
            string commonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            string appStartMenuPath = Path.Combine(commonStartMenuPath, "Programs", "Manufacturing Inventory");

            if (!Directory.Exists(appStartMenuPath))
                Directory.CreateDirectory(appStartMenuPath);

            string shortcutLocation = Path.Combine(appStartMenuPath, "Shortcut to Manufacturing Inventory" + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = "Shortcut to Manufacturing Inventory Software";
            shortcut.TargetPath = pathToExe;
            shortcut.Save();
        }

        public bool CheckFileVersion() {
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
            if (System.IO.File.Exists(@"\\172.20.4.20\Testing Installs\ManufacturingInventory.ConsoleTesting.exe")) {
                var serverVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(@"\\172.20.4.20\Testing Installs\ManufacturingInventory.ConsoleTesting.exe").FileVersion;
                if (fileVersion != serverVersion) {
                    return false;
                } else {
                    return true;
                }
            } else {
                return false;
            }
        }
    }
}
