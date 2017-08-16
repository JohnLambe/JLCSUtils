using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MvpFramework.Binding;
using System.Reflection;
using JohnLambe.Util.Types;

namespace MvpFramework.Generator
{
    public class ModelQuery
    {
        /// <summary>
        /// Apply a query defined on the class of the elements, to the given collection.
        /// </summary>
        /// <param name="all">All items of the specified type.</param>
        /// <param name="modelType"></param>
        /// <param name="id">Specifies which query to use (for models with multiple <see cref="ListGeneratorAttribute"/> methods.</param>
        /// <param name="ignoreNotFound">true to do nothing and return <paramref name="all"/> if the requested query (method) is not found.</param>
        /// <returns>The input with the specified </returns>
        /// <exception cref="KeyNotFoundException">If the requested query does not exist.</exception>
        [return: Nullable]
        public static IQueryable ApplyQuery<TModel>(IQueryable<TModel> all, [Nullable] Type modelType = null, [Nullable] string id = null, bool ignoreNotFound = false)
        {
            if (modelType == null)
                modelType = typeof(TModel);
            MethodInfo method = modelType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).FirstOrDefault(
                m =>
                {
                    var attribute = m.GetCustomAttribute<ListGeneratorAttribute>(false);
                    return (attribute?.Enabled ?? false) && (id == null || attribute.Id == id);
                });

            if (method == null)
            {
                if(ignoreNotFound)
                    return all;         // unmodified
                else
                    throw new KeyNotFoundException("List generator not found on " + modelType.FullName + " (" + (id ?? "") + ")");
            }

            return (IQueryable) method.Invoke(null, new object[] { all });
        }

        /// <summary>
        /// Same as <see cref="ApplyQuery{TModel}(IQueryable{TModel}, Type, string)"/> except that this returns <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="all"></param>
        /// <param name="modelType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IQueryable<TModel> ApplyQueryT<TModel>(IQueryable<TModel> all, Type modelType = null, string id = null)
        {
            return (IQueryable<TModel>) ApplyQuery(all,modelType,id);
        }

        //TODO: Inject additional parameters from DI container.
        //  Choose between multiple ListGeneratorAttribute methods.
        //  Apply modifier methods.
    }
}