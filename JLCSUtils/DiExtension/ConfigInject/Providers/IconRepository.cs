using DiExtension.ConfigInject.Providers;
using JohnLambe.Util.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiExtension.ConfigInject;
using System.Reflection;

namespace JohnLambe.Util.ConfigProvider
{
    /// <summary>
    /// Icon repository with a cache, that loads from directories and/or a resources from a list of assemblies.
    /// </summary>
    /// <typeparam name="TImage"></typeparam>
    public class IconRepository<TImage> : IconRepositoryConfigProvider<TImage>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceAssemblies"></param>
        /// <param name="directory">Directory to load image from. null to not use.</param>
        public IconRepository(Assembly[] resourceAssemblies, string[] directories = null) : base(null)
        {
            var chain = new ConfigProviderChain();

            if(directories != null)
                foreach (string directory in directories)
                    chain.RegisterProvider(new FileImageLoaderConfigProvider(directory));

            if(resourceAssemblies != null)
                foreach (var assm in resourceAssemblies)
                    chain.RegisterProvider(new ResourceConfigProvider(assm));

            Provider = new CacheConfigProvider(chain);
        }

     //   protected virtual string ImageDirectory { get; set; }

    //    protected virtual Assembly[] ResourceAssemblies { get; set; }
    }

        
}
