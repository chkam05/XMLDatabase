using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace XMLDatabase.Data.Base
{
    public interface IModel
    {

        #region CLASS MANAGEMENT FUNCTIONS

        PropertyInfo[] GetVariables();
        void SetVariable<T>(string name, T value);
        void TrySetVariable(string name, object value);
        string GetName();

        #endregion CLASS MANAGEMENT FUNCTIONS


        #region MODEL MANAGEMENT FUNCTIONS

        string GetIdentifier();
        void ForceNewIdentifier();
        void SetIdentifier(string identifier);

        #endregion MODEL MANAGEMENT FUNCTIONS


        #region DATABASE MANAGEMENT FUNCTIONS

        XElement AsXML();

        #endregion DATABASE MANAGEMENT FUNCTIONS

    }
}
