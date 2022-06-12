using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Capim.MongoDB.Setup;
using MongoDB.Driver;

namespace Capim.MongoDB
{
    public class MessageProcessor<MessageType> : IMessageProcessor<MessageType>
    {
        private readonly MongoClient _mongo;
        private readonly CapimMongoDBOptions _options;

        public MessageProcessor(MongoClient mongo, CapimMongoDBOptions options)
        {
            _mongo = mongo;
            _options = options;
        }

        public async Task<bool> Process(
            string messageId,
            string messageType,
            ReadOnlyDictionary<string, string> messageHeaders,
            MessageType messagePayload,
            Func<IClientSessionHandle, ReadOnlyDictionary<string, string>, MessageType, Task> processingFunction,
            bool autoCommit = false)
        {
            if (HasProcessed(messageId, messageType))
                return false;

            using (var session = _mongo.StartSession())
            {
                session.StartTransaction();

                MarkAsProcessed(session, messageId, messageType);

                await processingFunction(session, messageHeaders, messagePayload);

                if (autoCommit)
                    session.CommitTransaction();

                return true;
            }
        }

        private bool HasProcessed(string messageId, string messageType)
        {
            var collection = _mongo.GetDatabase(_options.MongoDatabaseName).GetCollection<MessageTracker>(_options.MongoCollectionName);

            return collection
                .AsQueryable()
                .Where(x => x.Id == messageId && x.Type == messageType)
                .Count() > 0;
        }

        private void MarkAsProcessed(IClientSessionHandle session, string messageId, string messageType)
        {
            var collection = _mongo.GetDatabase(_options.MongoDatabaseName).GetCollection<MessageTracker>(_options.MongoCollectionName);
            collection.InsertOne(session, new MessageTracker(messageId, messageType));
        }
    }
}