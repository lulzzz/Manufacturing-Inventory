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
using ManufacturingInventory.Application.Boundaries;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Domain.Extensions;
using Nito.AsyncEx;
using System.Reflection;

namespace ManufacturingInventory.ConsoleTesting {
    public class Program {
        public static void Main(string[] args) {
            //var version=FileVersionInfo.GetVersionInfo("").FileVersion;
            //ParserVersion();
            CheckAssembly();
        }

        public static void CheckAssembly() {
            Console.WriteLine("Getting Assembly Name");
            var assemblyName = AssemblyName.GetAssemblyName(@"C:\Program Files\ManufacturingInventory\ManufacturingApplication.dll");
            string fileNameNew = "ManufacturingInventory-3.2.60.7556-Release-x64.msi";
            var customAssemblyVersion = fileNameNew.Split('-')[1];
            var temp = Array.ConvertAll(customAssemblyVersion.Split("."), e => Convert.ToInt32(e));
            var fileVersionValue = CalculateVersion(temp);
            Console.WriteLine("dll Architecture: {0}", assemblyName.ProcessorArchitecture);
            Console.WriteLine("dll Version: {0}", assemblyName.Version.ToString());
            Console.WriteLine("Other: {0}", customAssemblyVersion);
            Console.WriteLine("FileVersionValue: {0}", fileVersionValue);
            FileInfo fileInfo = new FileInfo(@"\\172.20.4.20\ManufacturingInstall\Application\ManufacturingInventory-3.1.67.7556-Release-x64.msi");
            Console.WriteLine("File Full Name: {0}", fileInfo.FullName);
            Process process = new Process();
            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = "msiexec.exe",
                Arguments = string.Format(" /i \"{0}\"", fileInfo.FullName),
                UseShellExecute = false,
            };
            process.StartInfo = psi;
            process.Start();
        }

        public static void ParserVersion() {
            string fileNameOld = "ManufacturingInventory-3.3.62.7555-Release-x64.msi";
            string fileNameNew = "ManufacturingInventory-3.2.60.7556-Release-x64.msi";
            var newValues = ParseVersionCustom(fileNameNew);
            var oldValues = ParseVersionCustom(fileNameOld);
            var newVersion = CalculateVersion(newValues);
            var oldVersion = CalculateVersion(oldValues);
            Console.WriteLine("New Version: {0}", newVersion);
            Console.WriteLine("Old Version: {0}", oldVersion);
            Console.WriteLine("New>Old {0}",newVersion>oldVersion);

        }

        public static int[] ParseVersionCustom(string fileNameWithVersion) {
            var temp = fileNameWithVersion.Split('-');
            return Array.ConvertAll(temp[1].Split('.'),e=>Convert.ToInt32(e));
        }

        public static int CalculateVersion(int[] versionArray) {
            int dateTimeValue = 1;
            int majorMinorValue = 0;
            for (int i = 0; i < versionArray.Length; i++) {
                if (i >= 2) {
                    dateTimeValue *= versionArray[i];
                } else {
                    majorMinorValue += versionArray[i];
                }
            }
            return dateTimeValue + majorMinorValue;
        }
    }
}
