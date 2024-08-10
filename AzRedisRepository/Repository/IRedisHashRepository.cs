namespace AzRedisRepository.Repository
{
    public interface IRedisHashRepository<T>
    {
        Task StoreAsync(string key, T entity);
        Task<T> GetAsync(string key);
        Task<List<T>> GetAllAsync(string pattern);
        Task RemoveHashAsync(string key);
        Task RemoveAllAsync(string pattern);
    }

}
