namespace SWLicenses
{
    internal class FilesController
    {
        public static void CheckExistence()
        {
            CheckFileExists("serials.txt", "Error: \"serials.txt\" does not exist.");

            CheckFileExists("credentials.txt", "Error: \"credentials.txt\" does not exist.");

            CheckFileNotEmpty("serials.txt", "Error: \"serials.txt\" is empty. Add serial numbers to it.");

            CheckFileNotEmpty("credentials.txt", "Error: \"credentials.txt\" is empty. Add your credentials to it.");
        }

        private static void CheckFileExists(string filePath, string errorMessage)
        {
            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine(errorMessage);
                // Ask the user if he wants to create the file
                Console.WriteLine("Do you want to create it? (Y/N)");
                string answer = Console.ReadLine();
                if (answer == "Y" || answer == "y")
                {
                    System.IO.File.Create(filePath);
                    Console.WriteLine("File created successfully.");
                }
                else
                {
                    Console.WriteLine("File not created.");
                }
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        private static void CheckFileNotEmpty(string filePath, string errorMessage)
        {
            if (System.IO.File.ReadAllLines(filePath).Length == 0)
            {
                Console.WriteLine(errorMessage);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }
    }
}
