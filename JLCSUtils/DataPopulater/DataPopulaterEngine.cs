using JohnLambe.Util.Reflection;
using JohnLambe.Util.Services;
using JohnLambe.Util.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.DataPopulater
{
    public class DataPopulaterEngine
    {

        public virtual void Run()
        {
            foreach(var cls in Config.Classes)
            {
                ProcessClass(cls);
            }

        }

        public virtual void ProcessClass(ClassConfig clsConfig)
        {
            var cls = clsConfig.TargetClass;

            int count = RandomService.Next(clsConfig.MinimumInstances, clsConfig.MaximumInstances);

            for (int i = 0; i < count; i++)
            {
                var newInstance = CreateInstance(clsConfig);

                foreach (var propConfig in clsConfig.Properties)
                {
                    propConfig.Parent = clsConfig; //TODO: Assign once only

                    ProcessProperty(cls, propConfig, newInstance);
                }

                SaveInstance(clsConfig, newInstance);
            }

        }

        /// <summary>
        /// Returns a new instance compatible with the given configuration.
        /// </summary>
        /// <param name="clsConfig"></param>
        /// <returns></returns>
        protected virtual object CreateInstance(ClassConfig clsConfig)
        {
            return clsConfig.NewInstance();
        }

        /// <summary>
        /// Called with each new instance, when it is fully populated.
        /// This saves the instance to persistent storage, or whatever is to be done with the generated data.
        /// </summary>
        /// <param name="clsConfig"></param>
        /// <param name="instance">the instance to be saved.</param>
        protected virtual void SaveInstance(ClassConfig clsConfig, object instance)
        {
            OnSaveInstnace?.Invoke(this, new SaveInstanceArgs() { Config = clsConfig, Instance = instance });
        }

        public virtual event EventHandler<SaveInstanceArgs> OnSaveInstnace;

        /// <summary>
        /// Arguments for the <see cref="OnSaveInstnace"/> event.
        /// </summary>
        public class SaveInstanceArgs : EventArgs
        {
            /// <summary>
            /// The configuration that was generated from.
            /// </summary>
            public ClassConfig Config { get; set; }

            /// <summary>
            /// The new instance to be saved.
            /// </summary>
            public object Instance { get; set; }
        }

        /// <summary>
        /// Assign the given property on the given instance to a generated value.
        /// </summary>
        /// <param name="cls">Type of the instance.</param>
        /// <param name="propConfig"></param>
        /// <param name="instance"></param>
        public virtual void ProcessProperty(Type cls, PropertyConfigBase propConfig, object instance)
        {
            propConfig.RandomService = RandomService; //TODO: Assign once only
            
            var context = CreateContext();
            object directInstance = instance;
            context.RequiredType = ReflectionUtil.GetProperty(ref directInstance, propConfig.PropertyName).PropertyType;

            //var prop = propConfig.Property;
            var value = GeneralTypeConverter.Convert<object>(propConfig.GenerateValue(context));
            if(value != PropertyConfigBase.NoValue)
                ReflectionUtil.TrySetPropertyValue(instance, propConfig.PropertyName, value);
            //prop.SetValue(instance, GeneralTypeConverter.Convert(propConfig.GenerateValue(context), prop.PropertyType));

        }

        protected virtual IPropertyPopulaterContext CreateContext()
        {
            return new PropertyPopulaterContext();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual DataPopulaterConfig Config { get; set; }
        public virtual IRandomService RandomService { get; set; } = new RandomService();

    }

    public class PropertyPopulaterContext : IPropertyPopulaterContext
    {
        /// <summary>
        /// The type to be returned (generated).
        /// </summary>
        public virtual Type RequiredType { get; set; }

        public virtual object GetRandomInstance(Type type)
        {
            return null;
        }
    }

}
