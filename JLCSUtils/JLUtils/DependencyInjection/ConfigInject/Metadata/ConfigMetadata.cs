using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.DependencyInjection.ConfigInject.Metadata
{
    public class ConfigMetaData
    {
        public virtual IList<ConfigMetaDataItem> Items { get; protected set; }
    }

    public class ConfigMetaDataItem
    {
        public virtual string Name { get; set; }
        public virtual Type DataType { get; set; }
        public virtual string Description { get; set; }
        public virtual string Module { get; set; }
        public virtual string Category { get; set; }
        public virtual object DefaultValue { get; set; }
    }

    public class ConfigMetaDataProperty : ConfigMetaDataItem
    {
    }

    public class ConfigMetaDataCommand : ConfigMetaDataItem
    {
    }

}
