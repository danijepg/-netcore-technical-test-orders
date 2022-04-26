using Infrastructure.Messaging;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class MessagingDependencyInjectionConfiguration
    {
        public static IServiceCollection AddMessaging(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var messagingConfig = configuration.GetSection("messaging").Get<MessagingOptions>((a) => a.BindNonPublicProperties = true);

            services.AddMassTransit(configurator =>
            {
                configurator.UsingRabbitMq((context, config) =>
                {
                    config.Host(messagingConfig.Host);
                });
            });

            return (services);
        }
    }
}
