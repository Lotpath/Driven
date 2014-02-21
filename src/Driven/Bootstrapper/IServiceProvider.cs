namespace Driven.Bootstrapper
{
    public interface IServiceProvider
    {
        TService Resolve<TService>() where TService : class;
    }
}