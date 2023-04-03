using System;
using System.IO;

namespace SWLicenses
{
    internal class LicenseInfo
    {
        public string ProductName { get; set; }
        public string SerialNumber { get; set; }
        public string ActivatedComputer { get; set; }
        public DateTime MaintenanceEndDate { get; set; }

        public LicenseInfo(string productName, string serialNumber, string maintenanceEndDate)
        {
            ProductName = productName;
            SerialNumber = serialNumber;
            ActivatedComputer = "N/A";
            MaintenanceEndDate = DateTime.Parse(maintenanceEndDate);
        }

        public LicenseInfo(string productName, string serialNumber, string activatedComputer, DateTime maintenanceEndDate)
        {
            ProductName = productName;
            SerialNumber = serialNumber;
            ActivatedComputer = activatedComputer;
            MaintenanceEndDate = maintenanceEndDate;
        }

        public void Write(StreamWriter sw)
        {
            sw.WriteLine(ToString());
        }

        public override string ToString()
        {
            return $"\"{ProductName}\",\"{SerialNumber}\",\"{ActivatedComputer}\",\"{MaintenanceEndDate.ToString("yyyy-MM-dd")}\"";
        }
    }
}