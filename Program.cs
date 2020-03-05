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
            var exampleModelList = new List<ExampleModel>
            {
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
                    StringVar = "Funck Me!",
                    DateTimeVar = new DateTime(1994, 12, 05, 19, 10, 00)
                },

            };

            var instancesCounter = 1;

            Console.WriteLine("Listing of variables in Example Model ...");
            foreach (var exampleModel in exampleModelList)
            {
                Console.WriteLine($"Printing {instancesCounter}/{exampleModelList.Count()} instances of ExampleModel ...");
                instancesCounter++;

                foreach (var fieldInfo in exampleModel.GetVariables())
                {
                    Console.WriteLine($"({fieldInfo.PropertyType}) {fieldInfo.Name}: {fieldInfo.GetValue(exampleModel)}");
                }
            }

            Console.WriteLine("Creating XML database ...");
            var database = XMLDatabaseManager.CreateXMLDatabase(new string[] { "ExampleModel" });
            var dbManager = new XMLDatabaseManager(database);

            instancesCounter = 1;

            foreach (var exampleModel in exampleModelList)
            {
                Console.WriteLine($"Adding {instancesCounter}/{exampleModelList.Count()} instance of Example Model into XML database ...");
                instancesCounter++;

                dbManager.AddDataModelInstance(exampleModel);
                dbManager.UpdateDataModelInstance(exampleModel);
            }

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
                    var testModel = dbManager.GetDataModelInstance<ExampleModel>(exampleModel.GetIdentifier());
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
