using JohnLambe.Util.Misc;
using JohnLambe.Util.Reflection;
using System;
using System.ComponentModel;

namespace MvpFramework.Binding
{
    // Attributes for binding views, independent of the UI framework:

    /// <summary>
    /// Base class for attributes of this framework.
    /// </summary>
    // Subclasses are not necessarily required to be independent of the UI framework, but any that have dependendies on one,
    // must be declared in a namespace relating to that framework.
    public abstract class MvpAttributeBase : Attribute
    {
    }

    /// <summary>
    /// Attribute of this framework with an <see cref="Enabled"/> property.
    /// </summary>
    public abstract class MvpEnabledAttributeBase : MvpAttributeBase, IEnabledAttribute
    {
        /// <summary>
        /// True to enable handling of this attribute.
        /// Defaults to true. Set to false on overriding members to disable an attribute on the overridden member.
        /// </summary>
        public virtual bool Enabled { get; set; } = true;
    }


    /// <summary>
    /// Base class for attributes that can be used to generate a user interface element.
    /// </summary>
    public abstract class MvpUiAttributeBase : MvpDisplayAttributeBase
    {
        /// <summary>
        /// The ID of the handler, referenced in the user interface.
        /// null to derive from the method name.
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Sorting order in a list of handlers.
        /// </summary>
        public virtual int Order { get; set; }

        /// <summary>
        /// Filters where this appears or is accessible.
        /// e.g. depending on the view and this value, a button may be created that invokes the handler.
        /// </summary>
        public virtual string[] Filter { get; set; }

        /// <summary>
        /// If there is only one item in <see cref="Filter"/>, this is it.
        /// If there are none, this is null.
        /// Otherwise, an exception is thrown on reading this.
        /// </summary>
        public virtual string SingleFilter
        {
            get
            {
                if (Filter == null || Filter.Length == 0)
                    return null;
                else if (Filter.Length == 1)
                    return Filter[0];
                else
                    throw new InvalidOperationException("Multiple Filters are defined");
            }
            set
            {
                if (value == null)
                    Filter = null;
                else
                    Filter = new string[] { value };
            }
        }

        // Details for generating a UI item:

        /// <summary>
        /// True iff the user interface for this item should be generated automatically.
        /// Defaults to true.
        /// </summary>
        public virtual bool AutoGenerate { get; set; } = true;

        /// <summary>
        /// The name displayed for this item in the UI.
        /// </summary>
        public virtual string DisplayName { get; set; }
        //TODO?: Localisation.

        /// <summary>
        /// Keystroke to invoke this item.
        /// </summary>
        public virtual KeyboardKey HotKey { get; set; }

        /// <summary>
        /// The icon to be displayed in the UI for this item.
        /// </summary>
        [IconId]
        public virtual string IconId { get; set; }

        /// <summary>
        /// An identifier of a group of items (commands or properties).
        /// This can be used by the user interface to group field controls or buttons, etc.
        /// </summary>
        public virtual string Group { get; set; }

        /// <summary>
        /// Rights or roles required to access this item.
        /// To access this, the user must have one of the rights specified by an element of the array.
        /// The format of the string depends on the consuming system. It may specify a combination of rights/roles
        /// (So elements of the array are ORed, but rights may be ANDed within each element), or an expression.
        /// </summary>
        public virtual string[] Rights { get; set; }
        //TODO?: Change type to an interface, IPrivilege (same for all similar 'Rights' properties).
    }


    #region For Presenters

    /// <summary>
    /// Flags a method as a handler that can be invoked from a view.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class MvpHandlerAttribute : MvpUiAttributeBase
    {
        /// <summary>
        /// </summary>
        /// <param name="id">Value of <see cref="MvpUiAttributeBase.Id"/>.</param>
        public MvpHandlerAttribute(string id = null)
        {
            this.Id = id;
        }
        
        [Obsolete("Use Id")] // Renamed to Id. ('Name' could be confused with a name for display).
        public virtual string Name
        {
            get { return Id; }
            set { Id = value; }
        }        

        /// <summary>
        /// true iff this is the default button or default item in a list, etc.
        /// </summary>
        public virtual bool IsDefault { get; set; }

        //TOOO?: public virtual object ModalResult { get; set; }
    }

    #endregion


    #region For Models and Presenters

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
        public DisplayNameAnyAttribute()
        {
        }
        public DisplayNameAnyAttribute(string displayName) : base(displayName)
        {
        }

        /// <summary>
        /// A shorter version of the display name, for use when space is more limited.
        /// <para>
        /// If this is present, and <see cref="DisplayNameAttribute.DisplayName"/> is null, this should NOT be considered
        /// a default for it (if there is another method of getting a default name (for example, using the code name of the attributed item),
        /// it should be used).
        /// </para>
        /// </summary>
        public virtual string ShortName { get; set; }

        /*TODO:
         * Or use MvpDisplayNameAny
        /// <summary>
        /// Accelerator character for this item in a user interface.
        /// </summary>
        public virtual char AcceleratorChar { get; set; }
        */
    }

    /// <summary>
    /// Defines a group that items can be assigned to, for grouping in the unser interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class GroupDefinitionAttribute : MvpUiAttributeBase, IUiGroupModel
    {
        /// <summary>
        /// Represents the group of all items that have no group defined.
        /// </summary>
        public static IUiGroupModel Ungrouped
            => new GroupDefinitionAttribute()
            {
                Id = "",                
            };
    }

    /// <summary>
    /// Details of a group of items in the user interface (without the items).
    /// </summary>
    public interface IUiGroupModel
    {
        /// <summary>
        /// The ID of the handler, referenced in the user interface.
        /// </summary>
        string Id { get; }

//        string ParentId { get; set; }

        /// <summary>
        /// Sorting order weight.
        /// </summary>
        int Order { get; }


        // Details for generating a UI item:

        /// <summary>
        /// True iff this group be automatically generated.
        /// </summary>
        bool AutoGenerate { get; }

        /// <summary>
        /// The name displayed on this item in the UI.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Keystroke to invoke this item.
        /// </summary>
        KeyboardKey HotKey { get; }

        /// <summary>
        /// Character to choose this item in the UI when in a list, or a WinForms accelerator character, etc.
        /// </summary>
        char AcceleratorChar { get; }

        /// <summary>
        /// The icon to be displayed in the UI for this item.
        /// </summary>
        [IconId]
        string IconId { get; }

        /*
        /// <summary>
        /// Rights or roles required to access this item.
        /// To access this, the user must have one of the rights specified by an element of the array.
        /// The format of the string depends on the consuming system. It may specify a combination of rights/roles.
        /// (So elements of the array are ORed, but rights may be ANDed within each element.)
        /// </summary>
        public virtual string[] Rights { get; set; }
        */
    }

    #endregion


    #region For Models

    /// <summary>
    /// Additional display options.
    /// </summary>
    public abstract class MvpDisplayAttributeBase : MvpEnabledAttributeBase
    {
        public virtual bool IsVisible { get; set; } = true;

        /// <summary>
        /// Character to choose this item in the UI when in a list, or a WinForms accelerator character, etc.
        /// </summary>
        public virtual char AcceleratorChar { get; set; } = AcceleratorCaptionUtil.None;
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class MvpDisplayAttribute : MvpDisplayAttributeBase
    {
    }

    #endregion


    #region For attributed controls

    /// <summary>
    /// Flags a control binder class for automatic registration.
    /// The attributed class must implement <see cref="IControlBinder"/>.
    /// <para>A class can bind multiple classes of control, using multiple instances of this attribute.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ControlBinderAttribute : MvpEnabledAttributeBase
    {
        /// <summary>
        /// </summary>
        /// <param name="forControl"><inheritdoc cref="ForControl"/></param>
        public ControlBinderAttribute(Type forControl)
        {
            this.ForControl = forControl;
        }

        /// <summary>
        /// A control class for which the attributed class is a binder.
        /// </summary>
        public virtual Type ForControl { get; set; }

        //| We could add a property to be passed to the contructor. This could be used to distinguish different control mappings,
        //| but the same effect could be achieved with subclasses for different controls, or the constructor testing the type of control
        //| passed.
    }


    /// <summary>
    /// Flags a UI control class to be bound based on attributes.
    /// <seealso cref="AttributedControlBinder"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MvpBoundControlAttribute : MvpEnabledAttributeBase
    {
        /// <summary>
        /// Iff true, and the control class implements <see cref="IControlBinder"/>, it is also bound, after <see cref="AttributedControlBinder"/>.
        /// </summary>
        public virtual bool UseOwnHandler { get; set; } = true;

        /// <summary>
        /// Name of the property of the bound control that holds the Icon ID (<see cref="IconIdAttribute"/>).
        /// (For use when an IconID is available, e.g. from a <see cref="MvpHandlerAttribute"/>).
        /// </summary>
        /// <seealso cref="IconProperty"/>
        public virtual string IconIdProperty { get; set; }

        /// <summary>
        /// Name of the property of the bound control that holds the icon.
        /// (For use when an icon or IconID is available, e.g. from a <see cref="MvpHandlerAttribute"/>).
        /// <para>
        /// If both this and <see cref="IconIdProperty"/> are assigned, and an IconID and Icon Repository are available, both properties may be assigned, but only one would generally be required.
        /// Which property is supported depends on the bound control.
        /// </para>
        /// </summary>
        public virtual string IconProperty { get; set; }

        //| We could provide a property for the class that binds this control, but that can already be done
        //| using ControlBinderAttribute (with the mapping in the other direction), and mapping from binder
        //| to control is probably more useful (the same control could have different binders in different systems),
        //| and if a control always has the same binder, it can implement IControlBinder itself.
    }

    /// <summary>
    /// Base class for attributes for properties whose value defines a mapping between a view and a model or presenter,
    /// or events that return a value for this purpose.
    /// </summary>
    //[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false, Inherited = true)]
    //TODO: Support event that returns the value.
    //TODO: Support on Event. The event returns the property name or handler ID.
    public abstract class MvpMappingPropertyBaseAttribute : MvpEnabledAttributeBase
    {
    }

    /// <summary>
    /// Flags a property/event of a control as holding/returning the name of a property on the model
    /// to be bound to the control.
    /// <para>
    /// The atributed property is usually of type <see cref="string"/>.
    /// If it is not <see cref="string"/>, its <see cref="object.ToString()"/> method is used to give the model property name.
    /// </para>
    /// <para>
    /// The specified property on the control is populated from the model on binding.
    /// The property on the model is populated from the control when the specified event is fired.
    /// </para>
    /// </summary>
    public class MvpModelPropertyAttribute : MvpMappingPropertyBaseAttribute
    {
        /// <summary>
        /// </summary>
        /// <param name="valuePropertyName"><inheritdoc cref="ValuePropertyName"/></param>
        public MvpModelPropertyAttribute(string valuePropertyName)
        {
            this.ValuePropertyName = valuePropertyName;
        }

        /// <summary>
        /// </summary>
        /// <param name="valuePropertyName"><inheritdoc cref="ValuePropertyName"/></param>
        /// <param name="changeEventName"><inheritdoc cref="ChangeEventName"/></param>
        public MvpModelPropertyAttribute(string valuePropertyName, string changeEventName) : this(valuePropertyName)
        {
            this.ChangeEventName = changeEventName;
        }

        /// <summary>
        /// The name of the property (on the attributed class, possibly a superclass) that holds the value that is mapped to the model.
        /// </summary>
        public virtual string ValuePropertyName { get; set; }

        /// <summary>
        /// The name of the event (on the attributed class, possibly a superclass) that is fired when the bound property
        /// (specified by <see cref="ValuePropertyName"/>) on the control changes.
        /// null if changes in the control do not update the model.
        /// </summary>
        public virtual string ChangeEventName { get; set; }

        // public virtual string ModelEventName { get; set; }  ? // Event on model, fired when the model property changes, to update the Control.
    }

    /// <summary>
    /// Flags a property/event of a control as holding/returning the ID of a handler on the presenter
    /// to be bound to the control.
    /// <para>This property is usually of type <see cref="string"/>.
    /// If it is not <see cref="string"/>, its <see cref="object.ToString()"/> method is used.
    /// </para>
    /// </summary>
    public class MvpHandlerIdPropertyAttribute : MvpMappingPropertyBaseAttribute
    {
        /// <summary>
        /// </summary>
        /// <param name="eventName"><inheritdoc cref="EventName"/></param>
        public MvpHandlerIdPropertyAttribute(string eventName)
        {
            this.EventName = eventName;
        }

        /// <summary>
        /// The name of the event (on the attributed class, possibly a superclass) to be bound to the presenter.
        /// </summary>
        public virtual string EventName { get; set; }
    }

    /// <summary>
    /// Flags a property whose value is a binder string.
    /// Supported property types are <see cref="string"/>, <see cref="string"/>[] (each element is a binder string)
    /// and anything whose <see cref="object.ToString()"/> method returns the required value.
    /// <para>Syntax:
    /// <code>
    /// BinderString = *( BinderExpression ("|" / NL ) ) BinderExpression
    /// BinderExpression = ViewName BindingType ModelName [Comment]
    /// BindingType = "&lt;-"    ; Model to View
    ///               / "-&gt;"  ; View to Model
    ///               / "&lt;-&gt;" ; Bi-directional
    ///               / "=&gt;"  ; Invoke handler. ModelName is a HandlerId. ViewName can be an event or property. If a property, its 'Change' event is used.
    /// Comment = "//" *( &lt;any non-special character&gt; )
    /// 
    /// ViewName and ModelName are property names, or ViewName can be an event name, and ModelName can be a HandlerId.
    /// ViewName can be omitted on <see cref="MvpMappingPropertyBaseAttribute"/>, in which case, its value is provided by the attribute.
    /// BindingType can be omitted when ViewName is omitted. It defaults to "&lt;-&gt;" or "=&gt;" depending on the attribute type.
    /// </code>
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MvpBinderStringAttribute : MvpEnabledAttributeBase
    {
    }

    /// <summary>
    /// Causes the attributed event to be fired before and/or after binding.
    /// </summary>
    [AttributeUsage(AttributeTargets.Event, AllowMultiple = false, Inherited = true)]
    public class MvpBindingEventAttribute : MvpEnabledAttributeBase
    {
        /// <summary>
        /// Specifies whether the event is fired before and/or after binding.
        /// </summary>
        public virtual EventFireTime When { get; set; }
    }

    /// <summary>
    /// For controls that implement <see cref="IControlBinder"/>:
    /// This causes child controls of the attributed control to be bound in the usual way.
    /// This is the normal behaviour for controls that don't implement <see cref="IControlBinder"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MvpBindChildrenAttribute : MvpEnabledAttributeBase
    {
    }

    /// <summary>
    /// Specifies whether an event fires before and/or after an action.
    /// </summary>
    [Flags]
    public enum EventFireTime
    {
        Never = 0,
        Before = 1,
        After = 2,
        BeforeAndAfter = Before | After
    }

    /*
    /// <summary>
    /// Base class for attributes on a View that bind a property or event (which may be declared on a superclass of the attributed one)
    /// on the View to the model/presenter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class MvpBoundPropertyBaseAttribute : MvpAttribute
    {
        /// <summary>
        /// </summary>
        /// <param name="control">The name of the member (property or event) on the control.</param>
        /// <param name="model">The name of the item on the model or presenter.</param>
        public MvpBoundPropertyBaseAttribute(string control, string model)
        {
            this.NameOnControl = control;
            this.BindToName = model;
        }

        /// <summary>
        /// The name of the member (property or event) on the control.
        /// </summary>
        public virtual string NameOnControl { get; set; }

        /// <summary>
        /// The name of the item on the model or presenter.
        /// </summary>
        public virtual string BindToName { get; set; }
    }

    /// <summary>
    /// Binds a property (which may be declared on a superclass of the attributed one)
    /// on the attributed View to the Model.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class MvpBoundPropertyAttribute : MvpBoundPropertyBaseAttribute
    {
        /// <summary>
        /// </summary>
        /// <param name="control">The name of the property on the control.</param>
        /// <param name="model">The name of the property on the model.</param>
        public MvpBoundPropertyAttribute(string control, string model) : base(control,model)
        { }

        /// <summary>
        /// </summary>
        /// <param name="control">The name of the property on both the control and the model.</param>
        public MvpBoundPropertyAttribute(string control) : this(control, control)
        { }
    }

    /// <summary>
    /// Binds an event (which may be declared on a superclass of the attributed one)
    /// on the attributed View to the Presenter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class MvpBoundEventAttribute : MvpBoundPropertyBaseAttribute
    {
        /// <summary>
        /// </summary>
        /// <param name="control">The name of the event on the control.</param>
        /// <param name="handler">The name of the handler on the presenter.</param>
        public MvpBoundEventAttribute(string control, string handler) : base(control, handler)
        { }

        /// <summary>
        /// Binds the default event.
        /// </summary>
        /// <param name="control">The name of the event control.</param>
        public MvpBoundEventAttribute(string control) : this(control, null)
        { }
    }
    */

    #endregion


    #region For Views

    /// <summary>
    /// Binds the attributed property on a view to a property of the model.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MvpBindAttribute : MvpEnabledAttributeBase
    {
        /// <summary>
        /// Value of <see cref="Key"/> to bind the whole model (the model instance rather than a member of it).
        /// </summary>
        public const string Model = "";

        /// <summary>
        /// </summary>
        /// <param name="key"><see cref="Key"/></param>
        public MvpBindAttribute(string key = null)
        {
            this.Key = key;
        }

        /// <summary>
        /// The name of the property of the model to be bound to the attributed item.
        /// null to use the name of the attributed item.
        /// </summary>
        //| The name of this is chosen to match DiExtension.Attributes.InjectAttribute.Key.
        public virtual string Key { get; set; }

        /// <summary>
        /// Iff true, an exception is thrown if binding fails.
        /// </summary>
        public virtual bool Required { get; set; } = true;
    }

    /// <summary>
    /// Specifies the type of the model to be used with the attributed View.
    /// This can be used on a view class or interface.
    /// </summary>
    /// <remarks>
    /// This can be used instead of a generic type parameter.
    /// The WinForms designer does not support classes with generic type parameters.
    /// 
    /// DOESN'T DO ANYTHING YET, BUT CAN BE USED FOR DOCUMENTATION.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class ModelTypeAttribute : MvpAttributeBase
    {
        /// <summary>
        /// </summary>
        /// <param name="modelType">The type of the model.</param>
        public ModelTypeAttribute(Type modelType)
        {
            this.ModelTypes = new Type[] { modelType };
        }

        /// <summary>
        /// </summary>
        /// <param name="modelTypes"><see cref="ModelTypes"/></param>
        public ModelTypeAttribute(Type[] modelTypes)
        {
            this.ModelTypes = modelTypes;
        }

        /// <summary>
        /// The types of model supported by this View.
        /// If more than one, the view can accept any of them.
        /// Any types assignable to these types can be used as the model.
        /// <para>A value of null should be interpreted the same as if the attribute was not defined, i.e. the type of the model is not specified.</para>
        /// <para>If this is empty, it means that the view has no model.</para>
        /// </summary>
        public virtual Type[] ModelTypes { get; protected set; }

        /// <summary>
        /// Default nullability handling of bound property names.
        /// (The value of the defaultNullability parameter to <see cref="ReflectionUtil.TryGetPropertyValue{T}(object, string, PropertyNullabilityModifier)"/>).
        /// </summary>
        public virtual PropertyNullabilityModifier Nullability { get; set; }
    }

    #endregion

    #region For View Interfaces

    /// <summary>
    /// Base class for attributes that map events to handlers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Event, AllowMultiple = false, Inherited = true)]
    //|TODO?: Could allow multiple.
    public abstract class MvpEventDefinitionAttributeBase : MvpEnabledAttributeBase
    {
        /// <summary>
        /// Iff true, an exception is thrown if no handler is bound to this.
        /// </summary>
        public virtual bool Required { get; set; }
    }

    /// <summary>
    /// Specifies a handler ID for an event on a View, View Interface or constant on a View Interface Extension Class (to be bound to a <see cref="MvpHandlerAttribute"/>).
    /// <para>If the Presenter has a matching handler, it will be added to the event. (It can potentially fire multiple handlers if there are multiple matching ones).
    /// </para>
    /// <para>CURRENTLY IMPLEMENTED FOR VIEWS ONLY.</para>
    /// </summary>
    public class MvpEventAttribute : MvpEventDefinitionAttributeBase
    {
        /// <summary>
        /// </summary>
        /// <param name="id"><see cref="Id"/></param>
        public MvpEventAttribute(string id = null)
        {
            this.Id = id;
        }

        /// <summary>
        /// The ID of the handler (matches an ID defined in a <see cref="MvpHandlerAttribute"/>).
        /// null to use the event name.
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Iff true, and there are no handlers available, a handler that does nothing is injected.
        /// In this case, <see cref="MvpEventDefinitionAttributeBase.Required"/> is ignored.
        /// </summary>
        public virtual bool EmptyHandler { get; set; }
    }

    /// <summary>
    /// Placed on a constant on a View Interface Extension Class whose value is a handler ID for an event.
    /// <para>It is recommended to specify this on a constant and reference the constant in <see cref="MvpHandlerAttribute"/> instances, to avoid using a string literal.</para>
    /// <para>NOT IMPLEMENTED YET.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public abstract class MvpEventDefinitionAttribute : MvpEventDefinitionAttributeBase
    {
    }

    #endregion

}