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
        public MvpHandlerAttribute(string id = null)
        {
            this.Id = id;
        }

        /// <summary>
        /// The ID of the handler, referenced in the user interface.
        /// null to derive from the method name (NOT IMPLEMENTED YET).
        /// </summary>
        public virtual string Id { get; set; }
        
        [Obsolete("Use Id")] // Renamed to Id. ('Name' could be confused with a name for display).
        public virtual string Name
        {
            get { return Id; }
            set { Id = value; }
        }        

        /// <summary>
        /// Set to false on an attribute on an overridden member, to disable a handler attribute on a base class.
        /// </summary>
        public virtual bool Enabled { get; set; } = true;

        /// <summary>
        /// Sorting order in a list of handlers.
        /// </summary>
        public virtual int Order { get; set; }

        /// <summary>
        /// Filters where this appears is accessible.
        /// </summary>
        public virtual string[] Filter { get; set; }


        // Details for generating a UI item:

        public virtual bool AutoGenerate { get; set; } = false;

        /// <summary>
        /// The name displayed on this item in the UI.
        /// </summary>
        public virtual string DisplayName { get; set; }
        //TODO?: Localisation.

        /// <summary>
        /// Keystroke to invoke this item.
        /// </summary>
        public virtual KeyboardKey HotKey { get; set; }  

        /// <summary>
        /// Character to choose this item in the UI when in a list, or a WinForms accelerator character, etc.
        /// </summary>
        public virtual char AcceleratorChar { get; set; }

        /// <summary>
        /// The icon to be displayed in the UI for this item.
        /// </summary>
        [IconId]
        public virtual string IconId { get; set; }

        /// <summary>
        /// true iff this is the default button or default item in a list, etc.
        /// </summary>
        public virtual bool IsDefault { get; set; }

        /// <summary>
        /// Rights or roles required to access this item.
        /// To access this, the user must have one of the rights specified by an element of the array.
        /// The format of the string depends on the consming system. It may specify a combination of rights/roles.
        /// (So elements of the array are ORed, but rights may be ANDed within each element.)
        /// </summary>
        public virtual string[] Rights { get; set; }
        //TODO?: Change type to an interface, IPrivilege (same for all similar 'Rights' properties).

        //TOOO?: public virtual object ModalResult { get; set; }
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


    /// <summary>
    /// Flags a control binder class for automatic registration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ControlBinderAttribute : MvpAttribute
    {
        public ControlBinderAttribute(Type forControl)
        {
            this.ForControl = forControl;
        }

        /// <summary>
        /// The control class for which the annotated class is a binder.
        /// </summary>
        public virtual Type ForControl { get; set; }
    }

}