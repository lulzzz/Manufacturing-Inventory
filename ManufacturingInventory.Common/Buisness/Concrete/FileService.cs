using System;
using System.IO;
using ManufacturingInventory.Common.Buisness.Interfaces;
using ManufacturingInventory.Common.Application;

namespace ManufacturingInventory.Common.Buisness.Concrete {
    public class FileService : IFileService {

        public void DeleteFile(string name, string fileReference)
        {
            if (File.Exists(fileReference)) {
                try {
                    File.Delete(fileReference);
                } catch {
                    throw new Exception("Error deleting file");
                }
            } else {
                throw new FileNotFoundException("File: "+fileReference);
            }
        }

        public void RenameFile(string name, string newName, string fileReference)
        {
            if (File.Exists(fileReference)) {
                string newPath = Path.GetDirectoryName(fileReference);
                newPath += Path.Combine(newName + Path.GetExtension(fileReference));
                if (!File.Exists(newPath)) {
                    File.Move(fileReference, newPath);
                } else {
                    throw new Exception("Dublicate Filename,File: "+newPath);
                }
            } else {
                throw new FileNotFoundException("File: " + fileReference);
            }
        }

        //public async void SaveFile(string name, string source)
        //{
        //    string destinationFile;
        //    if(Directory.Exists(Constants.DestinationDirectory))
        //    {
        //        if (File.Exists(source))
        //        {
        //            destinationFile = Path.Combine(Constants.DestinationDirectory, name + Path.GetExtension(source));
        //            if (!File.Exists(destinationFile))
        //            {
        //                using (FileStream sourceStream = File.Open(source, FileMode.Open)) {
        //                    using(FileStream destStream = File.Create(destinationFile)) {
        //                        await sourceStream.CopyToAsync(destStream);
        //                    }
        //                }
        //            }else{
        //                throw new Exception("File already Exist, File: " + destinationFile);
        //            }
        //        } else {
        //            throw new FileNotFoundException("File: "+source);
        //        }
        //    } else {
        //        throw new DirectoryNotFoundException("Directory: " + Constants.DestinationDirectory);
        //    }
        //}

        public string SaveFile(string name, string source)
        {
            string destinationFile;
            if (Directory.Exists(Constants.DestinationDirectory)) {
                if (File.Exists(source)) {
                    destinationFile = Path.Combine(Constants.DestinationDirectory, name + Path.GetExtension(source));
                    if (!File.Exists(destinationFile)) {
                        File.Copy(source, destinationFile);
                        return destinationFile;
                    } else {
                        throw new Exception("File already Exist, File: " + destinationFile);
                    }
                } else {
                    throw new FileNotFoundException("File: " + source);
                }
            } else {
                throw new DirectoryNotFoundException("Directory: " + Constants.DestinationDirectory);
            }
        }
    }
}
