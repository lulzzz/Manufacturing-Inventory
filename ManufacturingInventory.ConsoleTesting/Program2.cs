using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Common.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;


namespace ManufacturingInventory.ConsoleTesting {
    public class Program2 {
        public static void Main(string[] args) {
                var serviceProvider = new ServiceCollection()
                    .AddLogging()
                    .AddDbContext<ManufacturingContext>()
                    .BuildServiceProvider();

                var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program2>();

                Console.WriteLine("Starting Application");

                var context = serviceProvider.GetService<ManufacturingContext>();
            var parts = context.Parts;
                foreach (var part in parts) {
                    foreach (var instance in part.PartInstances) {
                        Console.WriteLine("Instance: {0}", instance.Name);
                    }
                }

            Console.WriteLine("Done");
            Console.ReadKey();
        }

    }
}
