using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prism.Events;
using File = System.IO.File;

namespace ManufacturingInventory.InstallSequence.Infrastructure {
    public interface IInstaller {
        bool Install(string location = null);
        Task<bool> InstallAsync(CancellationToken cancellationToken,string location = null);
        int CalculateWork();
        Task UnInstall();
        int CalculateWorkUninstall();
        string InstallLocation { get; set; }
    }

    public class Installer:IInstaller {
        private string sourceDirectory = Constants.SourceDirectory;
        private string targetDirectory = Constants.InstallLocationDefault;
        private IEventAggregator _eventAggregator;
        

        public string InstallLocation {
            get=>this.targetDirectory;
            set => this.targetDirectory = value;
        }

        public Installer(IEventAggregator eventAggregator) {
            this._eventAggregator = eventAggregator;
        }

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

        public async Task<bool> InstallAsync(CancellationToken cancellationToken,string location = null) {
            bool success = true;

            if (!string.IsNullOrEmpty(location)) {
                this.sourceDirectory = location;
            }

            DirectoryInfo diSource = new DirectoryInfo(this.sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(this.targetDirectory);
            try {
                await CopyAllAsync(diSource, diTarget,cancellationToken);
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

        public int CalculateWork() {
            DirectoryInfo diSource = new DirectoryInfo(this.sourceDirectory);
            var fileCount = diSource.GetFiles().Length;
            foreach (DirectoryInfo diSourceSubDir in diSource.GetDirectories()) {
                fileCount += diSourceSubDir.GetFiles().Length;
            }
            return fileCount;
        }

        public int CalculateWorkUninstall() {
            DirectoryInfo diSource = new DirectoryInfo(this.targetDirectory);
            var fileCount = diSource.GetFiles().Length;
            foreach (DirectoryInfo diSourceSubDir in diSource.GetDirectories()) {
                fileCount += diSourceSubDir.GetFiles().Length;
            }
            return fileCount;
        }

        private void CopyAll(DirectoryInfo source, DirectoryInfo target) {
            //if (source.FullName.ToLower() == target.FullName.ToLower()) {
            //    return;
            //}

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

        public async Task CopyAllAsync(DirectoryInfo source, DirectoryInfo target, CancellationToken cancellationToken) {
            //if (source.FullName.ToLower() == target.FullName.ToLower()) {
            //    return;
            //}
            bool canceled = false;
            if (Directory.Exists(target.FullName) == false) {
                Directory.CreateDirectory(target.FullName);
            }
            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles()) {
                if (!cancellationToken.IsCancellationRequested) {
                    var dest = Path.Combine(target.ToString(), fi.Name);
                    if (File.Exists(dest)) {
                        FileInfo info = new FileInfo(dest);
                        if (fi.LastWriteTime != info.LastWriteTime) {
                            File.Delete(dest);
                            await this.CopyFileAsync(fi.FullName, dest, cancellationToken);
                            File.SetLastWriteTime(dest,fi.LastWriteTime);
                            this._eventAggregator.GetEvent<IncrementProgress>().Publish("Updating "+fi.Name);
                        } else {
                            this._eventAggregator.GetEvent<IncrementProgress>().Publish("Skipping " + fi.Name);
                        }
                    } else {
                        await this.CopyFileAsync(fi.FullName, dest, cancellationToken);
                        this._eventAggregator.GetEvent<IncrementProgress>().Publish("Copied " + fi.Name);
                    }
                } else {
                    canceled = true;
                    break;
                }
            }

            if (!canceled) {
                foreach (DirectoryInfo diSourceSubDir in source.GetDirectories()) {
                    DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                    CopyAll(diSourceSubDir, nextTargetSubDir);
                }
            }
        }

        public async Task UnInstall() {
            await this.DeleteInnerAsync(this.targetDirectory);
        }

        public async Task DeleteAsync(string path) {
            await DeleteInnerAsync(path);
        }

        private async Task DeleteInnerAsync(string path) {
            foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly)) {
                var f = new FileInfo(file);
                this._eventAggregator.GetEvent<IncrementProgress>().Publish("Deleteing " + f.Name);
                await DeleteFileAsync(file);
                await Task.Delay(100);
            }

            foreach (string directory in Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly)) {
                DirectoryInfo d = new DirectoryInfo(directory);
                this._eventAggregator.GetEvent<IncrementProgress>().Publish("Deleteing " + d.Name);
                await DeleteInnerAsync(directory);
                await Task.Delay(100);
            }
            Directory.Delete(path);
        }

        private static async Task DeleteFileAsync(string file) {
            using (FileStream stream = new FileStream(file, FileMode.Truncate, FileAccess.Write, FileShare.Delete, 4096, true)) {
                await stream.FlushAsync();
                File.Delete(file);
            }
        }

        public async Task CopyFileAsync(string sourceFile, string destinationFile, CancellationToken cancellationToken) {
            var fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            var bufferSize = 4096;

            using var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions);
            using var destinationStream = new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize, fileOptions);
            await sourceStream.CopyToAsync(destinationStream, bufferSize, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        public void AddStartMenuShortCut() {
            string pathToExe = Path.Combine(this.targetDirectory, "ManufacturingInventory.ManufacturingApplication.exe");
            string commonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            string appStartMenuPath = Path.Combine(commonStartMenuPath, "Programs", "Manufacturing Inventory");

            if (!Directory.Exists(appStartMenuPath))
                Directory.CreateDirectory(appStartMenuPath);

            string shortcutLocation = Path.Combine(appStartMenuPath, "Manufacturing Inventory" + ".lnk");
            if (System.IO.File.Exists(shortcutLocation)) {
                try {
                    System.IO.File.Delete(shortcutLocation);
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                    shortcut.Description = "Shortcut to Manufacturing Inventory Software";
                    shortcut.TargetPath = pathToExe;
                    shortcut.Save();
                } catch {

                }
            } else {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                shortcut.Description = "Shortcut to Manufacturing Inventory Software";
                shortcut.TargetPath = pathToExe;
                shortcut.Save();
            }
        }
    }
}
