using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Interfaces
{
    /// <summary>
    /// An instance that has a unique ID.
    /// </summary>
    /// <typeparam name="TKey">The type of the ID.</typeparam>
    public interface IHasId<TKey>
    {
        /// <summary>
        /// The unique ID of this instance.
        /// </summary>
        TKey Id { get; }
    }

    
    /// <summary>
    /// An instance that has a name.
    /// </summary>
    public interface INamedItem
    {
        /// <summary>
        /// The name of this instance.
        /// </summary>
        string Name { get; }
    }


    /// <summary>
    /// An instance that has a description.
    /// </summary>
    public interface IHasDescription
    {
        /// <summary>
        /// The description (human readable) of this item.
        /// </summary>
        string Description { get; }
    }

}
