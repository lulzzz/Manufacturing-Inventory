using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IWshRuntimeLibrary;
using Microsoft.EntityFrameworkCore;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Domain.Security.Interfaces;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Domain.Buisness.Concrete;
using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Interfaces;
using ManufacturingInventory.Application.Boundaries.Checkout;
using ManufacturingInventory.Infrastructure.Model.Providers;
using System.Collections.ObjectModel;
using ManufacturingInventory.Application.Boundaries.PartInstanceDetailsEdit;
using ManufacturingInventory.Application.Boundaries.CheckIn;
using ManufacturingInventory.Application.Boundaries.CategoryBoundaries;
using ManufacturingInventory.Application.Boundaries.ReportingBoundaries;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Domain.Extensions;
using Nito.AsyncEx;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace ManufacturingInventory.ConsoleTesting {


    public class ModelTesting {

        public static void Main(string[] args) {

            //AsyncContext.Run(RunAsync);
            Console.WriteLine("Month: {0}", DateTime.Now.ToString("MMMM"));
        }

        public static async Task RunAsync() {
            Console.WriteLine("Gather Report Data");

            DbContextOptionsBuilder<ManufacturingContext> optionsBuilder = new DbContextOptionsBuilder<ManufacturingContext>();
            optionsBuilder.UseSqlServer("server=172.20.4.20;database=manufacturing_inventory_dev;user=aelmendorf;password=Drizzle123!;MultipleActiveResultSets=true");
            var context = new ManufacturingContext(optionsBuilder.Options);
            DateTime start = new DateTime(2020, 6, 1);
            DateTime stop = new DateTime(2020, 6, 30);

            Console.WriteLine("Start: {0}/{1}/{2} Stop:{3}/{4}/{5}", start.Day, start.Month, start.Year, stop.Day, stop.Month, stop.Year);

            MonthlySummaryInput input = new MonthlySummaryInput(start, stop);
            MonthlySummaryUseCase reporting = new MonthlySummaryUseCase(context);
            var snapShot = await reporting.Execute(input);

            if (snapShot.Success) {
                Console.WriteLine("Succesfully Generated... Saving report to database");
                await reporting.SaveMonthlySummary();
            }
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

            var table=ConsoleTable.From<IPartMonthlySummary>(snapShot.Snapshot);
            Console.WriteLine(table.ToMinimalString());
        }

        //public static async Task RunAndGenerateAsync() {
        //    Console.WriteLine("Gathering Transaction List");

        //    DbContextOptionsBuilder<ManufacturingContext> optionsBuilder = new DbContextOptionsBuilder<ManufacturingContext>();
        //    optionsBuilder.UseSqlServer("server=172.20.4.20;database=manufacturing_inventory;user=aelmendorf;password=Drizzle123!;MultipleActiveResultSets=true");
        //    var context = new ManufacturingContext(optionsBuilder.Options);
        //    DateTime start = new DateTime(2020, 1, 1);
        //    DateTime stop = DateTime.Now;

        //    Console.WriteLine("Start: {0}/{1}/{2} Stop:{3}/{4}/{5}", start.Day, start.Month, start.Year, stop.Day, stop.Month, stop.Year);

        //    MonthlySummaryInput input = new MonthlySummaryInput(start, stop);
        //    MonthlySummaryUseCase reporting = new MonthlySummaryUseCase(context);
        //    var snapShot = await reporting.Execute(input);
        //    await GenerateMissingTransactions(snapShot.TransactionsNeeded);

        //}

        //public static async Task GenerateMissingTransactions(IEnumerable<TransactionInfo> transactions) {
        //    DbContextOptionsBuilder<ManufacturingContext> optionsBuilder = new DbContextOptionsBuilder<ManufacturingContext>();
        //    optionsBuilder.UseSqlServer("server=172.20.4.20;database=manufacturing_inventory;user=aelmendorf;password=Drizzle123!;MultipleActiveResultSets=true");
        //    var context = new ManufacturingContext(optionsBuilder.Options);
        //    IRepository<PartInstance> _partInstanceRepository=new PartInstanceRepository(context);
        //    IRepository<Transaction> _transactionRepository=new TransactionRepository(context);
        //    IUnitOfWork unitOfWork = new UnitOfWork(context);
        //    var user = context.Users
        //        .Include(e => e.Sessions)
        //            .ThenInclude(e => e.Transactions)
        //        .Include(e => e.Permission)
        //        .FirstOrDefault(e => e.FirstName == "Andrew");
        //    UserService userService = new UserService();
        //    if (user != null) {
        //        Session session = new Session(user);
        //        context.Sessions.Add(session);
        //        context.SaveChanges();
        //        userService.CurrentUserName = user.UserName;
        //        userService.CurrentSessionId = session.Id;
        //        userService.UserPermissionName = user.Permission.Name;
        //    }

        //    foreach (var tran in transactions) {
        //        var partInstance =await _partInstanceRepository.GetEntityAsync(e => e.Id == tran.PartInstanceId);
        //        if (partInstance != null) {
        //            Transaction transaction = new Transaction(partInstance, tran.Action, 0, 0, partInstance.CurrentLocation, new DateTime(2020, 1, 1, 0, 0, 0));
        //            transaction.Quantity = (int)tran.Quantity;
        //            transaction.UnitCost = tran.UnitCost;
        //            transaction.TotalCost = tran.TotalCost;
        //            if (userService.CurrentSessionId.HasValue) {
        //                transaction.SessionId = userService.CurrentSessionId.Value;
        //            }
        //            var updatedInstance = await _partInstanceRepository.UpdateAsync(partInstance);
        //            var transactionAdded = await _transactionRepository.AddAsync(transaction);
        //            if(updatedInstance!=null && transactionAdded != null) {
        //                var count = await unitOfWork.Save();
        //                if (count > 0) {
        //                    Console.WriteLine("**Success** save for PartIntstance {0}", partInstance.Name);
        //                } else {
        //                    await unitOfWork.Undo();
        //                    Console.WriteLine("**Failed** save for PartInstance {0} transaction",partInstance.Name);
        //                }
        //            } else {
        //                await unitOfWork.Undo();
        //                Console.WriteLine("**Failed** update for PartInstance {0} transaction", partInstance.Name);
        //            }

        //        }
        //    }
        //}

    }
}
