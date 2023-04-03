using System;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace SWLicenses
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FilesController.CheckExistence();

            string[] serials = File.ReadAllLines("serials.txt")
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToArray();

            new DriverManager().SetUpDriver(new ChromeConfig(), WebDriverManager.Helpers.VersionResolveStrategy.MatchingBrowser);
            IWebDriver driver = SwActivationManagerDriver.CreateChromeDriver();
            SwActivationManagerDriver.Login(driver);

            var progress = new Progress<int>(percent =>
            {
                Console.WriteLine($" Progress: {percent}%");
            });

            SwActivationManagerDriver.LookupSerials(driver, serials, progress).GetAwaiter().GetResult();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}