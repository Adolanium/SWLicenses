using OpenQA.Selenium;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace SWLicenses
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.CursorVisible = false;

            await FileValidator.CheckExistenceAsync();

            string[] serials = await Configuration.LoadSerialsAsync();

            new DriverManager().SetUpDriver(new ChromeConfig(), WebDriverManager.Helpers.VersionResolveStrategy.MatchingBrowser);
            IWebDriver driver = SolidWorksLicenseManager.CreateChromeDriver();
            SolidWorksLicenseManager.Login(driver);

            var progress = new Progress<int>(percent =>
            {
                Console.Write($"\rProgress: [{SolidWorksLicenseManager.GenerateProgressBar(percent, 50)}] {percent}%{" ",-10}");
            });

            await SolidWorksLicenseManager.LookupSerials(driver, serials, progress);

            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();

            driver.Close();
            driver.Quit();
        }
    }
}