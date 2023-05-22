using OpenQA.Selenium;

namespace SWLicenses
{
    internal class LicenseInfoParser
    {
        internal static LicenseInfo ParseLicenseInfo(IWebDriver driver)
        {
            var productName = driver.FindElement(By.Id("lblProdName")).Text;
            var version = driver.FindElement(By.Id("lblVersion")).Text;
            var serialNumber = driver.FindElement(By.Id("lblSerialNumber")).Text;
            var maintEnd = driver.FindElement(By.Id("lblMaintEnd")).Text;

            return new LicenseInfo(productName, serialNumber, maintEnd, version);
        }

        internal static List<IWebElement> GetReportTableRows(IWebDriver driver)
        {
            return driver.FindElements(By.XPath(@"//*[@id=""dgReport""]/tbody/tr")).ToList();
        }
    }
}