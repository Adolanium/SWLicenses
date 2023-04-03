using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using SWLicenses;

namespace SWLicenses
{
    internal class SolidWorksLicenseManager
    {
        private const string Url = "https://activate.solidworks.com/manager/";

        public static IWebDriver CreateChromeDriver()
        {
            var options = new ChromeOptions();
            options.AddArguments("headless");

            var service = ChromeDriverService.CreateDefaultService();
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;

            var driver = new ChromeDriver(service, options);
            driver.Navigate().GoToUrl(Url);
            return driver;
        }

        public static async void Login(IWebDriver driver)
        {
            string[] credentials = await Configuration.LoadCredentialsAsync();
            driver.FindElement(By.Id("Login2_txtName")).SendKeys(credentials[0]);
            driver.FindElement(By.Id("Login2_txtPassword")).SendKeys(credentials[1]);
            driver.FindElement(By.Id("Login2_cmdLogin")).Click();
        }

        public static void Logout(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var logoutLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Log Out")));
            logoutLink.Click();
        }

        public static void LookupSerial(IWebDriver driver, string serial)
        {
            driver.FindElement(By.Id("txtSearch")).SendKeys(serial);
            driver.FindElement(By.Id("cmdLookup")).Click();
            driver.FindElement(By.LinkText("View")).Click();
        }

        public static async Task LookupSerials(IWebDriver driver, string[] serials, IProgress<int> progress)
        {
            using var licenseInfoWriter = new LicenseInfoWriter("results.csv");

            int total = serials.Length;
            int completed = 0;

            Console.WriteLine("Processing serials:");
            foreach (string serial in serials)
            {
                try
                {
                    LookupSerial(driver, serial);

                    var reportTableRows = LicenseInfoParser.GetReportTableRows(driver);
                    int numRows = reportTableRows.Count;

                    var licenseInfo = LicenseInfoParser.ParseLicenseInfo(driver);

                    bool hasActiveComputer = false;
                    foreach (var row in reportTableRows.Skip(2))
                    {
                        var rowData = row.FindElements(By.TagName("td")).ToList();
                        if (rowData[2].Text.Equals("Y"))
                        {
                            licenseInfo.ActivatedComputer = rowData[1].Text;
                            licenseInfoWriter.Write(licenseInfo);
                            hasActiveComputer = true;
                        }
                    }

                    if (!hasActiveComputer)
                    {
                        licenseInfoWriter.Write(licenseInfo);
                    }

                    try
                    {
                        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                        var logoutLink = wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Log Out")));
                        logoutLink.Click();
                    }
                    catch (WebDriverTimeoutException)
                    {
                        Console.WriteLine("Log Out link not found after waiting.");
                    }
                    Login(driver);
                }
                catch
                {
                    licenseInfoWriter.Write(new LicenseInfo("N/A", serial, "N/A", DateTime.MinValue));
                    try
                    {
                        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                        var logoutLink = wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Log Out")));
                        logoutLink.Click();
                    }
                    catch (WebDriverTimeoutException)
                    {
                        Console.WriteLine("Log Out link not found after waiting.");
                    }
                    Login(driver);
                }

                completed++;
                int percent = completed * 100 / total;
                progress.Report(percent);

                Console.Write($"\r[{GenerateProgressBar(percent, 50)}] {percent}%");
            }
        }

        private static bool TryFindElement(IWebDriver driver, By by, out IWebElement element)
        {
            try
            {
                element = driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                element = null;
                return false;
            }
        }


        private static string GenerateProgressBar(int progress, int width)
        {
            int completedWidth = progress * width / 100;
            return new string('#', completedWidth) + new string(' ', width - completedWidth);
        }
    }
}
