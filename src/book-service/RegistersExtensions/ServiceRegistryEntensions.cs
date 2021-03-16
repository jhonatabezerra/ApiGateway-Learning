using BookService.Settings;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace BookService.RegistersExtensions
{
    public static class ServiceRegistryEntensions
    {
        /// <summary>
        /// Configure where the Book-Service service goes to register
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceSettings"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsulSettings(this IServiceCollection services, ServiceSettings serviceSettings)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(ConsulConfig =>
            {
                ConsulConfig.Address = new Uri(serviceSettings.ServiceDiscoveryAddress);
            }));

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="serviceSettings"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, ServiceSettings serviceSettings)
        {
            var consultClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("AppExtensions");
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            var registration = new AgentServiceRegistration()
            {
                ID = serviceSettings.ServiceName,
                Name = serviceSettings.ServiceName,
                Address = serviceSettings.ServiceHost, //{uri.Host}
                Port = serviceSettings.ServicePort // uri.Port
            };

            // If the service already exists, it will register and register again. 
            // This serves to update the information within the Consul. Like IP, Port, or DNS.
            logger.LogInformation("Registering with Consul");
            consultClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            consultClient.Agent.ServiceRegister(registration).ConfigureAwait(true);

            lifetime.ApplicationStopped.Register(() =>
            {
                logger.LogInformation("Unregistering from Consul");
            });

            return app;
        }
    }
}
