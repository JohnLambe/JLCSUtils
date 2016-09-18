using JohnLambe.Util.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.PluginFramework.Interfaces
{
    /// <summary>
    /// Provides details of a plugin, visible to other plugins.
    /// </summary>
    public interface IPluginDetails
    {
        /// <summary>
        /// Globally unique ID of this type of plugin.
        /// </summary>
        string ClassId { get; }

        /// <summary>
        /// Unique ID of this plugin instance.
        /// null if this is reference to a plugin type only.
        /// </summary>
        string InstanceId { get; }

        string Name { get; }

        /// <summary>
        /// Human-readable name.
        /// </summary>
        string DisplayName { get; }

        Version Version { get; }

        /// <summary>
        /// All categories that this plugin is in.
        /// May be empty, but never null.
        /// </summary>
        IReadOnlyCollection<IPluginCategory> Categories { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <returns>true if the plugin is in the given category.</returns>
        bool IsInCategory(IPluginCategory category);

        /// <summary>
        /// All child plugins of this one - plugins registered in this container.
        /// May be empty, but never null.
        /// </summary>
        IReadOnlyCollection<IPluginDetails> Children { get; }

        /// <summary>
        /// True if this plugin is a container - i.e. if it can contain other plugins, whether it currently does or not.
        /// </summary>
        bool IsContainer { get; }

        /*
        /// <summary>
        /// The plugin instance.
        /// null if this is just a reference to a plugin type.
        /// </summary>
        object Plugin { get; }
        // return IPlugin ?
         * 
        object GetInterface(Type interfaceType);  ??
         */

    }

    public static class PluginDetailsConstants
    {
        /// <summary>
        /// Passed to IEventSender to send an event to all siblings of the sending plugin.
        /// </summary>
        public static readonly IPluginDetails Local = new PluginDetailsConstant("<Local>");

        /// <summary>
        /// Passed to IEventSender to send an event to all children of the sending plugin.
        /// </summary>
        public static readonly IPluginDetails Children = new PluginDetailsConstant("<Children>");

        public static readonly IReadOnlyCollection<IPluginCategory> EmptyCategoryList 
            = new EmptyCollection<IPluginCategory>();

        public static readonly IReadOnlyCollection<IPluginDetails> EmptyPluginList
            = new EmptyCollection<IPluginDetails>();

        /// <summary>
        /// ID used to refer to the plugin host.
        /// </summary>
        public const string PluginHostId = "<Host>";
    }

    public class PluginDetailsConstant : IPluginDetails
    {
        public PluginDetailsConstant(string name)
        {
            this.Name = name;
        }

        public string ClassId
        {
            get { return Name; }
        }

        public string InstanceId
        {
            get { return Name; }
        }

        public string Name
        {
            get;
            protected set;
        }

        public string DisplayName
        {
            get { return Name; }
        }

        public Version Version
        {
            get { throw new NotImplementedException(); }
        }

        public IReadOnlyCollection<IPluginCategory> Categories
        {
            get { return PluginDetailsConstants.EmptyCategoryList; }
        }

        public bool IsInCategory(IPluginCategory category)
        {
            return false;
        }

        public IReadOnlyCollection<IPluginDetails> Children
        {
            get { return PluginDetailsConstants.EmptyPluginList; }
        }

        public bool IsContainer
        {
            get { return false; }
        }
    }
}
