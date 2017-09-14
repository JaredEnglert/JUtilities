using System.Collections.Generic;

namespace Utilitarian.Assemblies
{
    public interface IAssemblyService
    {
        IEnumerable<T> GetAllImplementations<T>();
    }
}
