﻿namespace SWLicenses
{
    internal class FilesController
    {
        public static void CheckExistence()
        {
            CheckFileExistsAndNotEmpty("serials.txt", "Error: \"serials.txt\" does not exist.", "Error: \"serials.txt\" is empty. Add serial numbers to it.");

            CheckFileExistsAndNotEmpty("credentials.txt", "Error: \"credentials.txt\" does not exist.", "Error: \"credentials.txt\" is empty. Add your credentials to it.");
        }

        private static void CheckFileExistsAndNotEmpty(string filePath, string notExistMessage, string emptyMessage)
        {
            CheckFileExists(filePath, notExistMessage);
            CheckFileNotEmpty(filePath, emptyMessage);
        }

        private static void CheckFileExists(string filePath, string errorMessage)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine(errorMessage);
                // Ask the user if they want to create the file
                Console.WriteLine("Do you want to create it? (Y/N)");
                string answer = Console.ReadLine();
                if (answer.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    File.Create(filePath).Dispose();
                    Console.WriteLine("File created successfully.");
                }
                else
                {
                    Console.WriteLine("File not created.");
                }
                ExitWithMessage("Press any key to exit...");
            }
        }

        private static void CheckFileNotEmpty(string filePath, string errorMessage)
        {
            if (File.ReadAllLines(filePath).Length == 0)
            {
                ExitWithMessage(errorMessage + "\nPress any key to exit...");
            }
        }

        private static void ExitWithMessage(string message)
        {
            Console.WriteLine(message);
            Console.ReadKey();
            Environment.Exit(1);
        }
    }
}