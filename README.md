# XMLDatabase

### XML File Database Framework for local projects with minimal database requirements.

This project allows to save any Instance of Class (variables) to XML File using 4 particular classes.

* ***IModel*** - is an interface with required function declarations to work with other Model Classes and Modules in this project.
* ***ModelBase*** - is an abstract class with required and declared in *IModel* function bodies, and variables **(Other models must inherit from it)**
* ***XMLDatabaseManager*** - allows to Add to XML, Update in XML, Remove from XML, Get/GetList from XML Instances of Class Models inherited from *ModelBase* and *IModel*.
* ***XMLDatabaseFileManager*** - allows to save XElement database container with values from Instances of Class Models into XML File.

## Usage Flow
![Usage Image](Documentation/XMLDatabase&#32;Usage.png)

## UML Diagrams
![UML Diagram Image](Documentation/XMLDatabase&#32;UML.png)

# Usage

**ModelBase** must be in usage name of the Model Class inherited from *ModelBase*.

## ModelBase

**ModelBase** must be in usage name of the Model Class inherited from *ModelBase*.

```ModelBase.ModelBase()``` - Create empty instance of Model Class with generated GUID private Identifier.  
```ModelBase.ModelBase(XElement xmlModel)``` - Create an instance of Model Class, filled by data from XML Element, received from XML File.  
```ModelBase.GetVariables()``` - Get array of Variables from this Model Class (inherited from *ModelBase*) as PropertyInfo with name, type, value of Variable.  
```ModelBase.SetVariable<T>(string name, T value)``` - Allow to set Vairable with particular generic "T" type which will be the value, and name as string.  
```ModelBase.TrySetVariable(string name, object value)``` - Allow to set unknown type of Variable that will be Converted to type of variable with particular name.  
```ModelBase.GetName()``` - return name of Class Model type.  
```ModelBase.GetIdentifier()``` - return private Identifier from this Instance of Class Model.  
```ModelBase.ForceNewIdentifier()``` - generate new GUID identifier for this Instance of Class Model.  
```ModelBase.SetIdentifier(string identifier)``` - allow to set custom GUID identifier from input string identifier value.  
```ModelBase.XElement AsXML()``` - convert Instance of Class Model to Instance of XElement Class Model representation in XML format.

## XMLDatabaseManager

```XMLDatabaseManager.XMLDatabaseManager(XElement xmlDatabase)``` - Load XElement as representation of XML object that is *root* of all elements contains in laded from file.  
```XMLDatabaseManager.CreateXMLDatabase( string[] dataModelsNames = null )``` - Create new free instance of XElement as representation of XML object that is *root* with XNodes that are names of Class Models of instances that will be storred in this XML database XElement object.  
```XMLDatabaseManager.GetDataModelInstance<T>(string identifier)``` - Allow get instance of Class Model with specific GUID identifier from XElement database instance.  
```XMLDatabaseManager.GetDataModelInstances<T>(Expression<Func<T, bool>> predicate = null)``` - Allow to get IEnumerable (list/collection) of instances of Class Model with specified assumptions given as Linq expresion.  
```XMLDatabaseManager.AddDataModelInstance(IModel dataModel)``` - Allow to add Instance of Class Model that is storred in XML database as XElement object.  
```XMLDatabaseManager.UpdateDataModelInstance(IModel dataModel)``` - Allow to update existing Instance of Class Model from XML database XElement object.  
```XMLDatabaseManager.RemoveDataModelInstance(IModel dataModel)``` - Allow to remove particular Instance of Class Model that is storred in XML database as XElement object, by passing it's copy as parameter.  
```XMLDatabaseManager.RemoveDataModelInstance<T>(string identifier)``` - Allow to remove particular Instance of Class Model that is storred in XML database as XElement object, by passing it's string GUID identifier.  
```XMLDatabaseManager.HasDataModelInstanceKey(string dataModelKey)``` - Check if XML Database XElement *root* object contains XNode for storring particular type of Class Model that name is passed as variable.
```XMLDatabaseManager.HasDataModelInstance(IModel dataModel)``` - Check if XML Database XElement *root* and his XNodes contains passed by variable Instance of Class Model.

## XMLDatabaseFileManager

```XMLDatabaseFileManager.SaveXMLDatabaseToFile(XElement xmlDatabase, string filePath)``` - Save XML database XElement *root* object into XML File storred under filePath.  
```XMLDatabaseFileManager.LoadXMLDatabaseFromFile(string filePath)``` - Load XML File from filePath and return data as XML Database XML XElement *root* object.

# Examples

## Creating Model, Add into XML Database, Save into XML File.
```
class MyModel : ModelBase, IModel
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public int Age { get; set; }
}
```
You can add multiple instances of one model, that identifier is unique.  
You can add multiple types of models that inherit from ***BaseModel***.
```
public class Program
{
    static void Main(string[] args)
    {
        var classNames = new string[] { typeof(ExampleModel).Name };
        var dbObject = XMLDatabaseManager.CreateXMLDatabase( classNames );
        var database = XMLDatabaseManager(dbObject);

        var modelInstance = new MyModel
        {
            Name = "Thomas",
            Surname = "Anderson",
            Age = 25
        };

        database.AddDataModelInstance(modelInstance);
        XMLDatabaseFileManager.SaveXMLDatabaseToFile(dbObject, "C:\MyXML.xml");
    }
}
```

## Loading model from file.
```
public class Program
{
    static void Main(string[] args)
    {
        var dbObject = XMLDatabaseFileManager.LoadXMLDatabaseFromFile("C:\MyXML.xml");
        var database = XMLDatabaseManager(dbObject);

        //  Get all Instances
        var listOfInstaces = XMLDatabaseManager.GetDataModelInstances<MyModel>().ToList();

        //  Get Instances where id is equal to ... particular GUID
        //  var modelInstace = XMLDatabaseManager.GetDataModelInstace<MyModel>( "640be81c..." );

        //  Get Instances where surname is equal to ... "Anderson"
        // var listOfInstances = XMLDatabaseManager.GetDataModelInstances<MyModel>(item => item.Surname == "Anderson").ToList();

        var modelInstace = listOfInstances[0];
    }
}
```

---
Open Source  
Copyright (c) Kamil Karpiński 2020