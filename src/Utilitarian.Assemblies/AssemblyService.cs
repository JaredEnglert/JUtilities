using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilitarian.Assemblies
{
    public class AssemblyService : IAssemblyService
    {
        public IEnumerable<T> GetAllImplementations<T>()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(T)))
                .Select(implementation => (T)Activator.CreateInstance(implementation));
        }
    }
}
