using OpenQA.Selenium;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
using WebDriverManager;

namespace SWLicenses
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string[] serials = System.IO.File.ReadAllLines("serials.txt");
            serials = Array.FindAll(serials, (s) => !string.IsNullOrWhiteSpace(s));
            serials = serials.Distinct().ToArray();
            new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
            IWebDriver driver = SWActivationManagerDriver.CreateChromeDriver();
            SWActivationManagerDriver.Login(driver);
            var progress = new Progress<int>(percent =>
            {
                Console.WriteLine($" Progress: {percent}%");
            });
            SWActivationManagerDriver.LookupSerials(driver, serials, progress);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}