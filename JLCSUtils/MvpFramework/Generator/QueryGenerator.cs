using JohnLambe.Util.Reflection;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Generator
{
    public class QueryGenerator
    {
        public virtual IQueryable CreateQuery<T>(IQueryable<T> source, string filter, QueryGeneratorOptions options)
        {
            return new QueryBuilder<T>(source, filter, options).Queryable();
        }


        public class QueryBuilder<T>
        {
            public QueryBuilder(IQueryable<T> source, string filter, QueryGeneratorOptions options)
            {
                Source = source;
                Filter = filter;
                Options = options;
            }

            protected readonly IQueryable<T> Source;
            protected readonly string Filter;
            protected readonly QueryGeneratorOptions Options;

            public IQueryable Queryable()
            {
                if(Options.HasFlag(QueryGeneratorOptions.Filter))
                {
                    IQueryable<T> query = Source;
                    var attribs = AttributeUtil.GetMemberAttributes<PropertyInfo,ColumnFilterAttribute>(typeof(T)); //TODO: Filter
                    foreach(var attribute in attribs)
                    {
                        query = query.Where(MakeFilterDelegate(attribute.Member,attribute.Attribute)).AsQueryable<T>();
                    }
                }

                if (Options.HasFlag(QueryGeneratorOptions.Sort))
                {

                }

                //            typeof(T).GetProperties().W;

                //            Func<bool,T> f = x => x.
                return
                Source.Where(MakeFilterDelegate("a", "b"))
                    .OrderBy(x => x, CreateComparer<T>())
                    .Select(x => Project(x))
                    .Select(x => new
                    {
                        a = 1,
                        b = x
                    }).AsQueryable()
                    ;

            }


            public virtual Func<T, bool> MakeFilterDelegate(string propertyName, object value)
            {
                PropertyInfo property = typeof(T).GetProperty(propertyName);
                return x => property.GetValue(x).Equals(value);
            }

            public virtual Func<T, bool> MakeFilterDelegate(PropertyInfo property, ColumnFilterAttribute attrib)
            {
                return x =>
                {
                    var value = property.GetValue(x) as IComparable;
                    return value.CompareTo(attrib.MinimumValue) >= 0 && value.CompareTo(attrib.MaximumValue) <= 0;
                };
            }

            public virtual dynamic Project(T source)
            {
                dynamic x = new { a = 1 };
                return x;
            }

            public virtual IComparer<TKey> CreateComparer<TKey>()
            {
                return null;
            }

            public class Projection : DynamicObject
            {
                public override bool TryGetMember(GetMemberBinder binder, out object result)
                {
                    return base.TryGetMember(binder, out result);
                }
            }

            /*
            public virtual Func<T, bool> MakeOrderDelegate<T>(string propertyName, object value)
            {
            }
            */
        }

        [Flags]
        public enum QueryGeneratorOptions
        {
            Project = 1,
            Filter = 2,
            Sort = 4
        }
    }
}
