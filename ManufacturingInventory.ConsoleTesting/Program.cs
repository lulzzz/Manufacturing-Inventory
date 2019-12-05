using System;
using System.IO;
using IWshRuntimeLibrary;

namespace ManufacturingInventory.ConsoleTesting {
    public class Program {
        public static void Main(string[] args) {
            AddStartMenuShortCut();
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        public static void InstallerTesting() {
            if (CheckFileVersion()) {
                //string sourceDirectory = @"\\172.20.4.20\Manufacturing Install";
                //string targetDirectory = @"C:\Program Files (x86)\InventoryTestInstall";
                //checking another another another another
                string sourceDirectory = @"\\172.20.4.20\Testing Installs";
                string targetDirectory = @"C:\Program Files (x86)\ConsoleTestInstall";

                DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
                DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

                CopyAll(diSource, diTarget);


                Console.WriteLine();
                Console.WriteLine("Done!");

                Console.ReadKey();
            } else {
                Console.WriteLine();
                Console.WriteLine("Done!");
                Console.ReadKey();
            }
        }

        public static bool CheckFileVersion() {
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
            if(System.IO.File.Exists(@"\\172.20.4.20\Testing Installs\ManufacturingInventory.ConsoleTesting.exe")) {
                
                var serverVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(@"\\172.20.4.20\Testing Installs\ManufacturingInventory.ConsoleTesting.exe").FileVersion;
                if (fileVersion != serverVersion) {
                    Console.WriteLine("New Version is available!");
                    Console.WriteLine("Current Version: {0}", fileVersion);
                    Console.WriteLine("New Version: {0}", serverVersion);
                    return false;
                } else {
                    Console.WriteLine(fileVersion); // -> "1.2.32.2" 
                    return true;
                }
            } else {
                Console.WriteLine("Current Version: {0}", fileVersion);
                return false;
            }
        }

        public static void AddStartMenuShortCut() {
            string pathToExe = @"C:\Program Files (x86)\InventoryTestInstall\ManufacturingInventory.Main.exe";
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

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target) {
            if (source.FullName.ToLower() == target.FullName.ToLower()) {
                return;
            }

            if (!Directory.Exists(target.FullName)) {
                Directory.CreateDirectory(target.FullName);
            }

            foreach (FileInfo file in source.GetFiles()) {

                var dest = Path.Combine(target.ToString(), file.Name);
                if (System.IO.File.Exists(dest)) {
                    FileInfo info = new FileInfo(dest);
                    if (file.LastWriteTime!=info.LastWriteTime) {
                        Console.WriteLine(@"Copying {0}\{1}", target.FullName, file.Name);
                        file.CopyTo(Path.Combine(target.ToString(), file.Name), true);
                    } else {
                        Console.WriteLine(@"Not Copying {0}\{1}", target.FullName, file.Name);
                    }
                }
            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories()) {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}
