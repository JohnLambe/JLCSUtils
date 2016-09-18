// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////

using JohnLambe.Util.DependencyInjection.ConfigInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using JohnLambe.Util.FilterDelegates;
using JohnLambe.Util.Encoding;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.PluginFramework.Attributes;
using JohnLambe.Util.PluginFramework.Interfaces;

namespace JohnLambe.Util.DependencyInjection
{
    public class DiContextConfigurer
    {
        public DiContextConfigurer(IExtendedDiContext diContext)
        {
            this.DiContext = diContext;
        }

        /// <summary>
        /// Scan an assembly for ConfigInject providers (identified by attributes) and register them.
        /// </summary>
        /// <param name="assm"></param>
        public virtual void ScanAssembly(Assembly assm, BooleanExpression<Type> filter)
        {
            ScanForProviders(assm);

            ScanForClasses(assm, filter);
        }

        /// <summary>
        /// Generates the string used to sort classes to be registered.
        /// The order is:
        /// - Priority (from DiRegisterAttributeBase)
        /// - Full class name
        /// - Index of the attribute causing the registraton.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attribute"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected string MakeSortKey(Type type, DiRegisterAttributeBase attribute, ushort index)
        {
            return SortStringCalculator.IntToSortString(attribute.Priority) + type.FullName
                + " " + SortStringCalculator.UInt16ToSortString(index);
        }

        protected struct RegistrationItem
        {
            public Type Class;
            public DiRegisterAttributeBase Attribute;
        }

        public virtual void ScanForClasses(Assembly assm, BooleanExpression<Type> filter = null)
        {
            var toBeRegistered = new SortedList<string, RegistrationItem>();

            // find all types to be registered:
            foreach (var type in assm.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
            {
                int attributeIndex = 0;
                foreach (var attribute in type.GetCustomAttributes<DiRegisterAttributeBase>())
                {
                    if (filter.TryEvaluate(type))
                    {
                        toBeRegistered.Add(MakeSortKey(type, attribute, (ushort)attributeIndex++), 
                            new RegistrationItem { Attribute = attribute, Class = type });
                    }
                }
            }

            // register them in order:
            foreach (var item in toBeRegistered.Values)
            {
                if (item.Attribute is DiRegisterTypeAttribute)
                {
                    if (item.Attribute.ForType != null)
                    {
                        if (item.Attribute.Name != null)
                            throw new Exception("DiRegisterTypeAttribute: Specifying both Name and ForType is invalid");
                        DiContext.RegisterType(item.Attribute.ForType, item.Class);
                    }
                    else
                    {
                        DiContext.RegisterType(item.Class, item.Attribute.Name);
                    }
                }
                else if (item.Attribute is DiRegisterInstanceAttribute)
                {
                    var instance = item.Class.Create<object>();
                    DiContext.RegisterInstance(item.Attribute.Name, instance);
                }
/*
                var containerRegAttribute = item.Class.GetCustomAttribute<ContainerRegisterAttribute>();
                if(containerRegAttribute != null)
                {
                    IPluginContainer container = GetContainerById(containerRegAttribute.ContainerId);
                    container.RegisterPlugin(item.);
                    containerRegAttribute.
                }
                */
            }
        }

        /*
        protected virtual IPluginContainer GetContainerById()
        {

        }
        */

        public virtual void ScanForProviders(Assembly assm)
        {
            foreach (var type in assm.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
            {
                var attribute = type.GetCustomAttribute<RegisterProviderAttribute>();
                if (attribute != null)
                {
                    DiContext.ProviderChain.RegisterProvider(type as IConfigProvider, attribute.Priority);
                }
            }
        }

        public virtual IExtendedDiContext DiContext { get; set; }
    }
}
