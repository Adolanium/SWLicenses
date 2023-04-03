using System;
using System.IO;

namespace SWLicenses
{
    internal class License
    {
        public string ProductName { get; set; }
        public string SerialNumber { get; set; }
        public string ActivatedComputer { get; set; }
        public DateTime MaintenanceEndDate { get; set; }

        public License(string productName, string serialNumber, string maintenanceEndDate)
        {
            ProductName = productName;
            SerialNumber = serialNumber;
            ActivatedComputer = "N/A";
            MaintenanceEndDate = DateTime.Parse(maintenanceEndDate);
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