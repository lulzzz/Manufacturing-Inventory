using System;
using System.IO;
using ManufacturingInventory.InstallSequence.Infrastructure;

namespace ManufacturingInventory.InstallConsole {
    class Program {
        static void Main(string[] args) {
            var response=VersionChecker.CheckInstalledVersion();
            Console.WriteLine("Message: {0}",response.Message);
            Console.WriteLine("Server Version: {0}",response.Version);
            Console.WriteLine("Should be done");
            Console.ReadKey();
        }

        public static void Testing() {
            string sourceDirectory = @"\\172.20.4.20\Manufacturing Install";
            string targetDirectory = @"C:\Program Files (x86)\InventoryTestInstall";
            //string sourceDirectory = @"\\172.20.4.20\Testing Installs";
            //string targetDirectory = @"C:\Program Files (x86)\ConsoleTestInstall";

            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);

            Console.WriteLine();
            Console.WriteLine("Done!");

            Console.ReadKey();
        }


        public static void CopyAll(DirectoryInfo source, DirectoryInfo target) {
            if (source.FullName.ToLower() == target.FullName.ToLower()) {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false) {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles()) {
                var dest = Path.Combine(target.ToString(), fi.Name);
                if (File.Exists(dest)) {
                    FileInfo info = new FileInfo(dest);
                    if (fi.LastWriteTime != info.LastWriteTime) {
                        //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                        fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
                    } else {
                        Console.WriteLine(@"Not Copying {0}\{1}", target.FullName, fi.Name);
                    }
                }

            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories()) {
                DirectoryInfo nextTargetSubDir =target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    
    
    }


}
