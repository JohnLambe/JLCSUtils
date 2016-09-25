using System;

namespace MvpFramework.Binding
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class MvpHandlerAttribute : System.Attribute
    {
        public string Name { get; set; }

        public bool Enabled { get; set; }
    }
}