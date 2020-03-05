using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XMLDatabase.Data.Models;
using XMLDatabase.Database;

namespace XMLDatabase
{
    public class TestClass
    {
        private static string filePath = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\XmlTestFile.xml";

        private static List<ExampleModel> exampleInstances = new List<ExampleModel> {
            new ExampleModel
            {
                BoolVar = false,
                IntVar = 12345,
                DoubleVar = 6.54321,
                StringVar = "Hello World",
                DateTimeVar = DateTime.UtcNow
            },
            new ExampleModel
            {
                BoolVar = true,
                IntVar = 67890,
                DoubleVar = 0.9876,
                StringVar = "Hello XML",
                DateTimeVar = new DateTime(1994, 12, 05, 19, 10, 00)
            },
        };

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Run Save and Load Test functions too check if XMLDatabase works correctly. </summary>
        public static void MakeTest()
        {
            SaveTest();
            LoadTest();
        }

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Test saving XMLDatabase functions for check if it works correctly. </summary>
        private static void SaveTest()
        {
            Console.WriteLine("Creating Example Model ...");

            var instancesCounter = 1;

            Console.WriteLine("Listing of variables in Example Model ...");
            foreach (var exampleModel in exampleInstances)
            {
                Console.WriteLine($"Printing {instancesCounter}/{exampleInstances.Count()} instances of ExampleModel ...");
                instancesCounter++;

                Console.WriteLine("----------------------------------------------------------------");
                foreach (var fieldInfo in exampleModel.GetVariables())
                {
                    Console.WriteLine($"({fieldInfo.PropertyType}) {fieldInfo.Name}: {fieldInfo.GetValue(exampleModel)}");
                }
                Console.WriteLine("----------------------------------------------------------------");
            }

            Console.WriteLine("Creating XML database ...");
            var database = XMLDatabaseManager.CreateXMLDatabase(new string[] { "ExampleModel" });
            var dbManager = new XMLDatabaseManager(database);

            instancesCounter = 1;

            foreach (var exampleModel in exampleInstances)
            {
                Console.WriteLine($"Adding {instancesCounter}/{exampleInstances.Count()} instance of Example Model into XML database ...");
                instancesCounter++;

                dbManager.AddDataModelInstance(exampleModel);
            }

            Console.WriteLine("Saving database to file ...");
            XMLDatabaseFileManager.SaveXMLDatabaseToFile(database, filePath);
        }

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Test loading XMLDatabase functions for check if it works correctly. </summary>
        private static void LoadTest()
        {
            Console.WriteLine("Loading database from file ...");
            var database = XMLDatabaseFileManager.LoadXMLDatabaseFromFile(filePath);
            var dbManager = new XMLDatabaseManager(database);

            var exampleModelList = new List<ExampleModel>();

            Console.WriteLine("Checking if database contains ExampleModel Key ...");
            if (dbManager.HasDataModelInstanceKey("ExampleModel"))
            {
                Console.WriteLine($"Getting instances of Example Model from database ...");
                exampleModelList = dbManager.GetDataModelInstances<ExampleModel>().ToList();
            }

            var instancesCounter = 1;
            foreach (var exampleModel in exampleModelList)
            {
                Console.WriteLine($"Printing {instancesCounter}/{exampleModelList.Count()} instances of ExampleModel ...");
                instancesCounter++;

                Console.WriteLine("----------------------------------------------------------------");
                foreach (var fieldInfo in exampleModel.GetVariables())
                {
                    Console.WriteLine($"({fieldInfo.PropertyType}) {fieldInfo.Name}: {fieldInfo.GetValue(exampleModel)}");
                }
                Console.WriteLine("----------------------------------------------------------------");
            }
        }
    }
}
