namespace Common.DI;

public interface IDIContainer
{
    void AddService<TService>(TService service);
    bool RemoveService<TService>();

    TService GetService<TService>();

}
