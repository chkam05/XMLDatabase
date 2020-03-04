using System;
using System.Collections.Generic;
using System.Linq;
using XMLDatabase.Data.Models;
using XMLDatabase.Database;

namespace XMLDatabase
{
    public class Program
    {
        private static string filePath = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\XmlTestFile.xml";

        //  ----------------------------------------------------------------------------------------------------
        static void Main(string[] args)
        {
            SaveTest();
            LoadTest();
        }

        //  ----------------------------------------------------------------------------------------------------
        private static void SaveTest()
        {
            Console.WriteLine("Creating Example Model ...");
            var exampleModel = new ExampleModel
            {
                BoolVar = false,
                IntVar = 12345,
                DoubleVar = 6.54321,
                StringVar = "Hello World",
                DateTimeVar = DateTime.UtcNow
            };

            Console.WriteLine("Listing of variables in Example Model ...");
            foreach (var fieldInfo in exampleModel.GetVariables())
            {
                Console.WriteLine($"({fieldInfo.PropertyType}) {fieldInfo.Name}: {fieldInfo.GetValue(exampleModel)}");
            }

            Console.WriteLine("Creating XML database ...");
            var database = XMLDatabaseManager.CreateXMLDatabase(new string[] { "ExampleModel" });
            var dbManager = new XMLDatabaseManager(database);

            Console.WriteLine("Adding Example Model Into XML database ...");
            dbManager.AddDataModelInstance(exampleModel);

            Console.WriteLine("Saving database to file ...");
            XMLDatabaseFileManager.SaveXMLDatabaseToFile(database, filePath);
        }

        //  ----------------------------------------------------------------------------------------------------
        private static void LoadTest()
        {
            Console.WriteLine("Loading database from file ...");
            var database = XMLDatabaseFileManager.LoadXMLDatabaseFromFile(filePath);
            var dbManager = new XMLDatabaseManager(database);

            var exampleModelList = new List<ExampleModel>();
            var instancesCounter = 0;

            Console.WriteLine("Checking if database contains ExampleModel Key ...");
            if (dbManager.HasDataModelInstanceKey("ExampleModel"))
            {
                var sourceNode = database.Element("ExampleModel");
                var xmlModels = sourceNode.Elements();
                instancesCounter = 1;

                foreach (var xmlModel in sourceNode.Elements())
                {
                    Console.WriteLine($"Getting {instancesCounter}/{xmlModels.Count()} instances of ExampleModel ...");
                    instancesCounter++;

                    var exampleModel = new ExampleModel(xmlModel);
                    exampleModelList.Add(exampleModel);
                }
            }

            instancesCounter = 1;

            foreach (var exampleModel in exampleModelList)
            {
                Console.WriteLine($"Printing {instancesCounter}/{exampleModelList.Count()} instances of ExampleModel ...");
                instancesCounter++;

                foreach (var fieldInfo in exampleModel.GetVariables())
                {
                    Console.WriteLine($"({fieldInfo.PropertyType}) {fieldInfo.Name}: {fieldInfo.GetValue(exampleModel)}");
                }
            }
        }
    }
}
