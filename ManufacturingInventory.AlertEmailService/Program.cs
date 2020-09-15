using System;
using ManufacturingInventory.AlertEmailService.Services;
using Nito.AsyncEx;

namespace ManufacturingInventory.AlertEmailService {
    public class Program {
        static void Main(string[] args) {
            Console.WriteLine("Running Service");
            AlertService alertService = new AlertService();
            AsyncContext.Run(alertService.Run);
            Console.WriteLine("Should be done");
            Console.ReadKey();
        }
    }
}
