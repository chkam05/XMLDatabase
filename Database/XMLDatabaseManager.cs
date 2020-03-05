using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;
using XMLDatabase.Data.Base;

namespace XMLDatabase.Database
{
    public class XMLDatabaseManager
    {
        private XElement database;


        #region CLASS FUNCTIONS

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> XMLDatabaseManager Contructior with loaded XML Database. </summary>
        /// <param name="xmlDatabase"> Loaded XML Database. </param>
        public XMLDatabaseManager(XElement xmlDatabase)
        {
            database = xmlDatabase;
        }

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Constructor for new empty XML Database. </summary>
        /// <param name="dataModelsNames"> Array of Data Models Classes Names. </param>
        /// <returns> New XML Database. </returns>
        public static XElement CreateXMLDatabase( string[] dataModelsNames = null )
        {
            //  Create empty XML Database Root element.
            var xmlDatabase = new XElement("XMLDatabase");

            //  Fill XML Database with Names of Data Models from list if exsit.
            if (dataModelsNames != null)
            {
                foreach (var dataModelName in dataModelsNames)
                {
                    var newNode = new XElement(dataModelName);
                    xmlDatabase.Add(newNode);
                }
            }

            return xmlDatabase;
        }

        #endregion CLASS FUNCTIONS


        #region DATABASE MANAGEMENT FUNCTIONS

        //  ----------------------------------------------------------------------------------------------------
        public T GetDataModelInstance<T>(string identifier)
        {
            //  Check if generic input Type is a type of Model Class interface.
            if (typeof(IModel).IsAssignableFrom(typeof(T)))
            {
                //  If database contains Node for specified Model Class - add model into database.
                if (HasDataModelInstanceKey(typeof(T).Name))
                {
                    var targetNode = database.Elements(typeof(T).Name);
                    var xmlModels = targetNode.Elements(typeof(T).Name).Where(
                        item => item.Attribute("id").Value == identifier);

                    //  If result from database is not null and contains XML Model.
                    if (xmlModels != null && xmlModels.Count() > 0)
                    {
                        T result = (T) Activator.CreateInstance(typeof(T), new object[] { xmlModels.FirstOrDefault() });
                        return result;
                    }
                }
            }

            return default(T);
        }

        //  ----------------------------------------------------------------------------------------------------
        public List<T> GetDataModelInstances<T>(Expression<Func<T, bool>> predicate)
        {
            return null;
        }

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Add Model Class Instance into XML Database. </summary>
        /// <param name="dataModel"> Interface of Model Class Instance. </param>
        public void AddDataModelInstance(IModel dataModel)
        {
            //  If database contains Node for specified Model Class - add model into database.
            if (HasDataModelInstanceKey(dataModel.GetName()))
            {
                var targetNode = database.Element(dataModel.GetName());
                targetNode.Add(dataModel.AsXML());
            }
        }

        //  ----------------------------------------------------------------------------------------------------
        public void UpdateDataModelInstance(IModel dataModel)
        {
            //  If database contains Node for specified Model Class - get Node.
            if (HasDataModelInstanceKey(dataModel.GetName()))
            {
                var targetNode = database.Element(dataModel.GetName());

                //  Get XML Database element based on "id" attribute.
                var xmlModels = database.Elements(dataModel.GetName()).Where(
                    item => item.Attribute("id").Value == dataModel.GetIdentifier());

                Console.WriteLine(targetNode);
                Console.WriteLine(xmlModels);
            }
        }

        //  ----------------------------------------------------------------------------------------------------
        public void RemoveDataModelInstance(IModel dataModel)
        {
            //
        }

        #endregion DATABASE MANAGEMENT FUNCTIONS


        #region DATABASE INTEGRITY FUNCTIONS

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Check if Root element of XML Database contains List of dataModelInstances. </summary>
        /// <param name="dataModel"> Interface of Data Model Instance. </param>
        /// <returns> True - if contains, False - otherwise. </returns>
        public bool HasDataModelInstanceKey(string dataModelKey)
        {
            return database.Elements().Where(item => item.Name == dataModelKey).Count() > 0;
        }

        //  ----------------------------------------------------------------------------------------------------
        public bool HasDataModelInstance(IModel dataModel)
        {
            //  Get instance of specified Node that contains Model Class Instances.
            var targetNode = database.Element(dataModel.GetName());

            if (targetNode != null)
            {
                //  Check if targetNode contains Model Class Instance where "id" is equal to dataModel "id".
                return targetNode.Elements().Where(
                    item => item.Attribute("id").Value == dataModel.GetIdentifier()).Count() > 0;
            }

            return false;
        }

        #endregion DATABASE INTEGRITY FUNCTIONS

    }
}
