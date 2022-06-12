using System;
using Microsoft.Extensions.DependencyInjection;

namespace Capim.MongoDB.Setup
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCapimMongoDB(
            this IServiceCollection services,
            Action<CapimMongoDBOptions> setupOptions)
        {
            var options = new CapimMongoDBOptions();
            setupOptions(options);

            if (string.IsNullOrWhiteSpace(options.MongoDatabaseName))
                throw new ArgumentException("MongoDatabaseName must be set to Capim MongoDB usage");

            services.AddSingleton(options);

            foreach (var extension in options.Extensions)
            {
                extension(services);
            }

            return services;
        }
    }
}