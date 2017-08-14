﻿using DiExtension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    /// <summary>
    /// Base class for injection attributes of the MVP framework.
    /// </summary>
    public abstract class MvpInjectAttribute : InjectAttribute
    {        
    }

    /// <summary>
    /// Flags a parameter to be injected by the MVP framework.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class MvpParamAttribute : MvpClassAttribute
    {
    }

    /// <summary>
    /// Flags that a Presenter being injected should get a View nested in the View of the Presenter
    /// that it is being injected into.
    /// Currently supported only on constructor injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class MvpNestedAttribute : MvpInjectAttribute
    {
        /// <summary>
        /// </summary>
        /// <param name="nestedViewId"><see cref="NestedViewId"/></param>
        public MvpNestedAttribute(string nestedViewId = null)
        {
            this.NestedViewId = nestedViewId;
        }

        /// <summary>
        /// <inheritdoc cref="INestedPresenterFactory.NestedViewId"/>
        /// </summary>
        public string NestedViewId { get; set; }
    }
}
