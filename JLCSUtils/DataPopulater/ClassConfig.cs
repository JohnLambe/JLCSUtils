using JohnLambe.Util.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.DataPopulater
{
    [XmlName("class")]
    public class ClassConfig
    {
        public virtual string ClassName
        {
            get { return _className; }
            set
            {
                _className = value;
                TargetClass = ReflectionUtil.FindType(value);
            }
        }
        private string _className;

        public virtual Type TargetClass { get; protected set; }

        public virtual int MinimumInstances { get; set; } = 1;
        public virtual int MaximumInstances { get; set; } = 1;

        public virtual int Instances
        {
            set
            {
                MinimumInstances = value;
                MaximumInstances = value;
            }
        }
    
        public virtual IEnumerable<PropertyConfigBase> Properties { get; set; }

        public virtual object NewInstance()
        {
            return ReflectionUtil.Create<object>(TargetClass);
        }

    }
}
