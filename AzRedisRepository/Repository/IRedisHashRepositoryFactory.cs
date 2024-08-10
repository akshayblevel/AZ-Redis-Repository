namespace AzRedisRepository.Repository
{
    public interface IRedisHashRepositoryFactory
    {
        IRedisHashRepository<T> CreateRepository<T>();
    }

}
