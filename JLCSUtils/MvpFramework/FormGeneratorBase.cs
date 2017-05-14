using JohnLambe.Types;
using JohnLambe.Util.FilterDelegates;
using JohnLambe.Util.Reflection;
using MvpFramework.Binding;
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
        /// <summary>
        /// The user interface control into which the generated controls will be added.
        /// </summary>
        public virtual TControl Target { get; set; }

        /// <summary>
        /// The model from which to generate a user interface.
        /// </summary>
        public virtual ModelBinderWrapper Model { get; set; }

        public virtual AutoSizeOption AutoSize { get; set; } = AutoSizeOption.Grow;

        /// <summary>
        /// Coordinates (top left) of first control caption.
        /// </summary>
//        public virtual Point Coords { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public virtual void Generate(string groupId = null, FilterDelegate<IUiGroupModel> groupFilter = null, FilterDelegate<ModelPropertyBinder> propertyFilter = null)
        {
            if (Target != null && Model != null)
            {
                GenerateGroup(GroupDefinitionAttribute.Ungrouped, propertyFilter);

                foreach (var group in Model.Groups.Where(g => groupFilter == null || groupFilter(g)))
                {
                    GenerateGroup(group, propertyFilter);
                }
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


        public virtual void StartGenerate()
        {
        }

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
        /// <param name="group"></param>
        /// <param name="propertyBinder"></param>
        public virtual TControl CreateControl([NotNull]ControlGeneratorContext context)
        {
            BeforeCreateControl(context);

            var controlType = GetControlForType(context.PropertyBinder.PropertyType);
            try
            {
                context.NewControl = ReflectionUtil.CallStaticMethod<TControl>(controlType, MvpControlMappingAttribute.CreateControlMethod, context);
            }
            catch (MissingMemberException)
            {
                context.NewControl = ReflectionUtil.Create<TControl>(controlType);
            }

            AfterCreateControl(context);

            return null;
        }

        protected virtual void BeforeCreateControl([NotNull]ControlGeneratorContext context)
        {
        }

        protected virtual void AfterCreateControl([NotNull]ControlGeneratorContext context)
        {
        }

        /// <summary>
        /// Get a UI control that handles a given data type.
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        protected virtual Type GetControlForType(Type dataType)
        {
            return null;
        }


        protected virtual AcceleratorCaptionUtil AccelCaptionUtil { get; } = new AcceleratorCaptionUtil();


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

            public virtual TControl NewControl { get; set; }

            public virtual int Index { get; set; }

            public virtual IDictionary<string, object> CustomProperties { get; } = new Dictionary<string, object>();
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
