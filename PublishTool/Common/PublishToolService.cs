using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace PublishTool.Common {
    
    public class UpdateLogEvent : PubSubEvent<string> { }

    public class PublishConstants {
        public static string SourceDirectory { get => @"D:\Software Development\Application Publish\Published"; }
        public static string PublishDirectroy { get=> @"\\172.20.4.20\Manufacturing Install\Application"; }
        public static string TargetProject { get => "\"D:/Software Development/Manufacturing Inventory/ManufacturingInventory/ManufacturingInventory.Main/ManufacturingInventory.UI.csproj\""; }
    }

    public interface IPublishToolService {
        string FinalPublishDirectory { get; set; }
        string PublishOutput { get; set; }
        string SourceDirectroy { get; set; }
        string TargetProject { get; set; }
        Task Publish();
        void SetPaths(string source, string target, string output);
    }

    public class PublishToolService : IPublishToolService {
        IEventAggregator _eventAggregator;
        public string FinalPublishDirectory { get; set; }
        public string SourceDirectroy { get; set; }
        public string TargetProject { get; set; }
        public string PublishOutput { get; set; }
        private bool _error = false;

        public PublishToolService(IEventAggregator eventAggregator) {
            this._eventAggregator = eventAggregator;
            this.PublishOutput = PublishConstants.PublishDirectroy;
            this.TargetProject = PublishConstants.TargetProject;
            this.SourceDirectroy = PublishConstants.SourceDirectory;
            this.ParseFileName();
        }

        public void SetPaths(string source, string target, string output) {
            this.SourceDirectroy = source;
            this.TargetProject = target;
            this.PublishOutput = output;
        }

        public async Task Publish() {
            await Task.Run(() => {
                Process process = new Process();
                ProcessStartInfo psi = new ProcessStartInfo {
                    FileName = "dotnet.exe",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Minimized,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
                psi.Arguments = "publish \"D:/Software Development/Manufacturing Inventory/ManufacturingInventory/ManufacturingInventory.Main/ManufacturingInventory.UI.csproj\" " +
                    "-c Release --output \"D:/Software Development/Application Publish/Published\" --self-contained false -r win7-x64";
                process.StartInfo = psi;
                process.OutputDataReceived += (sender, args) => this._eventAggregator.GetEvent<UpdateLogEvent>().Publish("Output: " + args.Data);
                process.ErrorDataReceived += (sender, args) => this._eventAggregator.GetEvent<UpdateLogEvent>().Publish("Error: " + args.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            });
            this.ParseFileName();
            if (!this._error) {
                var response = await ZipHelper.Execute(this.SourceDirectroy, this.FinalPublishDirectory, ZipAction.Zip, (message) => this._eventAggregator.GetEvent<UpdateLogEvent>().Publish(message));
            } else {
                this._eventAggregator.GetEvent<UpdateLogEvent>().Publish("File IO Error!!");
            }
        }

        private void ParseFileName() {
            var newRelease = Path.Combine(this.SourceDirectroy, "ManufacturingApplication.exe");
            if (File.Exists(newRelease)) {
                this._error = false;
                var versionToPublish = FileVersionInfo.GetVersionInfo(newRelease).FileVersion;
                this.FinalPublishDirectory = Path.Combine(this.PublishOutput, "Manufacturing_version-" + versionToPublish.Replace(".", "_") + ".zip");
            } else {
                this._error = true;
            }
        }
    }

    public enum ZipAction {
        Zip,
        UnZip
    }

    public class ZipHelperResponse {

        public ZipHelperResponse(string message, bool success) {
            this.Message = message;
            this.Success = success;
        }

        public string Message { get; set; }
        public bool Success { get; set; }
    }

    public static class ZipHelper {
        public delegate void ProgressDelegate(string message);

        public static async Task<ZipHelperResponse> Execute(string dir, string compressedFile, ZipAction action, ProgressDelegate outFunc = null) {
            DirectoryInfo directory;
            switch (action) {
                case ZipAction.Zip:
                    if (Directory.Exists(dir)) {
                        directory = new DirectoryInfo(dir);
                        return await CompressDirectory(directory, compressedFile, outFunc);
                    } else {
                        return new ZipHelperResponse("Directory Not Found", false);
                    }
                case ZipAction.UnZip:
                    if (!Directory.Exists(dir)) {
                        try {
                            directory = Directory.CreateDirectory(dir);
                        } catch {
                            return new ZipHelperResponse("Failed: Could Not Create Ouput Directory", false);
                        }
                    } else {
                        directory = new DirectoryInfo(dir);
                    }
                    return await DecompressDirectory(directory, compressedFile, outFunc);

                default:
                    return new ZipHelperResponse("Invalid Action", false);
            }
        }

        public static async Task<ZipHelperResponse> CompressDirectory(DirectoryInfo directory, string compressedFile, ProgressDelegate progressDelegate) {
            var fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            var bufferSize = 4096;
            using var zipStream = File.Create(compressedFile, bufferSize, fileOptions);
            ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Create);
            foreach (var file in directory.GetFiles()) {
                using var fileStream = file.OpenRead();
                progressDelegate?.Invoke("Copying {0}...." + file.Name);
                var entry = archive.CreateEntry(file.Name, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await fileStream.CopyToAsync(entryStream);
            }
            return new ZipHelperResponse("Done", true);
        }

        public static async Task<ZipHelperResponse> DecompressDirectory(DirectoryInfo directory, string compressedFile, ProgressDelegate progressDelegate) {
            var fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            var bufferSize = 4096;
            using var archive = ZipFile.OpenRead(compressedFile);

            foreach (var entry in archive.Entries) {
                using var zipStream = entry.Open();
                var destinationFile = Path.Combine(directory.FullName, entry.Name);
                using var fileStream = new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize, fileOptions);
                progressDelegate?.Invoke("Copying {0}...." + entry.Name);
                await zipStream.CopyToAsync(fileStream);
            }
            return new ZipHelperResponse("Unzip Succesful", true);
        }
    }


}
