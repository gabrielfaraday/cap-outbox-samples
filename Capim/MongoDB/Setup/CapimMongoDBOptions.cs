using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Capim.MongoDB.Setup
{
    public class CapimMongoDBOptions
    {
        internal List<Action<IServiceCollection>> Extensions { get; } = new();
        public string MongoDatabaseName { get; set; }
        public string MongoCollectionName { get; set; } = "message-tracker";

        public void AddMessageProcessor<MessageType>()
        {
            static void SetupNewProcessor(IServiceCollection services) =>
                services.AddScoped<IMessageProcessor<MessageType>, MessageProcessor<MessageType>>();

            Extensions.Add(SetupNewProcessor);
        }
    }
}