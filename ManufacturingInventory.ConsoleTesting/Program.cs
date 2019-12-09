using System;
using System.IO;
using System.Linq;
using IWshRuntimeLibrary;
using ManufacturingInventory.Common.Model;
using ManufacturingInventory.Common.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManufacturingInventory.ConsoleTesting {
    public class Program {
        public static void Main(string[] args) {
            //TransactionTesting();
            InitialUser();
            ParameterTesting();
            TransactionTesting();
        }

        #region DatabaseTesting

        public static void ReturnTransactionTest() {
            using var context = new ManufacturingContext();

            var outTransaction = context.Transactions.OfType<OutgoingTransaction>()
                .Include(e => e.Consumer)
                .Include(e => e.PartInstance)
                    .ThenInclude(e => e.InstanceParameter)
                .FirstOrDefault(e => e.Id == 2);

            if (outTransaction != null) {
                ReturningTransaction returnTransaction = new ReturningTransaction();
                returnTransaction.InstanceParameterValue = 1000;
                returnTransaction.OutgoingTransaction = outTransaction;
                returnTransaction.PartInstance = outTransaction.PartInstance;

            } else {
                Console.WriteLine("Erro finding transaction");
            }
            Console.ReadKey();
        }

        public static void TransactionTesting() {
            using var context = new ManufacturingContext();

            var consumer = context.Locations.OfType<Consumer>()
                .Include(e => e.ItemsAtLocation)
                    .ThenInclude(e => e.CurrentLocation)
                .Include(e => e.OutgoingTransactions)
                .FirstOrDefault(e => e.Name == "System B03");

            var instance = context.PartInstances
                .Include(e => e.Part)
                    .ThenInclude(e => e.Warehouse)
                .Include(e => e.CurrentLocation)
                .Include(e => e.InstanceParameter)
                    .ThenInclude(e => e.Parameter)
                    .ThenInclude(e => e.Unit)
                .Include(e => e.Price)
                .Single(e => e.Name =="TMA");

            var user = context.Users
                .Include(e => e.Sessions)
                    .ThenInclude(e => e.Transactions)
                .Include(e => e.Permission)
                .FirstOrDefault(e => e.FirstName == "Andrew");
                

            if(instance!=null && consumer != null && user!=null) {
                Session session = new Session(user);
                context.Sessions.Add(session);

                instance.CurrentLocation = consumer;


                OutgoingTransaction outgoing = new OutgoingTransaction();
                outgoing.Consumer = consumer;
                outgoing.PartInstance = instance;
                outgoing.InventoryAction = InventoryAction.OUTGOING;
                outgoing.IsReturning = true;
                outgoing.Quantity = 1;
                outgoing.InstanceParameterValue =(instance.InstanceParameter!=null)? instance.InstanceParameter.Value:0;
                outgoing.Session = session;
                context.Transactions.Add(outgoing);
                context.Entry<PartInstance>(instance).State = EntityState.Modified;
                context.Entry<Consumer>(consumer).State = EntityState.Modified;
                context.SaveChanges();
                Console.WriteLine("Should be done!");

            } else {
                Console.WriteLine("Error, Consumer or Instance not Found");
            }
            Console.ReadKey();
        }

        public static void ParameterTesting() {
            using var context = new ManufacturingContext();

            Warehouse warehouse = new Warehouse();
            warehouse.Name = "Epi System Parts";
            warehouse.Description = "Storage room for replacement epi parts";

            Consumer consumer1 = new Consumer();
            consumer1.Name = "System B01";
            consumer1.Description = "System B01 EPI System";

            Consumer consumer2 = new Consumer();
            consumer1.Name = "System A01";
            consumer1.Description = "System B01 EPI System";

            Consumer consumer3 = new Consumer();
            consumer1.Name = "System B02";
            consumer1.Description = "System B01 EPI System";

            Consumer consumer4 = new Consumer();
            consumer1.Name = "System B03";
            consumer1.Description = "System B01 EPI System";

            context.Locations.Add(consumer1);
            context.Locations.Add(consumer2);
            context.Locations.Add(consumer3);
            context.Locations.Add(consumer4);
            context.Locations.Add(warehouse);

            Unit unit = new Unit("Gram", "g", 10, 0);
            Parameter parameter = new Parameter("Bubbler Weight", "");
            parameter.Unit = unit;

            context.Units.Add(unit);
            context.Parameters.Add(parameter);

            Part part = new Part();
            part.Name = "Bubblers";
            part.Description = "Bubblers";
            part.Warehouse = warehouse;

            context.Parts.Add(part);

            PartInstance partInstance = new PartInstance(part, "TMA", "", "", "");
            partInstance.Quantity = 1;
            partInstance.CurrentLocation = warehouse;

            context.PartInstances.Add(partInstance);

            InstanceParameter instanceParameter = new InstanceParameter(partInstance, parameter);
            instanceParameter.MinValue = 200;
            instanceParameter.SafeValue = 400;
            instanceParameter.Value = 1861;
            instanceParameter.Tracked = true;

            context.InstanceParameters.Add(instanceParameter);

            context.SaveChanges();
            Console.WriteLine("Should be done!");
            Console.ReadKey();
        }

        public static void InitialUser() {
            using var context = new ManufacturingContext();


            Permission permission1 = new Permission() {
                Name = "InventoryAdminAccount",
                Description = "Full Inventory Privileges and User Control"
            };

            Permission permission2 = new Permission() {
                Name = "InventoryUserAccount",
                Description = "Inventory View Only"
            };

            Permission permission3 = new Permission() {
                Name = "InventoryUserFullAccount",
                Description = "Full Inventory Privileges"
            };

            Permission permission4 = new Permission() {
                Name = "InventoryUserLimitedAccount",
                Description = "Inventory Check In/Check Out/Create"
            };

            context.Permissions.Add(permission1);
            context.Permissions.Add(permission2);
            context.Permissions.Add(permission3);
            context.Permissions.Add(permission4);

            User user = new User();
            user.FirstName = "Andrew";
            user.LastName = "Elmendorf";

            user.Permission = permission1;
            context.Users.Add(user);
            context.SaveChanges();
            Console.WriteLine("Should be done");
            Console.ReadKey();
        }
        
        #endregion

        #region InstallTesting

        public static void InstallerMain() {
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
                DirectoryInfo nextTargetSubDir =target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        
        #endregion
    }
}
