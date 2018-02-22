using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection
{
    /// <summary>
    /// An attribute and the item that it is declared on.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <typeparam name="TMember">The type of the item on which the attribute is declared.</typeparam>
    public class AttributeAndMember<TAttribute, TMember>
        where TMember : MemberInfo
        where TAttribute : Attribute
    {
        /// <summary>
        /// The item that <see cref="Attribute"/> is declared on.
        /// </summary>
        public virtual TMember DeclaringMember { get; set; }

        /// <summary>
        /// An attribute of <see cref="DeclaringMember"/>.
        /// </summary>
        public virtual TAttribute Attribute { get; set; }
    }

    /// <summary>
    /// An attribute and the <see cref="MemberInfo"/> item that it is declared on.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <seealso cref="AttributeAndMember{TAttribute, TMember}"/>
    public class AttributeAndMember<TAttribute> : AttributeAndMember<TAttribute,MemberInfo>
        where TAttribute : Attribute
    {
    }
}
