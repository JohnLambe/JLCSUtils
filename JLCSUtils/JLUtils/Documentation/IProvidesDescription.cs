using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Documentation
{
    public interface IProvidesDescription  //TODO: Rename
    {
        /// <summary>
        /// A description of the type of item.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Describes the validation rule(s).
        /// </summary>
        string ValidationDescription { get; }
    }
}
