using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManufacturingInventory.Domain.IO {

    public static class FileService {

        public static async Task<string> UploadFileAsync(string sourceFile,string newName) {

            if (File.Exists(sourceFile)) {
                var dest = Path.Combine(FileConstants.DestinationDirectory, newName + Path.GetExtension(sourceFile));
                if (!File.Exists(dest)) {
                    if(await CopyFileAsync(sourceFile, dest)) {
                        return dest;
                    } else {
                        return string.Empty;
                    }
                } else {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        public static async Task<bool> CopyFileAsync(string sourceFile, string destinationFile) {
            
            var fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            var bufferSize = 4096;
            try {
                using var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions);
                using var destinationStream = new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize, fileOptions);
                await sourceStream.CopyToAsync(destinationStream, bufferSize).ConfigureAwait(continueOnCapturedContext: false);
                return true;
            } catch {
                return false;
            }

        }

        public static async Task<bool> RenameFileAsync(string sourceFile, string newName) {
            if (File.Exists(sourceFile)) {
                var dest = Path.Combine(FileConstants.DestinationDirectory, newName + Path.GetExtension(sourceFile));
                if (!File.Exists(dest)) {
                    if (await CopyFileAsync(sourceFile, dest)) {
                        try {
                            File.Delete(sourceFile);
                            return true;
                        } catch {
                            return false;
                        }
                    } else {
                        return false;
                    }
                } else {
                    return false;
                }
            }
            return false;
        }

        public static async Task<bool> DeleteFileAsync(string sourceFile) {
            try {
                using (FileStream stream = new FileStream(sourceFile, FileMode.Truncate, FileAccess.Write, FileShare.Delete, 4096, true)) {
                    await stream.FlushAsync();
                    File.Delete(sourceFile);
                }
                return true;
            } catch {
                return false;
            }

        }
    }
}
