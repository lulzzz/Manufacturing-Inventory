using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.InstallSequence.Infrastructure {
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
