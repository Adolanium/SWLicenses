using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.ObjectModel;

namespace SWLicenses
{
    internal class SolidWorksLicenseManager
    {
        private const string Url = "https://activate.solidworks.com/manager/";

        internal static IWebDriver CreateChromeDriver()
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

        internal static async Task Login(IWebDriver driver)
        {
            string[] credentials = await Configuration.LoadCredentialsAsync().ConfigureAwait(false);
            driver.FindElement(By.Id("Login2_txtName")).SendKeys(credentials[0]);
            driver.FindElement(By.Id("Login2_txtPassword")).SendKeys(credentials[1]);
            driver.FindElement(By.Id("Login2_cmdLogin")).Click();

            // Wait for the search input to be visible after logging in
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("txtSearch")));
        }

        internal static void LogOut(IWebDriver driver)
        {
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
        }

        internal static bool LookupSerial(IWebDriver driver, string serial)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Wait for the search input to be visible
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("txtSearch")));
            driver.FindElement(By.Id("txtSearch")).SendKeys(serial);

            // Wait for the lookup button to be clickable
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("cmdLookup")));
            driver.FindElement(By.Id("cmdLookup")).Click();

            // Check if the no match message is present
            if (IsElementPresent(driver, By.Id("lblNoMatch")))
            {
                return false;
            }

            // Wait for the view link to be clickable
            wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("View")));
            driver.FindElement(By.LinkText("View")).Click();

            return true;
        }

        private static bool IsElementPresent(IWebDriver driver, By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        internal static async Task LookupSerials(IWebDriver driver, string[] serials, IProgress<int> progress)
        {
            using var licenseInfoWriter = new LicenseInfoWriter("results.csv");

            int total = serials.Length;
            int completed = 0;

            Console.WriteLine("Processing serials:");
            foreach (string serial in serials)
            {
                try
                {
                    ProcessSerial(driver, serial, licenseInfoWriter);
                }
                catch (NoSuchElementException)
                {
                    HandleNoSuchElementException(serial, licenseInfoWriter);
                }
                finally
                {
                    LogOut(driver);
                    await Login(driver).ConfigureAwait(false);
                }

                completed++;
                int percent = completed * 100 / total;
                progress.Report(percent);

                Console.Write($"\r[{GenerateProgressBar(percent, 50)}] {percent}%");
            }
        }

        internal static void ProcessSerial(IWebDriver driver, string serial, LicenseInfoWriter licenseInfoWriter)
        {
            if (LookupSerial(driver, serial))
            {
                var reportTableRows = LicenseInfoParser.GetReportTableRows(driver);
                var licenseInfo = LicenseInfoParser.ParseLicenseInfo(driver);

                bool hasActiveComputer = HandleLicenseActivation(reportTableRows, licenseInfo, licenseInfoWriter);

                if (!hasActiveComputer)
                {
                    WriteInactiveLicense(licenseInfo, licenseInfoWriter);
                }
            }
        }

        internal static bool HandleLicenseActivation(List<IWebElement> reportTableRows, LicenseInfo licenseInfo, LicenseInfoWriter licenseInfoWriter)
        {
            bool hasActiveComputer = false;
            foreach (var row in reportTableRows.Skip(2))
            {
                var columnData = row.FindElements(By.TagName("td")).ToList();
                if (columnData[2].Text.Equals("Y"))
                {
                    licenseInfo.SetActivatedComputer(columnData[1].Text);
                    licenseInfoWriter.Write(licenseInfo);
                    hasActiveComputer = true;
                }
            }
            return hasActiveComputer;
        }

        internal static void WriteInactiveLicense(LicenseInfo licenseInfo, LicenseInfoWriter licenseInfoWriter)
        {
            licenseInfoWriter.Write(licenseInfo);
        }

        internal static void HandleNoSuchElementException(string serial, LicenseInfoWriter licenseInfoWriter)
        {
            licenseInfoWriter.Write(new LicenseInfo("N/A", serial, "N/A", DateTime.MinValue, "N/A"));
        }


        internal static string GenerateProgressBar(int progress, int width)
        {
            int completedWidth = progress * width / 100;
            int remainingWidth = width - completedWidth;

            string completedBar = new string('█', completedWidth);
            string remainingBar = new string('░', remainingWidth);

            return $"{completedBar}{remainingBar}";
        }
    }
}
