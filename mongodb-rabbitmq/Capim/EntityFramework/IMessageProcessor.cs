using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Capim.EntityFramework
{
    public interface IMessageProcessor<MessageType, ContextType> where ContextType : DbContext
    {
        Task<bool> Process(
            string messageId,
            string messageType,
            ReadOnlyDictionary<string, string> messageHeaders,
            MessageType messagePayload,
            Func<ContextType, ReadOnlyDictionary<string, string>, MessageType, Task> processingFunction,
            bool autoCommit = false);
    }
}