using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace mongodb_rabbitmq.Capim.MongoDB
{
    public interface IMessageProcessor<T>
    {
        Task<bool> Process(string id, ReadOnlyDictionary<string, string> header, T message, Func<IClientSessionHandle, ReadOnlyDictionary<string, string>, T, Task> function);
    }


    public class MessageProcessor<T> : IMessageProcessor<T>
    {
        private readonly MongoClient _mongo;
        IClientSessionHandle _session;
        string _id;

        public MessageProcessor(MongoClient mongo)
        {
            _mongo = mongo;
        }

        public async Task<bool> Process(string id, ReadOnlyDictionary<string, string> header, T message, Func<IClientSessionHandle, ReadOnlyDictionary<string, string>, T, Task> function)
        {
            if (HasProcessed(id))
                return false;

            var session = Initialize(id);

            try
            {
                await function(session, header, message);
                
                SaveChanges();
                return true;
            }
            catch
            {
                Rollback();
                throw;
            }
        }

        private bool HasProcessed(string id)
        {
            var collection = _mongo.GetDatabase("testCap").GetCollection<MessageTracker>("cap.processed");
            return collection.CountDocuments(Builders<MessageTracker>.Filter.Eq(x => x.Id, id)) > 0;
        }

        private IClientSessionHandle Initialize(string id)
        {
            _id = id;
            _session = _mongo.StartSession();
            _session.StartTransaction();
            return _session;
        }

        private void MarkAsProcessed(IClientSessionHandle session, string id)
        {
            var collection = _mongo.GetDatabase("testCap").GetCollection<MessageTracker>("cap.processed");
            collection.InsertOne(session, new MessageTracker(id));
        }

        private void SaveChanges()
        {
            MarkAsProcessed(_session, _id);
            _session.CommitTransaction();
            _session.Dispose();
        }

        private void Rollback()
        {
            _session.AbortTransaction();
            _session.Dispose();
        }
    }

}