using System;

namespace MvpFramework.Binding
{
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
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Sorting order in a list of handlers.
        /// </summary>
        public int Order { get; set; }

        public string[] Filter { get; set; }


        // Details for generating a UI item:

        public bool AutoGenerate { get; set; } = false;

        public string DisplayName { get; set; }
        public KeyboardKey HotKey { get; set; }  
        public char AcceleratorChar { get; set; }
        public string IconId { get; set; }
        public bool IsDefault { get; set; }
    }
}