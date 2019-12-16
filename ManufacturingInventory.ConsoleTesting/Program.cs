using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using IWshRuntimeLibrary;
using ManufacturingInventory.Common.Model;
using ManufacturingInventory.Common.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManufacturingInventory.ConsoleTesting {
    public class Program {
        public static void Main(string[] args) {
            //ParameterTesting();
            //DistributorPriceTesting();
            //ReturnTransactionTest();
            //InitialUser();

            //TransactionTesting();
            //Process.Start(@"D:\Software Development\Manufacturing Inventory\ManufacturingInventory\ManufacturingInventory.Installer\bin\Release\netcoreapp3.1\publish\InventoryInstaller.exe");
        }

        #region DatabaseTesting

        public static void CreateLocations() {

        }

        public static void DistributorPriceTesting() {
            using var context = new ManufacturingContext();

            var warehouse = context.Locations.Find(5);

            if (warehouse != null) {
                Distributor dist1 = new Distributor();
                dist1.Name = "Distributor B";

                Distributor dist2 = new Distributor();
                dist2.Name = "Distributor A";

                context.Distributors.Add(dist1);
                context.Distributors.Add(dist2);

              
                var part = new Part();
                part.Name = "Process Chemicals";
                part.Warehouse = (Warehouse)warehouse;

                context.Parts.Add(part);
                
                var partInstance = new PartInstance(part, "IPA", "", "", "");
                partInstance.Quantity = 4;
                partInstance.SafeQuantity = 2;
                partInstance.MinQuantity = 1;
                partInstance.CostReported = true;
                partInstance.CurrentLocation = partInstance.Part.Warehouse;

                var partInstance2 = new PartInstance(part,"Acetone", "", "", "");
                partInstance2.Quantity = 4;
                partInstance2.SafeQuantity = 2;
                partInstance2.MinQuantity = 1;
                partInstance2.CostReported = true;
                partInstance2.CurrentLocation = partInstance2.Part.Warehouse;

                context.PartInstances.Add(partInstance);
                context.PartInstances.Add(partInstance2);



                Price p1 = new Price();
                p1.Amount = 4.99;
                p1.MinOrder = 10;
                p1.PartInstance = partInstance;
                p1.TimeStamp = DateTime.Now;
                p1.Distributor = dist1;

                Price p2 = new Price();
                p2.Amount = 8.99;
                p2.MinOrder = 10;
                p2.PartInstance = partInstance2;
                p2.TimeStamp = DateTime.Now;
                p2.Distributor = dist1;

                partInstance.Price = p1;
                partInstance.UnitCost = partInstance.Price.Amount;
                partInstance.TotalCost = partInstance.Price.Amount*partInstance.Quantity;

                partInstance2.Price = p2;
                partInstance2.UnitCost = partInstance2.Price.Amount;
                partInstance2.TotalCost = partInstance2.Price.Amount * partInstance2.Quantity;

                dist1.Prices.Add(p1);
                dist1.Prices.Add(p2);

                context.Prices.Add(p1);
                context.Prices.Add(p2);
                context.SaveChanges();

                Console.WriteLine("Should be done.  Maybe.....");
            } else {
                Console.WriteLine("Error finding warehouse");
            }

            Console.ReadKey();






        }

        public static void ReturnTransactionTest() {
            using var context = new ManufacturingContext();

            var outTransaction = context.Transactions.OfType<OutgoingTransaction>()
                .Include(e => e.Consumer)
                .Include(e => e.PartInstance)
                    .ThenInclude(e => e.InstanceParameter)
                .Include(e=>e.PartInstance)
                    .ThenInclude(e=>e.CurrentLocation)
                .Include(e=>e.PartInstance)
                    .ThenInclude(e=>e.Part)
                    .ThenInclude(e=>e.Warehouse)
                .FirstOrDefault(e => e.Id == 1);

            var user = context.Users
                .Include(e => e.Sessions)
                    .ThenInclude(e => e.Transactions)
                .Include(e => e.Permission)
                .FirstOrDefault(e => e.FirstName == "Andrew");

            Condition condition1 = new Condition();
            condition1.Name = "Used";
            condition1.Description = "Partial";

            Condition condition2 = new Condition();
            condition2.Name = "New";
            condition2.Description = "Unused";

            Condition condition3 = new Condition();
            condition3.Name = "Empty";
            condition3.Description = "Empty";

            context.Categories.Add(condition1);
            context.Categories.Add(condition2);
            context.Categories.Add(condition3);


            if (outTransaction != null && user!=null) {
                //var partInstance = context.PartInstances.Include(e => e.Id == outTransaction.PartInstanceId);
                Session session = new Session(user);
                context.Sessions.Add(session);

                var partInstance = context.Entry<PartInstance>(outTransaction.PartInstance).Entity;
                
                ReturningTransaction returnTransaction = new ReturningTransaction();
                returnTransaction.InstanceParameterValue = 1000;
                returnTransaction.OutgoingTransaction = outTransaction;
                returnTransaction.PartInstance = partInstance;
                returnTransaction.Session = session;

                context.Transactions.Add(returnTransaction);
                outTransaction.ReturningTransaction = returnTransaction;

                partInstance.CurrentLocation = partInstance.Part.Warehouse;
                partInstance.InstanceParameter.Value = returnTransaction.InstanceParameterValue;
                partInstance.Condition = condition1;

                context.Entry<PartInstance>(partInstance).State = EntityState.Modified;
                context.Entry<Warehouse>(partInstance.Part.Warehouse).State = EntityState.Modified;
                context.Entry<InstanceParameter>(partInstance.InstanceParameter).State = EntityState.Modified;
                context.Entry<OutgoingTransaction>(outTransaction).State = EntityState.Modified;
                context.Transactions.Add(returnTransaction);

                context.SaveChanges();

                Console.WriteLine("Should be done, maybe....");
            } else {
                Console.WriteLine("Error finding transaction");
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
