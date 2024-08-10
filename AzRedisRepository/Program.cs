using AzRedisRepository.Repository;
using AzRedisRepository;
using StackExchange.Redis;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var redisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value;
var multiplexer = ConnectionMultiplexer.Connect(redisConnectionString);


builder.Services.AddSingleton<IRedisHashRepositoryFactory>(sp =>
{
    var connectionMultiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
    return new RedisHashRepositoryFactory(connectionMultiplexer);
});

builder.Services.AddTransient<IRedisHashRepository<WeatherForecast>>(sp =>
{
    var factory = sp.GetRequiredService<IRedisHashRepositoryFactory>();
    return factory.CreateRepository<WeatherForecast>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
