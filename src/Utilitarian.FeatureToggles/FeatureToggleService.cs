using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Utilitarian.FeatureToggles
{
    public interface IFeatureToggleService
    {
        bool IsEnabled(string featureToggleName, object @object = null);
    }

    public class FeatureToggleService : IFeatureToggleService
    {
        private readonly ConcurrentDictionary<string, IFeatureToggle> _featureToggles;

        public FeatureToggleService(IEnumerable<IFeatureToggle> featureToggles)
        {
            _featureToggles = new ConcurrentDictionary<string, IFeatureToggle>(featureToggles.ToDictionary(featureToggle => featureToggle.Name));
        }

        public bool IsEnabled(string featureToggleName, object @object = null)
        {
            return !string.IsNullOrWhiteSpace(featureToggleName) && _featureToggles.ContainsKey(featureToggleName) 
                ? _featureToggles[featureToggleName].IsEnabled(@object)
                : false;
        }
    }
}
