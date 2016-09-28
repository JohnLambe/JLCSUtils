using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JohnLambe.Util;

namespace AutoFormDemo
{
    public class AutoFormGenerator
    {
        public virtual Form Target { get; set; }

        public virtual object Model { get; set; }

        /// <summary>
        /// Coordinates (top left) of first control caption.
        /// </summary>
        public virtual Point Coords { get; set; }

        /// <summary>
        /// X-coordinate 
        /// </summary>
        public virtual int FieldControlLeft { get; set; } = 200;

        //TODO: Separate mappings generation to separate class? It could be independent of the type of UI.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public virtual void GenerateMappings(int start = 0, int end = -1)
        {
            _mappings.Clear();

            foreach(var property in Model.GetType().GetProperties())
            {
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
            }
        }

        public virtual void Generate()
        {
            int yCoord = Coords.Y;

            foreach (var mapping in _mappings)
            //TODO: filter by Priority range
            {
                //TODO: Refactor this with a control (could be a subclass of Panel etc.) that combines the Label and Field (data) control,
                // and also handles validation (populates validation properties on Field Control,
                // and attaches an event handler for other validation).

                var captionControl = new Label();
                captionControl.Text = mapping.Caption;
                captionControl.Left = Coords.X;
                captionControl.Top = yCoord;
                Target.Controls.Add(captionControl);

                //TODO: Tab stops

                //TODO: delegate to interface; implementation per control type (DataType mapped to control type; could be overridden by attribute).
                // e.g. bool mapped to CheckBox.
                var dataControl = new TextBox();
                dataControl.Left = FieldControlLeft;
                dataControl.Top = yCoord;
                dataControl.Text = (mapping.Property.GetValue(Model) ?? "").ToString();
                dataControl.Tag = mapping;   // we could assign it to an object that has this as a property, so that more details can be held in Tag in future

                Target.Controls.Add(dataControl);

                yCoord += captionControl.Height + VSpacing;
            }
        }

        protected const int VSpacing = 4;

        protected IList<PropertyMapping> _mappings = new List<PropertyMapping>();
    }


    /// <summary>
    /// Details of a property mapped to a UI control.
    /// This holds only Model (not View) details.
    /// </summary>
    public class PropertyMapping
    {
        public virtual string Caption { get; set; }

        public virtual Type DataType { get; set; }

        public virtual PropertyInfo Property { get; set; }

        public virtual int Priority { get; set; }

        //TODO: Add validation information
        public virtual int? MaximumLength { get; set; }
        public virtual int? MaximumValue { get; set; }
        public virtual int? MinimumValue { get; set; }
        // Regex validation  ?
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class UiAttribute : Attribute
    {
        public virtual int Priority { get; set; }

        //TODO
        // Regex validation ? 
    }
}
