using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.PluginFramework.Attributes
{
    /// <summary>
    /// Base class for all PluginFramework attributes.
    /// </summary>
    public abstract class PluginAttributeBase : Attribute
    {
    }


    /// <summary>
    /// Identifies a class as a plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited=false)]
    public class PluginAttribute : PluginAttributeBase
    {
        public PluginAttribute(string id = null, int priority = 0)
        {
            this.Id = id;
            this.Priority = priority;
        }

        /// <summary>
        /// Unique ID of the plugin.
        /// </summary>
        public string Id { get; set; } //Guid?
        //  Default to fully qualified class name?
        //  Default to Class Guid ??
        // IPlugin.Id instead ?

        /// <summary>
        /// Determines loading/initialisation order.
        /// </summary>
        public virtual int Priority { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool InitialiseOnStart { get; set; }
        // Use container registration instead *?*
    }

    /// <summary>
    /// Declares a dependency on another plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PluginRequirementAttribute : PluginAttributeBase
    {
        public PluginRequirementAttribute(string pluginId, string version = null,
            string name = null, RequirementType requirement = RequirementType.Installed)
        {
            PluginId = pluginId;
            Version = version;
            Name = name;
            Requirement = requirement;
        }

        /// <summary>
        /// The ID of the required plugin.
        /// </summary>
        public virtual string PluginId { get; set; }
        // The Plugin Host will have an ID (for declaring the minimum required version). Name and Requirement will not be used in this case.

        /// <summary>
        /// Minimum required version.
        /// </summary>
        public virtual string Version { get; set; }

        /// <summary>
        /// Name of the required plugin, for display in error message
        /// if the plugin is not available.
        /// </summary>
        public virtual string Name { get; set; }

        public virtual RequirementType Requirement { get; set; }
    }

    /*
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginContainerAttribute : PluginAttribute  //??
    {

    }
    */

    /// <summary>
    /// Registers a plugin with a container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public class ContainerRegisterAttribute : PluginAttributeBase
    {
        public ContainerRegisterAttribute(string containerId, int priority = 0)
        {
            this.ContainerId = containerId;
            this.Priority = priority;
        }

        /// <summary>
        /// The container to register with.
        /// </summary>
        public virtual string ContainerId { get; set; }

        /// <summary>
        /// Determines the order of receiving events.
        /// Iff 0, the Priority of the plugin (from PluginAttribute) is used.
        /// </summary>
        public virtual int Priority { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Instance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool InitialiseOnStart { get; set; }  //?
    }

    /// <summary>
    /// Assigns a category to a plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PluginCategoryAttribute : PluginAttributeBase
    {
        public PluginCategoryAttribute(Type category)
        {
            this.Category = category;
        }

        /// <summary>
        /// The type of the interface of the category.
        /// </summary>
        public virtual Type Category { get; set; }
        //        public virtual string CategoryId { get; set; }
    }

    /// <summary>
    /// Indicates at what stage a required plugin is needed.
    /// </summary>
    [Flags]
    public enum RequirementType
    {
        None = 0,
        /// <summary>
        /// The specified plugin must be installed before the one declaring the requirement can be loaded/initialised.
        /// </summary>
        Installed = 1,
        /// <summary>
        /// The specified plugin must be initialised before the one declaring the requirement can be initialised.
        /// </summary>
        Initialised = 2
    }
}
