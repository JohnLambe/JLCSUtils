using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.PluginFramework.Interfaces;
using JohnLambe.Util.Collections;

namespace JohnLambe.Util.PluginFramework.Host
{

    //    public partial class PluginHost
    //    {
    /// <summary>
    /// Details of a plugin held by the Host.
    /// </summary>
    public class PluginDetails : IPluginDetails
    {
        public PluginDetails(object plugin)
        {
            this.Plugin = plugin;
            PublicDetails = new PublicPluginDetails(this);
        }

        #region IPluginDetails members

        public virtual string ClassId { get; set; }

        public virtual string InstanceId { get; set; }

        public virtual string Name { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual Version Version { get; set; }

        public virtual IReadOnlyCollection<IPluginCategory> Categories
        {
            get;
            protected set;
        }

        public virtual bool IsInCategory(IPluginCategory category)
        {
            return Categories.Contains(category);
        }

        public virtual IReadOnlyCollection<IPluginDetails> Children { get; }
            = EmptyReadOnlyCollection<IPluginDetails>.Instance;

        public virtual bool IsContainer => false;

        #endregion

        #region Details for use by Host

        public virtual object Plugin
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Read-only object that gives access to only the <see cref="IPluginDetails"/> members,
        /// and references this object.
        /// </summary>
        public IPluginDetails PublicDetails { get; private set; }

        public class PublicPluginDetails : IPluginDetails
        {
            public PublicPluginDetails(PluginDetails details)
            {
                this.Details = details;
            }
            protected PluginDetails Details;

            public string ClassId
            {
                get
                {
                    return ((IPluginDetails)Details).ClassId;
                }
            }

            public string InstanceId
            {
                get
                {
                    return ((IPluginDetails)Details).InstanceId;
                }
            }

            public string Name
            {
                get
                {
                    return ((IPluginDetails)Details).Name;
                }
            }

            public string DisplayName
            {
                get
                {
                    return ((IPluginDetails)Details).DisplayName;
                }
            }

            public Version Version
            {
                get
                {
                    return ((IPluginDetails)Details).Version;
                }
            }

            public IReadOnlyCollection<IPluginCategory> Categories
            {
                get
                {
                    return ((IPluginDetails)Details).Categories;
                }
            }

            public IReadOnlyCollection<IPluginDetails> Children
            {
                get
                {
                    return ((IPluginDetails)Details).Children;
                }
            }

            public bool IsContainer
            {
                get
                {
                    return ((IPluginDetails)Details).IsContainer;
                }
            }

            public bool IsInCategory(IPluginCategory category)
            {
                return ((IPluginDetails)Details).IsInCategory(category);
            }
        }

    }
    //    }

}
