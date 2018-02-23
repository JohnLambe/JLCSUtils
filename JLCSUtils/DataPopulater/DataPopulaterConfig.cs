using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.DataPopulater
{
    public class DataPopulaterConfig
    {

        public virtual string Name { get; set; }

        public virtual IEnumerable<ClassConfig> Classes { get; set; }

        // Assemblies ?

    }
}
