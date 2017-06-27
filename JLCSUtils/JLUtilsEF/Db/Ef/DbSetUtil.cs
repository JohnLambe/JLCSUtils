using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db.Ef
{
    public static class DbSetUtil
    {
        /// <summary>
        /// Add iems if they do not already exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        /// <param name="key">Delegate to extract a key from each entity.
        /// Each item is added if no entity exists with its value returned by this matching the new one.
        /// </param>
        /// <param name="items">The items to be added.</param>
        /// <returns>The same DbSet passed in.</returns>
        public static IDbSet<T> AddIfNotExists<T>(this IDbSet<T> set,
                Func<T, object> key,
                params T[] items)
            where T : class
        {
            foreach (var item in items)
            {
                var keyValue = key(item);
                if (set.Where(x => !key(x).Equals(keyValue)).Any())
//                    if (set.Where(x => !key(x).Equals(key(item))).Any())
                {
                    set.Add(item);
                }
            }
            return set;
        }

        public static IDbSet<T> AddIfNotExists<T>(this IDbSet<T> set,
                Func<IQueryable<T>,T, bool> exists,
                params T[] items)
            where T : class
        {
            foreach (var item in items)
            {
                if(!exists(set,item))
                //                if (set.Where(x => !key(x).Equals(key(item))).Any())
//                if (set.Where(exists).Any())
                {
                    set.Add(item);
                }
            }
            return set;
        }

        public static IDbSet<T> AddIfNotExists<T>(this IDbSet<T> set,
                IEqualityComparer<T> comparer,
                params T[] items)
            where T : class
        {
//            var existing = set.Intersect(items, comparer);
//            items.Except(existing, comparer);
            foreach (var item in items.Except(set))
            {
                set.Add(item);
            }
            return set;
        }

        //TODO: Provide the same for a repository interface
    }
}
