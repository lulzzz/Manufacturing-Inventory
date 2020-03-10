using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using ZipHelperLibrary;

namespace PublishTool.Common {
    
    public class UpdateLogEvent : PubSubEvent<string> { }
    public class PublishDoneEvent : PubSubEvent { }

    public class PublishConstants {
        public static string SourceDirectory { get => @"D:\Software Development\Application Publish\Published"; }
        public static string PublishDirectroy { get=> @"\\172.20.4.20\Manufacturing Install\Application"; }
        public static string TargetProject { get => "\"D:/Software Development/Manufacturing Inventory/ManufacturingInventory/ManufacturingInventory.Main/ManufacturingInventory.UI.csproj\""; }
        public static string ManufacturingPublish { get=> "publish \"D:/Software Development/Manufacturing Inventory/ManufacturingInventory/ManufacturingInventory.Main/ManufacturingInventory.UI.csproj\" " +
                    "-c Release --output \"D:/Software Development/Application Publish/Published\" --self-contained false -r win7-x64";}
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
                psi.Arguments = PublishConstants.ManufacturingPublish;
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
                this._eventAggregator.GetEvent<UpdateLogEvent>().Publish("Publish Done!");
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
}
