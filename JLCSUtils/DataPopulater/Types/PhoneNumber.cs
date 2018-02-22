using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Types
{
    //TODO

    public class PhoneNumber
    {
        public const char InternationalPrefix = '+';

        public virtual string Value { get; set; }

        /*
        public virtual string InternationalForm
        {
            get
            {
                if (Value.StartsWith(InternationalPrefix))
                    return Value;

            }
//           set;
        }
        

        public virtual string CountryCode
        {
            get { }
        }
        */

        /// <summary>
        /// Phone number without spaces or hyphens.
        /// </summary>
        public virtual string ShortForm
        {
            get { return Value.RemoveCharacters(" -"); }
            set { Value = value; }
        }
    }
}
