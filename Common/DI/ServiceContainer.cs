using System.Diagnostics.CodeAnalysis;

namespace Common.DI;
public class ServiceContainer : IDIContainer
{
    private readonly List<object> m_Services;

    public ServiceContainer(params object[] services)
    {
        m_Services = services.ToList() ?? new();
    }

    public void AddService<TService>([NotNull] TService service)
    {
        m_Services.Add(service);
    }

    public TService? GetService<TService>()
    {
        return (TService)m_Services.Find(x => x.GetType() == typeof(TService));
    }

    public bool RemoveService<TService>()
    {
        return m_Services.Remove(m_Services.Find(x => x.GetType() == typeof(TService)));
    }
}
