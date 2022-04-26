using Core.Persistence;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace Infrastructure.DependencyInjection
{
    public static class PersistenceDependencyInjectionConfiguration
    {
        public static IServiceCollection AddInfrastructureRepositories(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var config = configuration.GetSection("postgresql").Get<PostgreSqlOptions>((a) => a.BindNonPublicProperties = true);

            services.AddTransient<IDbConnection>(_ => new NpgsqlConnection(config.ConnectionString));

            services.AddSingleton<IOrderAggregateRootRepository, OrdersAggregateRootRepository>();

            return (services);
        }
    }
}
