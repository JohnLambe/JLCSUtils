using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection
{
    public class AssemblyUtil
    {
        /// <summary>
        /// Returns all assemblies directly referenced by the given assembly.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="includeSource">Iff true, the given assembly is included.</param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetReferencedAssemblies(Assembly source = null, bool includeSource = false)
        {
            if (source == null)
                source = Assembly.GetCallingAssembly();

            var referencedAssemblies = source.GetReferencedAssemblies();  // names of referenced assemblies

            // Filter the list of all assemblies to those that match the referenced assemblies:
            return AppDomain.CurrentDomain.GetAssemblies().Where(a =>
                   referencedAssemblies.FirstOrDefault(an => an.FullName.Equals(a.FullName)) != null
                   || (includeSource && a.Equals(source))    // conditionally include the original assembly
                   );
        }
    }
}
