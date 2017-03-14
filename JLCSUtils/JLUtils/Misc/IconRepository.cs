using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
{
    /*
    public abstract class IconRepositoryBase<TKeyType, TImageType> : IIconRepository<TKeyType, TImageType>
    {
        public virtual TImageType GetIcon(TKeyType iconId, IconState state = IconState.Normal)
        {
            return LoadIcon(KeyString(iconId, state));
        }

        protected abstract TImageType LoadIcon(string iconKey);

        protected virtual string KeyString(TKeyType iconId, IconState state = IconState.Normal)
        {
            if (iconId == null)
                return null;
            return iconId.ToString() + "-" + state.ToString();
        }
    }


    public class FileIconRepository<TKeyType, TImageType> : IconRepositoryBase<TKeyType, TImageType>
    {
        public FileIconRepository(string directory)
        {
            this.Directory = directory;
        }

        protected virtual TImageType LoadIcon(string iconId)
        {

        }

        protected readonly string Directory;
    }


    public class ResourceIconRepository<TKeyType, TImageType> : IconRepositoryBase<TKeyType, TImageType>
    {
        protected virtual TImageType LoadIcon(string iconId)
        {

        }
    }


    public class IconRepositoryChain<TKeyType, TImageType> : IconRepositoryBase<TKeyType, TImageType>
    {
        public virtual TImageType GetIcon(TKeyType iconId, IconState state = IconState.Normal)
        {

        }


    }


    public class CachedIconRepository<TKeyType, TImageType> : IIconRepository<TKeyType, TImageType>
    {
        public CachedIconRepository(IIconRepository<TKeyType, TImageType> internalRepository)
        {
            this.InternalRepository = internalRepository;
        }

        public virtual TImageType GetIcon(TKeyType iconId, IconState state = IconState.Normal)
        {
            TImageType result;
            if(Cache.TryGetValue(iconId, out result))
            {
                return result;
            }
            result = InternalRepository.GetIcon(iconId, state);
            Cache[iconId] = result;
            return result;
        }

        protected readonly IIconRepository<TKeyType, TImageType> InternalRepository;
        protected readonly IDictionary<TKeyType, TImageType> Cache = new Dictionary<TKeyType,TImageType>();
    }
    */
}
