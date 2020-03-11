using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IWshRuntimeLibrary;
using Microsoft.EntityFrameworkCore;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Domain.Buisness.Interfaces;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Domain.Buisness.Concrete;
using ManufacturingInventory.Application.UseCases;

using ManufacturingInventory.Application.Boundaries.Checkout;
using ManufacturingInventory.Infrastructure.Model.Providers;
using System.Collections.ObjectModel;
using ManufacturingInventory.Application.Boundaries.PartInstanceDetailsEdit;
using ManufacturingInventory.Application.Boundaries.CheckIn;

namespace ManufacturingInventory.ConsoleTesting {
    public class Program {
        public static async Task<int> Main(string[] args) {
            using var context = new ManufacturingContext();
            var distributor = context.Distributors.FirstOrDefault(e=>e.Id==13);
            await context.AddAsync(distributor);
           // context.Update(new Distributor("AETestDistributor","Some Description"));
            await context.SaveChangesAsync();
            Console.WriteLine("Should be done");
            return 1;
        }

        public static async Task<int> UndoTransactionTesting() {
            using var context = new ManufacturingContext();
            IRepository<PartInstance> partInstanceRepo = new PartInstanceRepository(context);
            IRepository<Transaction> transactionRepo = new TransactionRepository(context);
            Console.WriteLine("Removing Transaction");
            var transaction = await transactionRepo.GetEntityAsync(e => e.Id == 18);
            if (transaction != null) {
                var partInstance = await partInstanceRepo.GetEntityAsync(e => e.Id == transaction.PartInstanceId);
                if (partInstance != null) {
                    partInstance.Quantity = 1;
                    partInstance.BubblerParameter.UpdateWeight(transaction.MeasuredWeight);
                    partInstance.LocationId = partInstance.Part.WarehouseId.Value;
                    if (!partInstance.DateRemoved.HasValue) {
                        partInstance.DateInstalled = null;
                    }
                    if (partInstance.Transactions.Remove(transaction)) {
                        var updated = await partInstanceRepo.UpdateAsync(partInstance);
                        var deleted = await transactionRepo.DeleteAsync(transaction);
                        if (updated != null && deleted != null) {
                            await context.SaveChangesAsync();
                            Console.WriteLine("Success:  Transaction should be deleted and partInstance updated");
                        } else {
                            Console.WriteLine("Failed to update partInstance");
                        }
                    } else {
                        Console.WriteLine("Failed to remove");
                    }
                } else {
                    Console.WriteLine("Failed: Error finding partInstance");
                }
            } else {
                Console.WriteLine("Error finding transaction");
            }
            Console.ReadKey();
            return 1;
        }

        //#region DatabaseTesting

        public static void Default() {
            Test();

            //UseCaseTesting();
            //CreateLocations();
            //CreateDistibutors();
            //CreateCategories();
            //DistributorPriceTesting();
            //TransactionTesting();
            //ReturnTransactionTest();
            //Process.Start(@"D:\Software Development\Manufacturing Inventory\ManufacturingInventory\ManufacturingInventory.Installer\bin\Release\netcoreapp3.1\publish\InventoryInstaller.exe");
            //AddAnotherPrice();
        }

        public static void Test(int? test = null) {
            int? test1 = test;
            Console.WriteLine("Has Value: {0}", test1.HasValue);
            Console.ReadKey();

        }

        public static void InitialCreate() {
            using var context = new ManufacturingContext();

            var tma = context.PartInstances
                .Include(e => e.Price).Include(e => e.BubblerParameter)
                .Single(e => e.SerialNumber == "1000006109");
            tma.UpdateWeight(2100);
            context.Entry<PartInstance>(tma).State = EntityState.Modified;
            context.SaveChanges();
            Console.WriteLine("Weight Changed");
            Console.ReadKey();
        }

        public static void CreateLocations() {
            using var context = new ManufacturingContext();
            var warehouse1 = new Warehouse();
            warehouse1.Name = "Epi System Parts";
            warehouse1.Description = "Storage room for replacement epi parts";

            var warehouse2 = new Warehouse();
            warehouse2.Name = "IT Storage Room";
            warehouse2.Description = "Storage Room for all things IT";

            var consumer1 = new Consumer();
            consumer1.Name = "System B01";
            consumer1.Description = "B01 EPI System";

            var consumer2 = new Consumer();
            consumer2.Name = "System A01";
            consumer2.Description = "B01 EPI System";

            var consumer3 = new Consumer();
            consumer3.Name = "System B02";
            consumer3.Description = "B02 EPI System";

            var consumer4 = new Consumer();
            consumer4.Name = "System B03";
            consumer4.Description = "B03 EPI System";

            var consumer5 = new Consumer();
            consumer5.Name = "System A02";
            consumer5.Description = "A02 EPI System";

            var consumer6 = new Consumer();
            consumer6.Name = "System A03";
            consumer6.Description = "A03 EPI System";

            var consumer7 = new Consumer();
            consumer7.Name = "System A04";
            consumer7.Description = "A04 EPI System";

            context.Locations.Add(warehouse1);
            context.Locations.Add(warehouse2);

            context.Locations.Add(consumer1);
            context.Locations.Add(consumer2);
            context.Locations.Add(consumer3);
            context.Locations.Add(consumer4);
            context.Locations.Add(consumer5);
            context.Locations.Add(consumer6);
            context.Locations.Add(consumer7);
            context.SaveChanges();
            Console.WriteLine("Locations Created,Press Any key To Continue");
            Console.ReadKey();
        }

        public static void CreateDistibutors() {
            using var context = new ManufacturingContext();

            Distributor dist1 = new Distributor();
            dist1.Name = "Distributor B";

            Distributor dist2 = new Distributor();
            dist2.Name = "Distributor A";

            Distributor dist3 = new Distributor();
            dist3.Name = "Distributor C";

            Distributor dist4 = new Distributor();
            dist4.Name = "Distributor D";

            Distributor dist5 = new Distributor();
            dist5.Name = "Distributor E";

            Distributor dist6 = new Distributor();
            dist6.Name = "Akzo Nobel";

            context.Distributors.Add(dist1);
            context.Distributors.Add(dist2);
            context.Distributors.Add(dist3);
            context.Distributors.Add(dist4);
            context.Distributors.Add(dist5);
            context.Distributors.Add(dist6);

            context.SaveChanges();
            Console.WriteLine("Distributors Created,Press any key to continue");
            Console.ReadKey();
        }

        public static void CreateCategories() {
            using var context = new ManufacturingContext();

            Condition condition1 = new Condition();
            condition1.Name = "Used";
            condition1.Description = "Partial";

            Condition condition2 = new Condition();
            condition2.Name = "New";
            condition2.Description = "Unused";

            Condition condition3 = new Condition();
            condition3.Name = "Empty";
            condition3.Description = "Empty";

            var Usage1 = new Usage();
            Usage1.Name = "System B01";
            Usage1.Description = "B01 EPI System";

            var Usage2 = new Usage();
            Usage2.Name = "System A01";
            Usage2.Description = "B01 EPI System";

            var Usage3 = new Usage();
            Usage3.Name = "System B02";
            Usage3.Description = "B02 EPI System";

            var Usage4 = new Usage();
            Usage4.Name = "System B03";
            Usage4.Description = "B03 EPI System";

            var Usage5 = new Usage();
            Usage5.Name = "System A02";
            Usage5.Description = "A02 EPI System";

            var Usage6 = new Usage();
            Usage6.Name = "System A03";
            Usage6.Description = "A03 EPI System";

            var Usage7 = new Usage();
            Usage7.Name = "System A04";
            Usage7.Description = "A04 EPI System";

            var org1 = new Organization("System Parts", "");
            var org2 = new Organization("Facility Parts", "");


            context.Categories.Add(condition1);
            context.Categories.Add(condition2);
            context.Categories.Add(condition3);

            context.Categories.Add(org1);
            context.Categories.Add(org2);
            context.Categories.Add(Usage1);
            context.Categories.Add(Usage2);
            context.Categories.Add(Usage3);
            context.Categories.Add(Usage4);
            context.Categories.Add(Usage5);
            context.Categories.Add(Usage6);
            context.Categories.Add(Usage7);

            context.SaveChanges();
            Console.WriteLine("Categories Created!");
            Console.ReadKey();



        }

        public static void DistributorPriceTesting() {
            using var context = new ManufacturingContext();

            var warehouse = context.Locations.OfType<Warehouse>().FirstOrDefault(e => e.Name == "Epi System Parts");
            var condition = context.Categories.OfType<Condition>().FirstOrDefault(e => e.Name == "New");
            var usage = context.Categories.OfType<Organization>().FirstOrDefault(e => e.Name == "System Parts");
            if (warehouse != null && condition != null && usage != null) {

                var dist = context.Distributors.FirstOrDefault(e => e.Name == "Distributor A");
                var dist2 = context.Distributors.FirstOrDefault(e => e.Name == "Akzo Nobel");

                if (dist != null && dist2 != null) {
                    var part = CreatePart("Process Chemicals", warehouse);
                    var p1 = CreatePrice(4.99, 10, dist.Id);
                    var p2 = CreatePrice(4.99, 10, dist.Id);

                    context.Add(part);
                    context.Add(p1);
                    context.Add(p2);

                    context.Add(new PartPrice(part,p1));
                    context.Add(new PartPrice(part,p2));

                    var instance1 = CreatePartInstance("IPA", part, 8, 4, 2);
                    instance1.Condition = condition;

                    var instance2 = CreatePartInstance("Acetone", part, 16, 2, 1);
                    instance2.Condition = condition;
                 
                    instance1.Price = p1;
                    instance1.UnitCost = p1.UnitCost;
                    instance1.TotalCost = instance1.UnitCost * instance1.Quantity;

                    instance2.Price = p2;
                    instance2.UnitCost = p2.UnitCost;
                    instance2.TotalCost = instance2.UnitCost * instance2.Quantity;

                   
                    context.Add(instance1);
                    context.Add(instance2);

                    context.PriceLogs.Add(new PriceLog() { Price = p1, PartInstance = instance1, IsCurrent = true });
                    context.PriceLogs.Add(new PriceLog() { Price = p2, PartInstance = instance2, IsCurrent = true });

                    context.SaveChanges();
                    
                    Console.WriteLine("Parts Created,Press any key to continue");
                } else {
                    Console.WriteLine("Error Finding Distributor");
                }
            } else {
                Console.WriteLine("Error finding warehous");
            }
            Console.ReadKey();
        }

        public static void AddAnotherPrice() {
            using var context = new ManufacturingContext();
            var part = context.Parts.FirstOrDefault(e => e.Id == 1);
            var instance = context.PartInstances.Include(e=>e.Price).FirstOrDefault(e => e.Id == 1);
            var dist = context.Distributors.FirstOrDefault(e => e.Id == 2);

            if (instance.PriceId.HasValue) {
                var log = context.PriceLogs
                    .Include(e=>e.Price)
                    .Include(e=>e.PartInstance)
                    .Where(e => 
                    e.PriceId == instance.PriceId && 
                    e.PartInstanceId==instance.Id 
                    && e.IsCurrent).ToList();

                log.ForEach(line => { 
                    line.IsCurrent = false;
                    context.Update(line);
                });
            }

            var price = CreatePrice(10.24, 50, dist.Id);

            context.Add(price);
            context.Add(new PartPrice(part, price));

            instance.Price = price;

            instance.UnitCost = price.UnitCost;
            instance.TotalCost = instance.UnitCost * instance.Quantity;

            context.PriceLogs.Add(new PriceLog() { PartInstance=instance,Price=price,IsCurrent=true,TimeStamp=DateTime.Now});


            context.SaveChanges();
            Console.WriteLine("Price Should be Changed");
            Console.ReadKey();
        }
        
        public static void CreateBubblers() {
            using var context = new ManufacturingContext();
            var warehouse = context.Locations.OfType<Warehouse>().FirstOrDefault(e => e.Name == "Epi System Parts");
            var condition = context.Categories.OfType<Condition>().FirstOrDefault(e => e.Name == "New");
            var usage = context.Categories.OfType<Organization>().FirstOrDefault(e => e.Name == "System Parts");
            if (warehouse != null && condition != null && usage != null) {
                var dist = context.Distributors.FirstOrDefault(e => e.Name == "Distributor A");
                var dist2 = context.Distributors.FirstOrDefault(e => e.Name == "Akzo Nobel");

                var part2 = CreatePart("Bubblers", warehouse);
                var tma1 = CreateBubbler("TMA", part2, 380, 2243, 0, "1607AL1203", "1000006109");
                var tma2 = CreateBubbler("TMA", part2, 380, 2316, 0, "1509AL1152", "1000009476");
                var tma3 = CreateBubbler("TMA", part2, 380, 2283, 0, "1604AL1190", "1000000192");
                var tmg1 = CreateBubbler("TMG", part2, 600, 2497, 1, "1211GM2059", "600VW2676");
                var tmg2 = CreateBubbler("TMG", part2, 600, 2526, 1, "1507GM2598", "1000000813");

                tma1.Condition = condition;
                tma2.Condition = condition;
                tma3.Condition = condition;
                tmg1.Condition = condition;
                tmg2.Condition = condition;

                part2.PartInstances.Add(tma1);
                part2.PartInstances.Add(tma2);
                part2.PartInstances.Add(tma3);
                part2.PartInstances.Add(tma1);
                part2.PartInstances.Add(tmg1);
                part2.PartInstances.Add(tmg2);

                var p1_2 = CreatePrice(64.25, 0, dist2.Id);
                var p2_2 = CreatePrice(65.04, 0, dist2.Id);
                var p3_2 = CreatePrice(38.45, 0, dist2.Id);
                var p4_2 = CreatePrice(45.78, 0, dist2.Id);
                var p5_2 = CreatePrice(62.88, 0, dist2.Id);

                tma1.UpdatePrice(p1_2);
                tma2.UpdatePrice(p2_2);
                tma3.UpdatePrice(p3_2);
                tmg1.UpdatePrice(p4_2);
                tmg2.UpdatePrice(p5_2);

                //dist2.Prices.Add(p1_2);
                //dist2.Prices.Add(p2_2);
                //dist2.Prices.Add(p3_2);
                //dist2.Prices.Add(p4_2);
                //dist2.Prices.Add(p5_2);

                //context.Prices.Add(p1_2);
                //context.Prices.Add(p2_2);
                //context.Prices.Add(p3_2);
                //context.Prices.Add(p4_2);
                //context.Prices.Add(p5_2);

                context.Parts.Add(part2);


                context.Entry<Distributor>(dist).State = EntityState.Modified;
                context.Entry<Distributor>(dist2).State = EntityState.Modified;
                context.SaveChanges();
            }


        }

        public static Part CreatePart(string name, Warehouse warehouse) {
            var part = new Part();
            part.Name = name;
            part.Warehouse = (Warehouse)warehouse;
            return part;
        }

        public static PartInstance CreatePartInstance(string name, Part part, int quantity, int safe, int min) {
            Random rand = new Random();
            var partInstance = new PartInstance(part, name, rand.Next(2345623, 99999999).ToString(), rand.Next(100, 1500).ToString(), "", false);
            partInstance.Quantity = quantity;
            partInstance.SafeQuantity = safe;
            partInstance.MinQuantity = min;
            partInstance.CostReported = true;
            partInstance.CurrentLocation = part.Warehouse;
            return partInstance;
        }

        public static PartInstance CreateBubbler(string name, Part part, double net, double gross, double tare, string lot, string sn) {
            var param = new BubblerParameter(net, gross, tare);
            param.Weight = net;
            param.Measured = gross;
            var bubbler = new PartInstance(part, name, sn, lot, "", true, param);
            bubbler.Part = part;
            bubbler.CurrentLocation = part.Warehouse;
            bubbler.CostReported = true;
            bubbler.IsBubbler = true;
            bubbler.Quantity = 1;
            return bubbler;
        }

        public static Price CreatePrice(double unitCost, int min, int distributorId) {
            Price price = new Price();
            price.UnitCost = unitCost;
            price.MinOrder = min;
            price.TimeStamp = DateTime.Now;
            price.DistributorId = distributorId;
            return price;
        }

        //public static void CreateParts() {
        //    using var context = new ManufacturingContext();

        //    var warehouse = context.Locations.OfType<Warehouse>().FirstOrDefault(e => e.Name == "Epi System Parts");
        //    if (warehouse != null) {
        //        Unit unit = new Unit("Gram", "g", 10, 0);
        //        Parameter parameter = new Parameter("Bubbler Weight", "");
        //        parameter.Unit = unit;

        //        context.Units.Add(unit);
        //        context.Parameters.Add(parameter);

        //        Part part = new Part();
        //        part.Name = "Bubblers";
        //        part.Description = "Bubblers";
        //        part.Warehouse = warehouse;
        //        part.HoldsBubblers = true;
        //        context.Parts.Add(part);

        //        PartInstance partInstance = new PartInstance(part, "TMA", "", "", "", false);
        //        partInstance.Quantity = 1;
        //        partInstance.CurrentLocation = warehouse;

        //        context.PartInstances.Add(partInstance);

        //        InstanceParameter instanceParameter = new InstanceParameter(partInstance, parameter);
        //        instanceParameter.MinValue = 200;
        //        instanceParameter.SafeValue = 400;
        //        instanceParameter.Value = 1861;
        //        instanceParameter.Tracked = true;

        //        context.InstanceParameters.Add(instanceParameter);

        //        context.SaveChanges();
        //        Console.WriteLine("Should be done!");

        //    } else {
        //        Console.WriteLine("Error finding warehouse");
        //    }
        //    Console.ReadKey();
        //}

        //public static void ReturnTransactionTest() {
        //    using var context = new ManufacturingContext();

        //    var outTransaction = context.Transactions.OfType<Transaction>()
        //        .Include(e => e.Location)
        //        .Include(e => e.PartInstance)
        //            .ThenInclude(e => e.CurrentLocation)
        //        .Include(e => e.PartInstance)
        //            .ThenInclude(e => e.Part)
        //                .ThenInclude(e => e.Warehouse)
        //         .Include(e => e.PartInstance)
        //            .ThenInclude(e => e.Price)
        //         .Include(e => e.PartInstance)
        //            .ThenInclude(e => e.BubblerParameter)
        //        .Include(e => e.Session)
        //            .ThenInclude(e => e.User)
        //        .FirstOrDefault(e => e.Id == 1);

        //    var user = context.Users
        //        .Include(e => e.Sessions)
        //            .ThenInclude(e => e.Transactions)
        //        .Include(e => e.Permission)
        //        .FirstOrDefault(e => e.FirstName == "Andrew");

        //    var condition = context.Categories.OfType<Condition>().FirstOrDefault(e => e.Name == "Used");


        //    if (outTransaction != null && user != null && condition != null) {
        //        //var partInstance = context.PartInstances.Include(e => e.Id == outTransaction.PartInstanceId);
        //        Session session = new Session(user);
        //        context.Sessions.Add(session);

        //        var partInstance = context.Entry<PartInstance>((PartInstance)outTransaction.PartInstance).Entity;

        //        partInstance.CurrentLocation = partInstance.Part.Warehouse;
        //        partInstance.UpdateWeight(1900);
        //        partInstance.Condition = condition;

        //        Transaction returnTransaction = new Transaction(partInstance, InventoryAction.RETURNING, partInstance.BubblerParameter.Measured,partInstance.BubblerParameter.Weight, partInstance.Part.Warehouse, outTransaction);
        //        //returnTransaction.InventoryAction = InventoryAction.RETURNING;
        //        //returnTransaction.Quantity = 1;
        //        //returnTransaction.ParameterValue = partInstance.BubblerParameter.Weight;
        //        //returnTransaction.ReferenceTransaction = outTransaction;
        //        //returnTransaction.PartInstance = partInstance;
        //        returnTransaction.Session = session;
        //        context.Transactions.Add(returnTransaction);

        //        context.Entry<PartInstance>(partInstance).State = EntityState.Modified;
        //        context.Entry<Warehouse>(partInstance.Part.Warehouse).State = EntityState.Modified;
        //        //context.Entry<InstanceParameter>(partInstance.InstanceParameter).State = EntityState.Modified;
        //        context.Entry<Transaction>(outTransaction).State = EntityState.Modified;
        //        context.Transactions.Add(returnTransaction);

        //        context.SaveChanges();

        //        Console.WriteLine("Should be done, maybe....");
        //    } else {
        //        Console.WriteLine("Error finding transaction");
        //    }
        //    Console.ReadKey();
        //}

        //public static void TransactionTesting() {
        //    using var context = new ManufacturingContext();

        //    var consumer = context.Locations.OfType<Consumer>()
        //        .Include(e => e.ItemsAtLocation)
        //            .ThenInclude(e => e.CurrentLocation)
        //        .Include(e => e.Transactions)
        //        .FirstOrDefault(e => e.Name == "System B03");

        //    var tma1 = context.PartInstances.OfType<PartInstance>()
        //        .Include(e => e.Part)
        //            .ThenInclude(e => e.Warehouse)
        //        .Include(e => e.CurrentLocation)
        //        .Include(e => e.Price)
        //        .Include(e => e.BubblerParameter)
        //        .FirstOrDefault(e => e.SerialNumber == "1000006109");

        //    var user = context.Users
        //        .Include(e => e.Sessions)
        //            .ThenInclude(e => e.Transactions)
        //        .Include(e => e.Permission)
        //        .FirstOrDefault(e => e.FirstName == "Andrew");


        //    if (tma1 != null && consumer != null && user != null) {
        //        Session session = new Session(user);
        //        context.Sessions.Add(session);

        //        tma1.CurrentLocation = consumer;
        //        tma1.UpdateWeight(2100);

        //        Transaction outgoing = new Transaction(tma1, InventoryAction.OUTGOING, tma1.BubblerParameter.Weight, tma1.BubblerParameter.Weight, true, consumer);
        //        //outgoing.Location = consumer;
        //        //outgoing.PartInstance = tma1;
        //        //outgoing.InventoryAction = InventoryAction.OUTGOING;
        //        //outgoing.IsReturning = true;
        //        //outgoing.Quantity = 1;
        //        /*outgoing.ParameterValue = tma1.BubblerParameter.Weight; */ //TODO: Cleaup check in/Check out
        //        outgoing.Session = session;
        //        context.Transactions.Add(outgoing);
        //        context.Entry<PartInstance>(tma1).State = EntityState.Modified;
        //        context.Entry<Consumer>(consumer).State = EntityState.Modified;
        //        context.SaveChanges();
        //        Console.WriteLine("Should be done!");

        //    } else {
        //        Console.WriteLine("Error, Consumer or Instance not Found");
        //    }
        //    Console.ReadKey();
        //}

        //public static void InitialUser() {
        //    using var context = new ManufacturingContext();
        //    Permission permission1 = new Permission() {
        //        Name = "InventoryAdminAccount",
        //        Description = "Full Inventory Privileges and User Control"
        //    };

        //    Permission permission2 = new Permission() {
        //        Name = "InventoryUserAccount",
        //        Description = "Inventory View Only"
        //    };

        //    Permission permission3 = new Permission() {
        //        Name = "InventoryUserFullAccount",
        //        Description = "Full Inventory Privileges"
        //    };

        //    Permission permission4 = new Permission() {
        //        Name = "InventoryUserLimitedAccount",
        //        Description = "Inventory Check In/Check Out/Create"
        //    };

        //    context.Permissions.Add(permission1);
        //    context.Permissions.Add(permission2);
        //    context.Permissions.Add(permission3);
        //    context.Permissions.Add(permission4);

        //    User user = new User();
        //    user.FirstName = "Andrew";
        //    user.LastName = "Elmendorf";

        //    user.Permission = permission1;
        //    context.Users.Add(user);
        //    context.SaveChanges();
        //    Console.WriteLine("Should be done");
        //    Console.ReadKey();
        //}

        //#endregion

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
