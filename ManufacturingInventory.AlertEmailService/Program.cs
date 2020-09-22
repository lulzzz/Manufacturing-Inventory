using System;
using ManufacturingInventory.AlertEmailService.Services;
using Nito.AsyncEx;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text;
//using Microsoft.As

namespace ManufacturingInventory.AlertEmailService {
    public class Program {
        //public IConfiguration Configuration { get; private set; }
        static void Main(string[] args) {
            //const string loggerTemplate = @"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u4}]<{ThreadId}> [{SourceContext:l}] {Message:lj}{NewLine}{Exception}";
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var logDirPath = Path.Combine(baseDir, "AppLogs");
            if (!Directory.Exists(logDirPath)) {
                Directory.CreateDirectory(logDirPath);
            }
            //var logfile = Path.Combine(baseDir, "App_Data", "logs", "log.txt");
            var logfile = Path.Combine(logDirPath, "log.txt");

            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => {
                    builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Warning)
                    .AddFile(logfile)
                    .AddEventLog(new EventLogSettings() {
                        SourceName = "AlertEmailService",
                        LogName = "AlertEmailServiceLog",
                        Filter = (x, y) => y >= LogLevel.Warning
                    });
                })
                .AddTransient<IAlertService, AlertService>()
                .AddTransient<IEmailer, Emailer>()
                .BuildServiceProvider();


            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            StringBuilder builder =new StringBuilder();
            builder.AppendLine("====================================================================");
            builder.AppendLine($"Application Starts. Version: {System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version}");
            builder.AppendLine($"Application Directory: {AppDomain.CurrentDomain.BaseDirectory}");
            builder.AppendLine("====================================================================\r\n");


            logger.LogWarning(builder.ToString());

            var alertService=serviceProvider.GetService<IAlertService>();
            AsyncContext.Run(alertService.Run);
            //Console.WriteLine("Running Service");
            //AlertService alertService = new AlertService();
            //AsyncContext.Run(alertService.Run);
        }
    }
}
