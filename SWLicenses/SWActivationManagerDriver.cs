using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace SWLicenses
{
    internal class SWActivationManagerDriver
    {
        public static IWebDriver CreateChromeDriver()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("headless");
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;
            IWebDriver driver = new ChromeDriver(service, options);
            driver.Navigate().GoToUrl("https://activate.solidworks.com/manager/");
            return driver;
        }

        public static void Login(IWebDriver driver)
        {
            string[] creds = System.IO.File.ReadAllLines("credentials.txt");
            string username = creds[0];
            string password = creds[1];
            driver.FindElement(By.Id("Login2_txtName")).SendKeys(username);
            driver.FindElement(By.Id("Login2_txtPassword")).SendKeys(password);
            driver.FindElement(By.Id("Login2_cmdLogin")).Click();
        }

        public static void LookupSerial(IWebDriver driver, String serial)
        {
            driver.FindElement(By.Id("txtSearch")).SendKeys(serial);
            driver.FindElement(By.Id("cmdLookup")).Click();
            driver.FindElement(By.LinkText("View")).Click();
        }

        public static void LookupSerials(IWebDriver driver, string[] serials, IProgress<int> progress)
        {
            StreamWriter sw = new StreamWriter("results.csv");
            sw.WriteLine("Product Name,Serial Number,Computer,Maintenance End");
            const int width = 50;
            int total = serials.Length;
            int completed = 0;

            Console.WriteLine("Processing serials:");
            foreach (string serial in serials)
            {
                try
                {
                    LookupSerial(driver, serial);

                    int rows = driver.FindElements(By.XPath(@"//*[@id=""dgReport""]/tbody/tr")).Count;
                    int cols = driver.FindElements(By.XPath(@"//*[@id=""dgReport""]/tbody/tr[2]/td")).Count;

                    String productName = driver.FindElement(By.Id("lblProdName")).Text;
                    String serialNumber = driver.FindElement(By.Id("lblSerialNumber")).Text;
                    String maintEnd = driver.FindElement(By.Id("lblMaintEnd")).Text;

                    License license = new License(productName, serialNumber, maintEnd);

                    int flag = 0;
                    int count = 0;

                    for (int i = 3; i < rows + 1; i++)
                    {
                        flag++;
                        String row = i.ToString();
                        String value = driver.FindElement(By.XPath(@"//*[@id=""dgReport""]/tbody/tr[" + row + "]/td[3]")).Text;
                        if (value.Equals("Y"))
                        {
                            String activatedComp = driver.FindElement(By.XPath(@"//*[@id=""dgReport""]/tbody/tr[" + row + "]/td[2]")).Text;
                            license.ActivatedComputer = activatedComp;
                            license.Write(sw);

                        }
                        else
                        {
                            count++;
                        }

                    }
                    if (flag == count)
                    {
                        license.Write(sw);
                    }
                    driver.FindElement(By.LinkText("Log Out")).Click();
                    Login(driver);
                }
                catch
                {
                    sw.WriteLine("NONE," + serial + ",NONE,NONE");
                    driver.FindElement(By.LinkText("Log Out")).Click();
                    Login(driver);
                }
                completed++;
                int percent = completed * 100 / total;
                progress.Report(percent);

                int completedWidth = percent * width / 100;
                string progressBar = "[" + new string('#', completedWidth) + new string(' ', width - completedWidth) + "]";
                Console.Write($"\r{progressBar} {percent}%");
            }
            sw.Close();
            driver.Close();
            driver.Quit();
        }
    }
}
