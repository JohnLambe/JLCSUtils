using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.DataPopulater
{
    public class XmlNameAttribute : Attribute
    {
        public XmlNameAttribute(string name)
        {
            Name = name;
        }

        public virtual string Name { get; set; }
    }
}
