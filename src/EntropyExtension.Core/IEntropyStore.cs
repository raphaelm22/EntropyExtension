namespace EntropyExtension.Core
{
    public interface IEntropyStore
    {
        void Log(EntropyLogInfo info);
        void EnsureCreated();
    }
}
