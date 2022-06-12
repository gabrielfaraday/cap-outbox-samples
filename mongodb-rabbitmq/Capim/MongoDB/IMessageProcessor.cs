using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Capim.MongoDB
{
    public interface IMessageProcessor<MessageType>
    {
        Task<bool> Process(
            string messageId,
            string messageType,
            ReadOnlyDictionary<string, string> messageHeaders,
            MessageType messagePayload,
            Func<IClientSessionHandle, ReadOnlyDictionary<string, string>, MessageType, Task> processingFunction,
            bool autoCommit = false);
    }
}