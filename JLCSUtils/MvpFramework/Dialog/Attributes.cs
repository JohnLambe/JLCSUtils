using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Dialog
{
    /// <summary>
    /// Register a message dialog type to be available for configuration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RegisterMessageDialogAttribute : MvpClassAttribute
    {
        public virtual string[] Filter { get; set; }

        public virtual string Category { get; set; }
    }
}
