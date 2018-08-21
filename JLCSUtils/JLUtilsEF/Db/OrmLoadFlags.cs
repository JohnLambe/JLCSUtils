using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db
{
    /// <summary>
    /// Options relating to loading entities.
    /// </summary>
    [Flags]
    public enum OrmLoadFlags
    {
        Default = 0,

        /// <summary>
        /// Load the entity, excluding items covered by other options.
        /// </summary>
        Entity = 1,

        /// <summary>
        /// Populate navigaton properties directly on the entity.
        /// </summary>
        References = 2,

        /// <summary>
        /// Populate collection properties directly on the entity.
        /// </summary>
        Collections = 4,

        /// <summary>
        /// Load only if the entity is already attached to the context.
        /// </summary>
        IfAttached = 0x1000000,
    }

}
