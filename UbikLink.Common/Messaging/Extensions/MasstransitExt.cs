using Azure.Messaging.ServiceBus;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

//TODO: change transport type (no need to websocket if no behind proxy with Azure Message Bus)
namespace UbikLink.Common.Messaging.Extensions
{
    public enum TransportType
    {
        AzureBus,
        RabbitMQ
    }

    public static class MasstransitExt
    {
        public static IServiceCollection AddMasstransitBackend<TDbContext>(this IServiceCollection services,
            string serviceNamePrefix,
            string connectionString,
            bool withUserTenantFilters,
            TransportType transport,
            Assembly currentAssembly,
            string transportUserIfRabbit = "",
            string transportPasswordIfRabbit = "") where TDbContext : DbContext
        {
            services.AddMassTransit(x =>
            {
                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(prefix: serviceNamePrefix, includeNamespace: false));

                switch (transport)
                {
                    case TransportType.AzureBus:
                        x.UsingAzureServiceBus((context, cfg) =>
                        {
                            cfg.Host(connectionString, h =>
                            {
                                h.TransportType = ServiceBusTransportType.AmqpWebSockets;
                            });

                            cfg.ConfigureEndpoints(context);

                            if (withUserTenantFilters)
                            {
                                cfg.UsePublishFilter(typeof(TenantAndUserIdsPublishFilter<>), context);
                                cfg.UseConsumeFilter(typeof(TenantAndUserIdsConsumeFilter<>), context);
                            }
                        });
                        break;
                    case TransportType.RabbitMQ:
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host(connectionString, h =>
                            {
                                h.Username(transportUserIfRabbit);
                                h.Password(transportPasswordIfRabbit);
                            });

                            cfg.ConfigureEndpoints(context);

                            if (withUserTenantFilters)
                            {
                                cfg.UsePublishFilter(typeof(TenantAndUserIdsPublishFilter<>), context);
                                cfg.UseConsumeFilter(typeof(TenantAndUserIdsConsumeFilter<>), context);
                            }
                        });
                        break;
                }

                //Inbox, outbox
                x.AddEntityFrameworkOutbox<TDbContext>(o =>
                {
                    o.UsePostgres();
                    o.UseBusOutbox();
                });

                //Add all consumers
                x.AddConsumers(currentAssembly);
            });

            return services;
        }

        public static IServiceCollection AddMasstransitFrontend(this IServiceCollection services,
            string serviceNamePrefix,
            string connectionString,
            bool withUserTenantFilters,
            TransportType transport,
            Assembly currentAssembly,
            string transportUserIfRabbit = "",
            string transportPasswordIfRabbit = "")
        {
            services.AddMassTransit(x =>
            {
                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(prefix: serviceNamePrefix, includeNamespace: false));
                
                switch (transport)
                {
                    case TransportType.AzureBus:
                        x.UsingAzureServiceBus((context, cfg) =>
                        {
                            cfg.Host(connectionString, h =>
                            {
                                h.TransportType = ServiceBusTransportType.AmqpWebSockets;
                            });

                            cfg.ConfigureEndpoints(context);

                            if (withUserTenantFilters)
                            {
                                cfg.UsePublishFilter(typeof(TenantAndUserIdsPublishFilter<>), context);
                                cfg.UseConsumeFilter(typeof(TenantAndUserIdsConsumeFilter<>), context);
                            }
                        });
                        break;
                    case TransportType.RabbitMQ:
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host(connectionString, h =>
                            {
                                h.Username(transportUserIfRabbit);
                                h.Password(transportPasswordIfRabbit);
                            });

                            cfg.ConfigureEndpoints(context);

                            if (withUserTenantFilters)
                            {
                                cfg.UsePublishFilter(typeof(TenantAndUserIdsPublishFilter<>), context);
                                cfg.UseConsumeFilter(typeof(TenantAndUserIdsConsumeFilter<>), context);
                            }
                        });
                        break;
                }

                //Add all consumers
                x.AddConsumers(currentAssembly);
            });

            return services;
        }
    }
}
