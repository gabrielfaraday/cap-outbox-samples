using System;
using Microsoft.Extensions.DependencyInjection;

namespace Capim.EntityFramework.Setup
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCapimEntityFramework(
            this IServiceCollection services,
            Action<CapimEntityFrameworkOptions> setupOptions)
        {
            var options = new CapimEntityFrameworkOptions();
            setupOptions(options);

            foreach (var extension in options.Extensions)
            {
                extension(services);
            }

            return services;
        }
    }
}