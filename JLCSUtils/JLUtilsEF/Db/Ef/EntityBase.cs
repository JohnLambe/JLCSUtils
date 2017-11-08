using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db.Ef
{
    /// <summary>
    /// Base class for Entity Framework entities.
    /// <para>
    /// Automatically initializes certain inverse collection properties to empty collections. See <see cref="CollectionInitializer"/>.
    /// </para>
    /// </summary>
    public class EntityBase
    {
        public EntityBase()
        {
            CollectionInitializer.InitializeInstance(this);
        }
    }
}
