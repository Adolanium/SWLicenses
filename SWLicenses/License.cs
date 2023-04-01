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
            sw.WriteLine(ProductName + "," + SerialNumber + "," + ActivatedComputer + "," + MaintenanceEndDate);
        }

        public void Write()
        {
            Console.WriteLine(ProductName + "," + SerialNumber + "," + ActivatedComputer + "," + MaintenanceEndDate);
        }
    }
}
