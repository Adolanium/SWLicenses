using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

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
            string[] credentials = await Configuration.LoadCredentialsAsync();
            driver.FindElement(By.Id("Login2_txtName")).SendKeys(credentials[0]);
            driver.FindElement(By.Id("Login2_txtPassword")).SendKeys(credentials[1]);
            driver.FindElement(By.Id("Login2_cmdLogin")).Click();
        }

        internal static void LookupSerial(IWebDriver driver, string serial)
        {
            driver.FindElement(By.Id("txtSearch")).SendKeys(serial);
            driver.FindElement(By.Id("cmdLookup")).Click();
            driver.FindElement(By.LinkText("View")).Click();
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
                            licenseInfo.SetActivatedComputer(rowData[1].Text);
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
                    await Login(driver);
                }
                catch (NoSuchElementException)
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
                    await Login(driver);
                }

                completed++;
                int percent = completed * 100 / total;
                progress.Report(percent);

                Console.Write($"\r[{GenerateProgressBar(percent, 50)}] {percent}%");
            }
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
