using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Interfaces
{
    /// <summary>
    /// Marshals objects to/from a stream.
    /// </summary>
    public interface IObjectMarshaller
    {
        /// <summary>
        /// Add a representation of <paramref name="source"/> to the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="source"></param>
        /// <param name="name">The name (field name) of the item being written.</param>
        void MarshalObject(Stream stream, object source, string name = null);

        /// <summary>
        /// Marshal all items in <paramref name="source"/> to the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="source"></param>
        /// <param name="name"></param>
        void Marshal(Stream stream, ILookup<string,object> source, string name = null);

        /// <summary>
        /// Unmarshal the next object from the stream and return it.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        /// <returns>The name (field name) of the item read.</returns>
        string UnmarshalObject(Stream stream, ref object value);

        /// <summary>
        /// Unmarshall an object from the stream into the given dictionary.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="destination"></param>
        /// <returns>The name (field name) of the item read.</returns>
        string Unmarshal(Stream stream, IDictionary<string, object> destination);
    }


    /*
     * Provide adaptors from object (for its properties, possibly filtered using an attribute), DataReader (for its fields),
     *   IDictionry, etc. to ILookup<K,T>.
     * 
     * 
     * Possibly encoding formats (of implementations):
     * 
     * Optimised for performance of parser:
     *   EncodedItem ::=
     *       null-terminated UTF-8 string:  Field name.
     *       byte (enum):                   Type - identifies a primitive type, string, or byte array.
     *       If length is not implied by the type (if the type is `string` or `byte array`) {
     *         unsigned 32-bit integer:     Length.
     *       }
     *       0 or more bytes (length determined by preceding field):  Value.
     *   Encodes the same information as JSON; can be used as a direct replacement.
     * 
     * Optimised for size:
     *     Field names and type could be omitted if all fields are included and encoding and decoder have the same definition.
     *     The length of the length could be variable: Each byte encodes 7-bits of it and the 8th bit indicates whether there is another byte.
     * 
     * For maximum compatibility: Use JSON or XML.
     * 
     */

}
