using StackExchange.Redis;

namespace AzRedisRepository.Repository
{
    public class RedisHashRepositoryFactory : IRedisHashRepositoryFactory
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisHashRepositoryFactory(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public IRedisHashRepository<T> CreateRepository<T>()
        {
            return new RedisHashRepository<T>(_connectionMultiplexer);
        }
    }

}
