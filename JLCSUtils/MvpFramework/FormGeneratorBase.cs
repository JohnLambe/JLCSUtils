using DiExtension;
using JohnLambe.Types;
using JohnLambe.Util.Collections;
using JohnLambe.Util.Diagnostic;
using JohnLambe.Util.FilterDelegates;
using JohnLambe.Util.Reflection;
using MvpFramework.Binding;
using MvpFramework.Dialog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    /// <summary>
    /// Populates a user interface control with a user interface for a given model, or selected properties of it.
    /// </summary>
    /// <typeparam name="TControl"></typeparam>
    public abstract class FormGeneratorBase<TControl>
        where TControl : class
    {
        public FormGeneratorBase(IDiContext diContext = null)
        {
            this.DiContext = diContext;
            CachedDataTypeToControlTypeMap = new CachedSimpleLookup<Type,Type>(DataTypeToControlTypeMap);
        }


        /// <summary>
        /// The user interface control into which the generated controls will be added.
        /// </summary>
        public virtual TControl Target { get; set; }

        /// <summary>
        /// The model from which to generate a user interface.
        /// </summary>
        public virtual ModelBinderWrapper Model { get; set; }

        public virtual AutoSizeOption AutoSize { get; set; } = AutoSizeOption.Grow;

        /*
        /// <summary>
        /// Coordinates (top left) of first control caption.
        /// </summary>
                public virtual Point Coords { get; set; }
        */

        /// <summary>
        /// Generate controls from the model.
        /// </summary>
        /// <param name="groupFilter">
        /// Delegate to filter groups to generate controls for.
        /// null for all.
        /// </param>
        /// <param name="propertyFilter">
        /// Delegate to filter propertis within each group, to generate controls for.
        /// null for all.
        /// </param>
        public virtual void Generate(FilterDelegate<IUiGroupModel> groupFilter = null, FilterDelegate<ModelPropertyBinder> propertyFilter = null)
        {
            if (Target != null && Model != null)
            {
                StartGenerate();

                GenerateGroup(GroupDefinitionAttribute.Ungrouped, propertyFilter);

                foreach (var group in Model.Groups.Where(g => groupFilter == null || groupFilter(g)))
                {
                    GenerateGroup(group, propertyFilter);
                }

                EndGenerate();
            }
        }

        public virtual void GenerateGroup(IUiGroupModel group, FilterDelegate<ModelPropertyBinder> propertyFilter)
        {
            TControl parentControl;
            if (group.Id != "")
                parentControl = CreateGroup(null, group);
            else
                parentControl = Target;      // if no group, then controls are being added directly to the target control

            int index = 0;
            foreach (var property in Model.GetPropertiesByGroup(group.Id)
                .Where(p => (propertyFilter == null || propertyFilter(p)) && p.AutoGenerate)
                )
            {
                var context = new ControlGeneratorContext()
                {
                    Group = group,
                    ParentControl = parentControl,
                    PropertyBinder = property,
                    Index = ++index
                };
                CreateControl(context);

                /*
                var attribute = property.GetCustomAttribute<UiAttribute>();

                var mapping = new PropertyMapping();
                mapping.Property = property;
                mapping.Caption = property.Name;
                mapping.DataType = property.PropertyType;

                mapping.Priority = attribute?.Priority ?? 0;  //TODO: auto-generate priority when not explicitly given

                //TODO: Populate validation and other settings,
                // including from Data Annotations.

                _mappings.Add(mapping);
                //TODO: Sort _mappings by Priority
                */
            }

        }


        /// <summary>
        /// Called once before generating a collection of controls.
        /// </summary>
        public virtual void StartGenerate()
        {
        }

        /// <summary>
        /// Called after generaating a collection of controls.
        /// </summary>
        public virtual void EndGenerate()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent">The parent group, or null for the root.</param>
        /// <param name="group">The model of the group to be created.</param>
        public virtual TControl CreateGroup(IUiGroupModel parent, IUiGroupModel group)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public virtual TControl CreateControl([NotNull]ControlGeneratorContext context)
        {
            BeforeCreateControl(context);

            OnCreateControl(context);

            AfterCreateControl(context);

            return context.NewControl;
        }

        /// <summary>
        /// Called before creating a control.
        /// This can modify the context and/or populate <see cref="ControlGeneratorContext.NewControl"/> in <paramref name="context"/>,
        /// thus preventing the usual method of creating the control (including calling <see cref="OnCreateControl(ControlGeneratorContext)"/>).
        /// </summary>
        /// <param name="context"></param>
        protected virtual void BeforeCreateControl([NotNull]ControlGeneratorContext context)
        {
        }

        /// <summary>
        /// Creates a control.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnCreateControl([NotNull]ControlGeneratorContext context)
        {
            if (context.NewControl == null)   // if not already created
            {
                var controlType = GetControlTypeForDataType(context.PropertyBinder.PropertyType);
                try
                {
                    context.NewControl = ReflectionUtil.CallStaticMethod<TControl>(controlType, MvpControlMappingAttribute.CreateControlMethod, context);
                }
                catch (MissingMemberException)
                {
                    context.NewControl = ReflectionUtil.Create<TControl>(controlType);
                }
            }
            Diagnostics.Assert(context.NewControl != null, "FormGeneratorBase.OnCreateControl: NewControl==null");

            DiContext?.BuildUp(context.NewControl);                       // run dependincy injection if we have a DI container

            (context.NewControl as IGeneratableControl)?.ControlGeneratation(context);  // call if it implements the interface
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        protected virtual void AfterCreateControl([NotNull]ControlGeneratorContext context)
        {
        }


        /// <summary>
        /// Get a UI control that handles a given data type.
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        protected virtual Type GetControlTypeForDataType(Type dataType)
        {
            return CachedDataTypeToControlTypeMap.TryGetValue(dataType);
        }


        #region Scanning for mappings

        /// <summary>
        /// Scan assemblies and register mappings.
        /// </summary>
        /// <param name="assemblies">The list of assemblies to scan. If empty, the calling assembly is scanned.</param>
        public virtual void Scan(params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = new Assembly[] { Assembly.GetCallingAssembly() };
            ScanAssemblies(assemblies);
        }

        /// <summary>
        /// Scan assemblies and register mappings.
        /// </summary>
        /// <param name="assemblies">The list of assemblies to scan. If empty, this does nothing.</param>
        public virtual void ScanAssemblies(IEnumerable<Assembly> assemblies)
        {
            foreach (var attrib in assemblies.SelectMany(a => a.GetTypes()).SelectMany(t => t.GetAttributesWithMember<MvpControlMappingAttribute, Type>()))
            {
                AddMapping(attrib.Attribute.ForType, attrib.DeclaringMember);
            }
        }

        #endregion

        public virtual void AddMapping(Type dataType, Type controlType)
        {
            DataTypeToControlTypeMap.Add(dataType, controlType);
        }


        /// <summary>
        /// Dependency injection context from which new controls are injected.
        /// </summary>
        protected virtual IDiContext DiContext { get; private set; }

        /// <summary>
        /// Tracks accelerator characters in the generated controls.
        /// </summary>
        protected virtual AcceleratorCaptionUtil Accelerators { get; } = new AcceleratorCaptionUtil();

        /// <summary>
        /// Maps the type of data displayed/edited to a type of control to handle it.
        /// </summary>
        protected readonly TypeMap DataTypeToControlTypeMap = new TypeMap();
        protected virtual ISimpleLookup<Type, Type> CachedDataTypeToControlTypeMap { get; }


        /// <summary>
        /// Details provided when creating a control.
        /// </summary>
        public class ControlGeneratorContext
        {
            /// <summary>
            /// The UI group of the new new control is in.
            /// This will have been passed in a previous call to <see cref="FormGeneratorBase{TControl}.CreateGroup"/>.
            /// </summary>
            public virtual IUiGroupModel Group { get; set; }

            /// <summary>
            /// The property to be bound to the new control.
            /// </summary>
            public virtual ModelPropertyBinder PropertyBinder { get; set; }

            /// <summary>
            /// The UI control that the new control is to be placed in
            /// (typically corresponds to <see cref="Group"/>, and is the control returned by
            /// the call to <see cref="FormGeneratorBase{TControl}.CreateGroup(IUiGroupModel, IUiGroupModel)"/> for that group).
            /// </summary>
            public virtual TControl ParentControl { get; set; }

            /// <summary>
            /// The control being generated.
            /// </summary>
            public virtual TControl NewControl { get; set; }

            /// <summary>
            /// The 1-based index of the control in its parent.
            /// </summary>
            /// <remarks>This is suitable for a WinForms <see cref="System.Windows.Forms.Control.TabIndex"/> value.</remarks>
            public virtual int Index { get; set; }

            /// <summary>
            /// For consumers of this framework to add data to.
            /// </summary>
            public virtual IDictionary<string, object> CustomProperties { get; } = new Dictionary<string, object>();
        }

        public interface IGeneratableControl
        {
            /// <summary>
            /// Called by the form generator after construction, and running dependency injection if applicable.
            /// </summary>
            /// <param name="context"></param>
            void ControlGeneratation(ControlGeneratorContext context);
        }

    }

    [Flags]
    public enum AutoSizeOption
    {
        None = 0,
        /// <summary>
        /// Increasing the size is allowed.
        /// </summary>
        Grow = 1,
        /// <summary>
        /// Decreasing the size is allowed.
        /// </summary>
        Shrink = 2,
        GrowAndShrink = Grow | Shrink
    }
}
