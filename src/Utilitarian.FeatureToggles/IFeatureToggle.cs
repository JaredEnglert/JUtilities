namespace Utilitarian.FeatureToggles
{
    public interface IFeatureToggle
    {
        string Name { get; }

        bool IsEnabled(object @object = null);
    }
}
