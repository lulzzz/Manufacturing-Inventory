using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Domain.Buisness.Concrete;
using ManufacturingInventory.Application.UseCases;
using System.Collections.Generic;
using ManufacturingInventory.Common.Application;
using System.Data.OleDb;
using Nito.AsyncEx;

namespace ManufacturingInventory.ConsoleTesting {
    public class ModelTesting {

        public static void Main(string[] args) {

            //syncContext.Run(AddAlertToAllInstances);
            //AsyncContext.Run(DeletingAlerts);
            //AddAlertToAllInstances();
            AsyncContext.Run(ChangeStockTypeThenAlert);
        }

        public static async Task ChangeStockTypeThenAlert() {
            DbContextOptionsBuilder<ManufacturingContext> optionsBuilder = new DbContextOptionsBuilder<ManufacturingContext>();
            optionsBuilder.UseSqlServer("server=172.27.192.1;database=manufacturing_inventory;user=aelmendorf;password=Drizzle123!;MultipleActiveResultSets=true");
            using var context = new ManufacturingContext(optionsBuilder.Options);
            var user = context.Users
                .Include(e => e.Sessions)
                    .ThenInclude(e => e.Transactions)
                .Include(e => e.Permission)
                .FirstOrDefault(e => e.FirstName == "Andrew");
            UserService userService = new UserService();
            if (user != null) {
                Session session = new Session(user);
                context.Sessions.Add(session);
                context.SaveChanges();
                userService.CurrentUserName = user.UserName;
                userService.CurrentSessionId = session.Id;
                userService.UserPermissionName = user.Permission.Name;
            }
            var partInstance = await context.PartInstances.Include(e => e.IndividualAlert).Include(e => e.StockType).ThenInclude(e => e.CombinedAlert).FirstOrDefaultAsync(e=>e.Id==66);
            var newStockType = await context.Categories.OfType<StockType>().Include(e => e.CombinedAlert).ThenInclude(e => e.UserAlerts).FirstOrDefaultAsync(e => e.Id == 14);
            if (partInstance != null && newStockType!=null) {
                if (newStockType.IsDefault) {
                    if (!partInstance.StockType.IsDefault) {
                        //from combinded to individual
                        Console.WriteLine("Combined to Individual");
                        IndividualAlert alert = new IndividualAlert();
                        alert.PartInstance = partInstance;
                        partInstance.IndividualAlert = alert;
                        context.Add(alert);
                        partInstance.StockType = newStockType;
                        context.Update(partInstance);
                        await context.SaveChangesAsync();
                        Console.WriteLine("Case one should be done");
                    } else {
                        //from individual to individual.  Should never be here
                        Console.WriteLine("You should not be here");
                    }
                } else {
                    if (partInstance.StockType.IsDefault) {
                        if (partInstance.IndividualAlert != null) {
                            //from individual to combined
                            var userAlerts = context.UserAlerts.Where(e => e.AlertId == partInstance.IndividualAlertId);
                            context.RemoveRange(userAlerts);
                            var deleted = context.Alerts.Remove(partInstance.IndividualAlert);
                            partInstance.IndividualAlert = null;
                            partInstance.StockType = newStockType;
                            newStockType.Quantity += partInstance.Quantity;
                            context.Update(newStockType);
                            await context.SaveChangesAsync();
                            Console.WriteLine("Should be done");
                        } else {
                            Console.WriteLine("You should not be here");
                        }
                    } else {
                        //from combined to another combined
                        Console.WriteLine("Combined to Combined");
                        var oldStock=context.Entry<StockType>(partInstance.StockType).Entity;
                        oldStock.Quantity-= partInstance.Quantity;
                        var okay=oldStock.PartInstances.Remove(partInstance);
                        partInstance.StockType = newStockType;
                        newStockType.PartInstances.Add(partInstance);
                        newStockType.Quantity += partInstance.Quantity;

                        context.Update(newStockType);
                        context.Update(oldStock);
                        context.Update(partInstance);
                        await context.SaveChangesAsync();
                        Console.WriteLine("Should be finished");
                    }
                }
                Console.WriteLine("Done, Press any key to exit");
            } else {
                Console.WriteLine("PartInstance not found");
            }

        }

        public static void AddAlertToAllInstances() {
            DbContextOptionsBuilder<ManufacturingContext> optionsBuilder = new DbContextOptionsBuilder<ManufacturingContext>();
            optionsBuilder.UseSqlServer("server=172.27.192.1;database=manufacturing_inventory;user=aelmendorf;password=Drizzle123!;MultipleActiveResultSets=true");
            using var context = new ManufacturingContext(optionsBuilder.Options);
            var user = context.Users
                .Include(e => e.Sessions)
                    .ThenInclude(e => e.Transactions)
                .Include(e => e.Permission)
                .FirstOrDefault(e => e.FirstName == "Andrew");
            UserService userService = new UserService();
            if (user != null) {
                Session session = new Session(user);
                context.Sessions.Add(session);
                context.SaveChanges();
                userService.CurrentUserName = user.UserName;
                userService.CurrentSessionId = session.Id;
                userService.UserPermissionName = user.Permission.Name;
            }
            var partInstances = context.PartInstances.Include(e => e.IndividualAlert).Include(e => e.StockType).ThenInclude(e=>e.CombinedAlert);
            List<Task> tasks = new List<Task>();
            foreach(var instance in partInstances) {
                if (instance.StockType.IsDefault) {
                    //individual alert
                    if (instance.IndividualAlert == null) {
                        IndividualAlert alert = new IndividualAlert();
                        alert.PartInstance = instance;
                        instance.IndividualAlert = alert;
                        context.Add(alert);
                        //context.SaveChanges();
                    }                
                } else {
                    //combined alert
                    if (instance.StockType.CombinedAlert == null) {
                        CombinedAlert alert = new CombinedAlert();
                        alert.StockHolder = instance.StockType;
                        context.Add(alert);
                        //context.SaveChanges();
                    }
                }
            }
            context.SaveChanges();

        }

        public static async Task DeletingAlerts() {
            DbContextOptionsBuilder<ManufacturingContext> optionsBuilder = new DbContextOptionsBuilder<ManufacturingContext>();
            optionsBuilder.UseSqlServer("server=172.27.192.1;database=manufacturing_inventory;user=aelmendorf;password=Drizzle123!;MultipleActiveResultSets=true");
            using var context = new ManufacturingContext(optionsBuilder.Options);
            //var alert =await context.Alerts.OfType<IndividualAlert>().Include(e=>e.UserAlerts).ThenInclude(e=>e.User).Include(e=>e.PartInstance).FirstOrDefaultAsync(e => e.Id == 3);

            var user = context.Users
                .Include(e => e.Sessions)
                    .ThenInclude(e => e.Transactions)
                .Include(e => e.Permission)
                .Include(e=>e.UserAlerts)
                    .ThenInclude(e=>e.Alert)
                .FirstOrDefault(e => e.FirstName == "Andrew");
            UserService userService = new UserService();
            if (user != null) {
                Session session = new Session(user);
                await context.Sessions.AddAsync(session);
                await context.SaveChangesAsync();
                userService.CurrentUserName = user.UserName;
                userService.CurrentSessionId = session.Id;
                userService.UserPermissionName = user.Permission.Name;
                Console.WriteLine("User found and Object created");
                var alert = await context.UserAlerts.FirstOrDefaultAsync(e => e.UserId == user.Id && e.AlertId == 3);

                if (alert != null) {
                    
                    await context.SaveChangesAsync();
                    Console.WriteLine("Done?");
                } else {
                    Console.WriteLine("Could not find alert");
                }
            }
        }

        public static async Task WorkingWithIndividualAlerts() {

            DbContextOptionsBuilder<ManufacturingContext> optionsBuilder = new DbContextOptionsBuilder<ManufacturingContext>();
            optionsBuilder.UseSqlServer("server=172.27.192.1;database=manufacturing_inventory;user=aelmendorf;password=Drizzle123!;MultipleActiveResultSets=true");
            using var context = new ManufacturingContext(optionsBuilder.Options);

            var user = context.Users
                .Include(e => e.Sessions)
                    .ThenInclude(e => e.Transactions)
                .Include(e => e.Permission)
                .FirstOrDefault(e => e.FirstName == "Andrew");
            UserService userService = new UserService();
            if (user != null) {
                Session session = new Session(user);
                await context.Sessions.AddAsync(session);
                await context.SaveChangesAsync();
                userService.CurrentUserName = user.UserName;
                userService.CurrentSessionId = session.Id;
                userService.UserPermissionName = user.Permission.Name;
            }
            var partInstance = await context.PartInstances.Include(e => e.BubblerParameter).Include(e => e.IndividualAlert).FirstOrDefaultAsync(e=>e.Id == 1);
            if (partInstance != null) {
                IndividualAlert alert = new IndividualAlert();
                alert.PartInstance = partInstance;
                UserAlert userAlert = new UserAlert();
                userAlert.Alert = alert;
                userAlert.User = user;
                var added=await context.AddAsync(userAlert);
                if (added != null) {
                    await context.SaveChangesAsync();
                    Console.WriteLine("Should be saved");
                } else {
                    Console.WriteLine("Failed to add Alert");
                }           
            } else {
                Console.WriteLine("PartInstance Not Found");
            }


        }

        public static async Task WorkingWithCombinedAlerts() {
            DbContextOptionsBuilder<ManufacturingContext> optionsBuilder = new DbContextOptionsBuilder<ManufacturingContext>();
            optionsBuilder.UseSqlServer("server=172.27.192.1;database=manufacturing_inventory;user=aelmendorf;password=Drizzle123!;MultipleActiveResultSets=true");
            using var context = new ManufacturingContext(optionsBuilder.Options);

            var user = context.Users
                .Include(e => e.Sessions)
                    .ThenInclude(e => e.Transactions)
                .Include(e => e.Permission)
                .FirstOrDefault(e => e.FirstName == "Andrew");
            UserService userService = new UserService();
            if (user != null) {
                Session session = new Session(user);
                await context.Sessions.AddAsync(session);
                await context.SaveChangesAsync();
                userService.CurrentUserName = user.UserName;
                userService.CurrentSessionId = session.Id;
                userService.UserPermissionName = user.Permission.Name;
            }
            var stockType = await context.Categories.OfType<StockType>().Include(e => e.PartInstances).ThenInclude(e => e.BubblerParameter).FirstOrDefaultAsync(e => e.Id == 16);
            if (stockType != null) {
                CombinedAlert tmaAlert = new CombinedAlert();
                UserAlert userAlert = new UserAlert();
                userAlert.IsEnabled = true;
                tmaAlert.StockHolder = stockType;
                userAlert.Alert = tmaAlert;
                userAlert.User = user;
                var added = await context.AddAsync(userAlert);
                if (added != null) {
                    await context.SaveChangesAsync();
                    Console.WriteLine("Should be saved");
                } else {
                    Console.WriteLine("Could Not Save UserAlert");
                }
            } else {
                Console.WriteLine("StockType is null");
            }

        }

        public static async Task ResetStockTypeQuantity() {
            DbContextOptionsBuilder<ManufacturingContext> optionsBuilder = new DbContextOptionsBuilder<ManufacturingContext>();
            optionsBuilder.UseSqlServer("server=172.27.192.1;database=manufacturing_inventory;user=aelmendorf;password=Drizzle123!;MultipleActiveResultSets=true");
            using var context = new ManufacturingContext(optionsBuilder.Options);
            var tma = await context.Categories.OfType<StockType>().Include(e=>e.PartInstances).ThenInclude(e=>e.BubblerParameter).FirstOrDefaultAsync(e => e.Id == 17);
            foreach(var instance in tma.PartInstances) {
                Console.WriteLine("PartInstance: {0} Quantity", instance.BubblerParameter.Weight);
            }
            tma.Quantity = 0; 
            tma.Quantity += (int)tma.PartInstances.Sum(instance => instance.BubblerParameter.Weight);
            Console.WriteLine();
            Console.WriteLine("New Quantity: {0}", tma.Quantity);
            context.Update(tma);
            await context.SaveChangesAsync();
            Console.WriteLine("Should be done");
        }

        public static async Task TestingCategoryUseCase() {
            DbContextOptionsBuilder<ManufacturingContext> optionsBuilder = new DbContextOptionsBuilder<ManufacturingContext>();
            optionsBuilder.UseSqlServer("server=172.27.192.1;database=manufacturing_inventory;user=aelmendorf;password=Drizzle123!;MultipleActiveResultSets=true");
            using var context = new ManufacturingContext(optionsBuilder.Options);
            var categoryService = new CategoryEdit(context);
            Console.WriteLine("Adding partInstance to category, please wait");
            var category=await categoryService.GetCategory(7);
            Console.WriteLine("Category {0} ", category.Name);
            //var output = await categoryService.AddPartTo(3, category);
            //Console.WriteLine(output.Message);
            Console.WriteLine("Done, Please come again");
        }

        public static void TestingStockTypes() {
            DbContextOptionsBuilder<ManufacturingContext> optionsBuilder = new DbContextOptionsBuilder<ManufacturingContext>();
            optionsBuilder.UseSqlServer("server=172.27.192.1;database=manufacturing_inventory;user=aelmendorf;password=Drizzle123!;MultipleActiveResultSets=true");
            using var context = new ManufacturingContext(optionsBuilder.Options);
            Console.WriteLine("Updating Stock Type");
            var stockType = context.Categories.OfType<StockType>().Include(e => e.PartInstances).ThenInclude(e => e.BubblerParameter).FirstOrDefault(e => e.Name == "TMA");
            if (stockType != null) {
                stockType.Quantity += (int)stockType.PartInstances.Sum(e => e.BubblerParameter.Weight);
                context.Update(stockType);
                Console.WriteLine("New Quantity: {0}", stockType.Quantity);
                Console.WriteLine("Save Changes...");
                context.SaveChanges();
            } else {
                Console.WriteLine("StockType Not Found");
            }

            Console.WriteLine("Exiting");
        }

        public static async Task RunAsync() {
            //Console.WriteLine("Gather Report Data");

            //DbContextOptionsBuilder<ManufacturingContext> optionsBuilder = new DbContextOptionsBuilder<ManufacturingContext>();
            //optionsBuilder.UseSqlServer("server=172.20.4.20;database=manufacturing_inventory_dev;user=aelmendorf;password=Drizzle123!;MultipleActiveResultSets=true");
            //var context = new ManufacturingContext(optionsBuilder.Options);
            //DateTime start = new DateTime(2020, 6, 1);
            //DateTime stop = new DateTime(2020, 6, 30);

            //Console.WriteLine("Start: {0}/{1}/{2} Stop:{3}/{4}/{5}", start.Day, start.Month, start.Year, stop.Day, stop.Month, stop.Year);

            //MonthlyReportInput input = new MonthlyReportInput(start, stop);
            //MonthlySummaryUseCase reporting = new MonthlySummaryUseCase(context);
            //var snapShot = await reporting.Execute(input);

            //if (snapShot.Success) {
            //    Console.WriteLine("Succesfully Generated... Saving report to database");
            //    //await reporting.SaveMonthlySummary();
            //}
            //StringBuilder builder = new StringBuilder();
            //StringBuilder transactionBuffer = new StringBuilder();
            //foreach (var row in snapShot.Snapshot) {
            //    builder.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}", row.PartName,
            //        row.InstanceName, row.StartQuantity, row.StartCost, row.IncomingQuantity, row.IncomingCost,row.TotalOutgoingQuantity, row.TotalOutgoingCost, row.EndQuantity, row.EndCost, row.CurrentQuantity, row.CurrentCost).AppendLine();
            //}
            //System.IO.File.WriteAllText(@"C:\Users\AElmendo\Documents\TestManufacturingReport.txt", builder.ToString());

            //foreach(var transaction in snapShot.TransactionsNeeded) {
            //    transactionBuffer.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t",
            //        transaction.PartInstanceId,transaction.PartInstanceName, transaction.Action, transaction.Quantity, transaction.UnitCost,
            //        transaction.TotalCost, transaction.LocationId, transaction.LocationName).AppendLine();
            //}

            //System.IO.File.WriteAllText(@"C:\Users\AElmendo\Documents\NeededTransactions.txt", transactionBuffer.ToString());

           // var table=ConsoleTable.From<IPartMonthlySummary>(snapShot.Snapshot);
            //Console.WriteLine(table.ToMinimalString());
        }
    }
}
