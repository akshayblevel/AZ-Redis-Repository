using StackExchange.Redis;
using System.Reflection;

namespace AzRedisRepository.Repository
{
    public class RedisHashRepository<T> : IRedisHashRepository<T>
    {
        private readonly IDatabase _database;

        public RedisHashRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
        }

        public async Task StoreAsync(string key, T entity)
        {
            var hashEntries = GetHashEntries(entity);
            await _database.HashSetAsync(key, hashEntries);
        }

        public async Task<T> GetAsync(string key)
        {
            var hashEntries = await _database.HashGetAllAsync(key);
            if (hashEntries.Length == 0)
                return default;

            return MapToEntity(hashEntries);
        }

        public async Task<List<T>> GetAllAsync(string pattern)
        {
            var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern).ToArray();
            var result = new List<T>();

            foreach (var key in keys)
            {
                var hashEntries = await _database.HashGetAllAsync(key);
                if (hashEntries.Length > 0)
                {
                    var entity = MapToEntity(hashEntries);
                    result.Add(entity);
                }
            }

            return result;
        }

        public async Task RemoveHashAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }


        public async Task RemoveAllAsync(string pattern)
        {
            var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern).ToArray();

            foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
            }
        }

        private HashEntry[] GetHashEntries(T entity)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return properties.Select(p => new HashEntry(p.Name, p.GetValue(entity)?.ToString())).ToArray();
        }

        private T MapToEntity(HashEntry[] hashEntries)
        {
            var entity = Activator.CreateInstance<T>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var entry in hashEntries)
            {
                var property = properties.FirstOrDefault(p => p.Name == entry.Name);
                if (property != null)
                {
                    object value = entry.Value;
                    if (property.PropertyType == typeof(DateTime))
                    {
                        if (DateTime.TryParse(value.ToString(), out var dateTimeValue))
                        {
                            value = dateTimeValue;
                        }
                        else
                        {
                            throw new InvalidCastException($"Cannot convert '{entry.Value}' to DateTime.");
                        }
                    }
                    else if (property.PropertyType == typeof(DateTime?))
                    {
                        if (DateTime.TryParse(value.ToString(), out var nullableDateTimeValue))
                        {
                            value = nullableDateTimeValue;
                        }
                        else
                        {
                            value = null;
                        }
                    }
                    else
                    {
                        value = Convert.ChangeType(entry.Value, property.PropertyType);
                    }

                    property.SetValue(entity, value);
                }
            }

            return entity;
        }
    }

}
