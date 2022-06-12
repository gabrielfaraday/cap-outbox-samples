using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Capim.EntityFramework.Setup
{
    public class CapimEntityFrameworkOptions
    {
        internal List<Action<IServiceCollection>> Extensions { get; } = new();

        public void AddMessageProcessor<MessageType, ContextType>() where ContextType : DbContext
        {
            static void SetupNewProcessor(IServiceCollection services) =>
                services.AddScoped<IMessageProcessor<MessageType, ContextType>, MessageProcessor<MessageType, ContextType>>();

            Extensions.Add(SetupNewProcessor);
        }
    }
}