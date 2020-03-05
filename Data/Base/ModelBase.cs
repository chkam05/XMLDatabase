using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace XMLDatabase.Data.Base
{
    public abstract class ModelBase : IModel
    {
        private string  Id  { get; set; }


        #region CLASS MANAGEMENT FUNCTIONS

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Get array of FiledInfos from Model Class Instance. </summary>
        /// <returns> Array of Class FieldInfos. </returns>
        public PropertyInfo[] GetVariables()
        {
            //  Configure BindingFlags for specified Field/Properties Types.
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            //  Get array of Class Instance Properties.
            var properties = this.GetType().GetProperties(bindingFlags);

            return properties;
        }

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Set property value of Model Class Instance. </summary>
        /// <typeparam name="T"> Type of property/variable. </typeparam>
        /// <param name="name"> Name of property/variable. </param>
        /// <param name="value"> Value to set. </param>
        public void SetVariable<T>(string name, T value)
        {
            //  Get Class Instance property with specific name.
            var property = this.GetType().GetProperty(name);

            //  Change value if property exists and is right T type.
            if (property != null && property.PropertyType == typeof(T))
            {
                property.SetValue(this, value);
            }
        }

        //  ----------------------------------------------------------------------------------------------------
        public void TrySetVariable(string name, object value)
        {
            //  Get Class Instance property with sepcific name.
            var property = this.GetType().GetProperty(name);

            //  Convert value if property exists and assing to value.
            if (property != null)
            {
                property.SetValue(
                    this, Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture));
            }
        }

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Get Name of Model Class. </summary>
        /// <returns> Name of Model Class. </returns>
        public string GetName()
        {
            return this.GetType().Name;
        }

        #endregion CLASS MANAGEMENT FUNCTIONS


        #region CLASS FUNCTIONS

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Model Class Instance Constructor with Identifier creation. </summary>
        public ModelBase()
        {
            //  Generate New Identifier.
            ForceNewIdentifier();
        }

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Model Class Instance Constructor with data injection from XML Model. </summary>
        /// <param name="xmlModel"> XML Model from XML Database. </param>
        public ModelBase(XElement xmlModel)
        {
            //  Get identifier from XML Model.
            var identifier = xmlModel.Attribute("id");

            //  Raise error if index was not loaded from XML Model.
            if (identifier == null)
            {
                throw new ArgumentException("Identifier of loaded Model cannot be empty.");
            }

            this.Id = identifier.Value;

            //  Get rest of variables from XML Model and load it into Class Model.
            foreach (var xmlVariable in xmlModel.Elements())
            {
                var name = xmlVariable.Name.ToString();
                var typeAttrib = xmlVariable.Attribute("type");
                var value = xmlVariable.Value.ToString();

                //  Raise error if Value Type Attribute was not loaded from XML Model.
                if (typeAttrib == null)
                {
                    throw new ArgumentException($"Type of {name} variable cannot be empty.");
                }

                this.TrySetVariable(name, value);
            }
        }

        #endregion CLASS FUNCTIONS


        #region MODEL MANAGEMENT FUNCTIONS

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Get Instance Identifier. </summary>
        /// <returns> Model Class Instance Identifier. </returns>
        public string GetIdentifier()
        {
            return Id;
        }

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Generate new Identifier for Model Class Instance. </summary>
        public void ForceNewIdentifier()
        {
            Id = Guid.NewGuid().ToString("N");
        }

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Set Custom Instance Identifier. </summary>
        /// <param name="guid"> New GUID Identifier. </param>
        public void SetIdentifier(string identifier)
        {
            Id = identifier;
        }

        #endregion MODEL MANAGEMENT FUNCTIONS


        #region DATABASE MANAGEMENT FUNCTIONS

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Conver Class Model Instance to XML Model. </summary>
        /// <returns> XML Model to save into database. </returns>
        public XElement AsXML()
        {
            //  Create empty XML element to fill and return.
            var xmlResult = new XElement(this.GetName());
            
            //  Add Identifier Attribute.
            var identifierAttrib = new XAttribute("id", this.Id);
            xmlResult.Add(identifierAttrib);

            //  Add Public Class Model Instance Variables.
            foreach (var variable in this.GetVariables())
            {
                var typeAttrib = new XAttribute("type", variable.PropertyType.ToString());
                var xmlVariable = new XElement(variable.Name, variable.GetValue(this));
                xmlVariable.Add(typeAttrib);

                xmlResult.Add(xmlVariable);
            }

            return xmlResult;
        }

        #endregion DATABASE MANAGEMENT FUNCTIONS

    }
}
