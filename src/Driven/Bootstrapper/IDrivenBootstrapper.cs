namespace Driven.Bootstrapper
{
    public interface IDrivenBootstrapper
    {
        void Initialize();
        IDrivenEngine GetEngine();
    }
}