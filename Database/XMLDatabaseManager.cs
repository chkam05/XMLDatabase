using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// <summary> Get Instance of Class Model Instance from XML Database by generic "T" type, and ModelBase identifier. </summary>
        /// <typeparam name="T"> Generic "T" type of Class Model, inheriting from ModelBase class and IModel interface. </typeparam>
        /// <param name="identifier"> Identifier of Class Model Instance in database. </param>
        /// <returns> Class Model Instance from database or null if not exist. </returns>
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
        /// <summary> Get Enumerable List of Class Model Instances from XML Database by generic "T" type,
        /// and using Linq querry expression for selecting specific instances. </summary>
        /// <typeparam name="T"> Generic "T" type of Class Model, inheriting from ModelBase class and IModel interface. </typeparam>
        /// <param name="predicate"> Linq querry expression for selecting specific instances. </param>
        /// <returns> Enumerable List of Class Model Instances from XML Database or null if not exists. </returns>
        public IEnumerable<T> GetDataModelInstances<T>(Expression<Func<T, bool>> predicate = null)
        {
            //  Check if generic input Type is a type of Model Class interface.
            if (typeof(IModel).IsAssignableFrom(typeof(T)))
            {
                //  If database contains Node for specified Model Class - add model into database.
                if (HasDataModelInstanceKey(typeof(T).Name))
                {
                    var targetNode = database.Elements(typeof(T).Name);
                    var xmlModels = from xmlModel in targetNode.Elements()
                        select (T) Activator.CreateInstance(typeof(T), new object[] { xmlModel });

                    //  If result from database is not null and contains XML Model.
                    if (xmlModels != null && xmlModels.Count() > 0)
                    {
                        //  If Linq predicate is not null.
                        if (predicate != null)
                        {
                            return xmlModels.AsQueryable().Where<T>(predicate);
                        }

                        //  Return all of xml Models.
                        else
                        {
                            return xmlModels;
                        }
                    }
                }
            }
            
            return null;
        }

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Add Model Class Instance into XML Database. </summary>
        /// <param name="dataModel"> Interface of Model Class Instance. </param>
        /// <returns> True - if success, False - otherwise. </returns>
        public bool AddDataModelInstance(IModel dataModel)
        {
            //  If database contains Node for specified Model Class - add model into database.
            if (HasDataModelInstanceKey(dataModel.GetName()))
            {
                var targetNode = database.Element(dataModel.GetName());
                targetNode.Add(dataModel.AsXML());
                return true;
            }

            return false;
        }

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Update Model Instance in Database by removing old XML Model
        /// and insert new Model Class Instance with updated variables. </summary>
        /// <param name="dataModel"> Interface of updated Model Class Instance. </param>
        /// <returns> True - if success, False - otherwise. </returns>
        public bool UpdateDataModelInstance(IModel dataModel)
        {
            //  If database contains Node for specified Model Class - get Node.
            if (HasDataModelInstanceKey(dataModel.GetName()))
            {
                var targetNode = database.Element(dataModel.GetName());

                //  Get XML Database element based on "id" attribute.
                var xmlModels = targetNode.Elements(dataModel.GetName()).Where(
                    item => item.Attribute("id") != null && item.Attribute("id").Value == dataModel.GetIdentifier());

                //  If result XML Models are not null - remove them and add updated Model.
                if (xmlModels != null && xmlModels.Count() > 0)
                {
                    xmlModels.Remove();
                    targetNode.Add(dataModel.AsXML());
                    return true;
                }
            }

            return false;
        }

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Remove Model Instance from Database by passing Class Model Instance to remove. </summary>
        /// <param name="dataModel"> Class Model Instance to remove from Database. </param>
        /// <returns> True - if success, False - otherwise. </returns>
        public bool RemoveDataModelInstance(IModel dataModel)
        {
            //  If database contains Node for specified Model Class - get Node.
            if (HasDataModelInstanceKey(dataModel.GetName()))
            {
                var targetNode = database.Element(dataModel.GetName());

                //  Get XML Database element based on "id" attribute.
                var xmlModels = targetNode.Elements(dataModel.GetName()).Where(
                    item => item.Attribute("id").Value == dataModel.GetIdentifier());

                //  If result XML Models are not null - remove them and add updated Model.
                if (xmlModels != null && xmlModels.Count() > 0)
                {
                    xmlModels.Remove();
                    return true;
                }
            }

            return false;
        }

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Remove Model Instance from Database by passing it's generic "T" type and identifier. </summary>
        /// <typeparam name="T"> Generic "T" type of Class Model, inheriting from ModelBase class and IModel interface. </typeparam>
        /// <param name="identifier"> Identifier of Class Model Instance in database. </param>
        /// <returns> True - if success, False - otherwise. </returns>
        public bool RemoveDataModelInstance<T>(string identifier)
        {
            //  Check if generic input Type is a type of Model Class interface.
            if (typeof(IModel).IsAssignableFrom(typeof(T)))
            {
                //  If database contains Node for specified Model Class - add model into database.
                if (HasDataModelInstanceKey(typeof(T).Name))
                {
                    var targetNode = database.Elements(typeof(T).Name);

                    //  Get XML Database element based on "id" attribute.
                    var xmlModels = targetNode.Elements(typeof(T).Name).Where(
                        item => item.Attribute("id").Value == identifier);

                    //  If result XML Models are not null - remove them and add updated Model.
                    if (xmlModels != null && xmlModels.Count() > 0)
                    {
                        xmlModels.Remove();
                        return true;
                    }
                }
            }

            return false;
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
