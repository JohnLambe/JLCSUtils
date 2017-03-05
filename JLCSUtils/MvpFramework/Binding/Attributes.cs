using JohnLambe.Util.Misc;
using System;
using System.ComponentModel;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Base class for attributes of this framework.
    /// </summary>
    public abstract class MvpAttribute : Attribute
    {
    }


    /// <summary>
    /// Flags a method as a handler that can be invoked from a view.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class MvpHandlerAttribute : System.Attribute
    {
        public MvpHandlerAttribute(string name = null)
        {
            this.Name = name;
        }

        /// <summary>
        /// The ID of the handler, referenced in the user interface.
        /// null to derive from the method name (NOT IMPLEMENTED YET).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Set to false to disable a handler attribute on a base class.
        /// </summary>
        public virtual bool Enabled { get; set; } = true;

        /// <summary>
        /// Sorting order in a list of handlers.
        /// </summary>
        public virtual int Order { get; set; }

        public virtual string[] Filter { get; set; }


        // Details for generating a UI item:

        public virtual bool AutoGenerate { get; set; } = false;

        public virtual string DisplayName { get; set; }
        public virtual KeyboardKey HotKey { get; set; }  
        public virtual char AcceleratorChar { get; set; }
        [IconId]
        public virtual string IconId { get; set; }
        public virtual bool IsDefault { get; set; }
    }


    /// <summary>
    /// Attribute for any item that provides a name for displaying in a user interface.
    /// For use when there is code to use it for this purpose, e.g. this can be used on enum values, and a user interface
    /// for displaying or inputting a value of that type could use them.
    /// <para>This is allowed on items that <see cref="DisplayNameAttribute"/> (the base class) is not.
    /// When testing for the presence of this in code, testing for <see cref="DisplayNameAttribute"/> is recommended.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class DisplayNameAnyAttribute : DisplayNameAttribute
    {
    }


    /// <summary>
    /// Flags a property to be mapped to the title of a view (e.g. window title).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ViewTitleAttribute : MvpAttribute
    {
    }
}