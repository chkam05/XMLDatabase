using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace XMLDatabase.Database
{
    public class XMLDatabaseFileManager
    {
        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Save XML database to XML file. </summary>
        /// <param name="xmlDatabase"> XML database. </param>
        /// <param name="filePath"> Path to new or existsing XML file. </param>
        /// <returns> True - Save complete, False - otherwise. </returns>
        public static void SaveXMLDatabaseToFile(XElement xmlDatabase, string filePath)
        {
            //  Check if file exists and notify user about overwirte.
            if (File.Exists(filePath))
            {
                //
            }

            //  Open file as stream and write database into.
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                xmlDatabase.Save(writer);
                writer.Close();
            }
        }

        //  ----------------------------------------------------------------------------------------------------
        public static XElement LoadXMLDatabaseFromFile(string filePath)
        {
            //  Check if database XML file exists, and raise exception if not.
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("");
            }

            //  Open file as stream and load its content into XML database.
            using (StreamReader reader = new StreamReader(filePath))
            {
                var result = XElement.Load(reader);
                reader.Close();
                return result;
            }
        }
    }
}
