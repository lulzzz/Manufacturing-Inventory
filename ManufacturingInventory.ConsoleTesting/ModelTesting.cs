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
using System.Threading.Tasks.Dataflow;

namespace ManufacturingInventory.ConsoleTesting {


    public class ModelTesting {

        public static void Main(string[] args) {

            AsyncContext.Run(TestingCategoryUseCase);
            //Console.WriteLine("Month: {0}", DateTime.Now.ToString("MMMM"));

        }

        public static async Task TestingCategoryUseCase() {
            DbContextOptionsBuilder<ManufacturingContext> optionsBuilder = new DbContextOptionsBuilder<ManufacturingContext>();
            optionsBuilder.UseSqlServer("server=172.27.192.1;database=manufacturing_inventory;user=aelmendorf;password=Drizzle123!;MultipleActiveResultSets=true");
            using var context = new ManufacturingContext(optionsBuilder.Options);
            var categoryService = new CategoryEdit(context);
            Console.WriteLine("Adding partInstance to category, please wait");
            var category=await categoryService.GetCategory(7);
            Console.WriteLine("Category {0} ", category.Name);
            var output = await categoryService.AddPartTo(3, category);
            Console.WriteLine(output.Message);
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
