using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;

namespace ManufacturingInventory.ConsoleTesting {
    public class InventoryTransfer {
        public static Dictionary<int, string> HeaderIndex = new Dictionary<int, string> {
            {0,"Name"},
            {1,"Quantity"},
            {2,"MinQuantity"},
            {3,"SafeQuantity"},
            {4,"SerialNumber"},
            {5,"CostReported"},
            {6,"IsBubbler"},
            {7,"IsReusable"},
            {8,"PartId"},
            {9,"StockTypeId"},
            {10,"LocationId"},
            {11,"UsageId"}
        };


        public static void Main(string[] args) {
            ImportPartInstances();
            //OutputClassProperties();
        }

        public static void OutputClassProperties() {
            StringBuilder builder = new StringBuilder();
            var properties = typeof(Price).GetProperties();
            StreamWriter streamWriter = new StreamWriter(@"C:\InventoryTransfer\PartProperties.txt", false);
            foreach (var property in properties) {
                streamWriter.WriteLine(property.Name);
            }
            streamWriter.Close();
            Console.WriteLine("Should be done");
            Console.ReadKey();
        }

        public static void ImportPartInstances() {
            var context = new ManufacturingContext();
            var lines = File.ReadAllLines(@"C:\InventoryTransfer\PartInstances.txt");
            foreach (var line in lines) {
                var rows = line.Split('\t');
                PartInstance instance = new PartInstance();
                var properties = typeof(PartInstance).GetProperties();
                foreach (var keyValue in HeaderIndex) {
                    if (keyValue.Key == 0 || keyValue.Key == 4) {
                        instance.GetType().GetProperty(keyValue.Value).SetValue(instance, rows[keyValue.Key]);
                    } else if ((keyValue.Key>=1 && keyValue.Key<=3) || (keyValue.Key >= 8 && keyValue.Key <= 11)) {
                        instance.GetType().GetProperty(keyValue.Value).SetValue(instance,Convert.ToInt32(rows[keyValue.Key]));
                    } else if(keyValue.Key>=5 && keyValue.Key <= 7) {
                        instance.GetType().GetProperty(keyValue.Value).SetValue(instance,false);
                    }
                }
                context.Add(instance);
            }
            context.SaveChanges();
            Console.WriteLine("Maybe???");
            Console.ReadKey();
        }
    }
}
