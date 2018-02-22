using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection
{
    /// <summary>
    /// Utilities for working with Assemblies.
    /// </summary>
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

        /// <summary>
        /// Returns the loaded assembly with the given name.
        /// </summary>
        /// <param name="assemblySimpleName"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">If no assembly with the given name is loaded.</exception>
        public static Assembly GetLoadedAssemblyByName(string assemblySimpleName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name == assemblySimpleName).First();
        }
    }
}
