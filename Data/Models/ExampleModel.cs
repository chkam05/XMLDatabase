using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using XMLDatabase.Data.Base;

namespace XMLDatabase.Data.Models
{
    public class ExampleModel : ModelBase, IModel
    {
        public bool BoolVar { get; set; }
        public int IntVar { get; set; }
        public double DoubleVar { get; set; }
        public string StringVar { get; set; }
        public DateTime DateTimeVar { get; set; }


        public ExampleModel() : base()
        {
            //
        }

        public ExampleModel(XElement xmlModel) : base(xmlModel)
        {
            //
        }
    }
}
