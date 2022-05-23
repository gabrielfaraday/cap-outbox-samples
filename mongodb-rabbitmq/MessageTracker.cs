using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace mongodb_rabbitmq
{
    public class Message
    {
        public Message(long id)
        {
            Id = id;
        }

        [BsonId]
        public long Id { get; set; }
    }

    public interface IMessageTracker
    {
        bool HasProcessed(long id);
        void MarkAsProcessed(long id);
    }

    public class MessageTracker : IMessageTracker
    {
        private readonly MongoDB _mongo;

        public MessageTracker(MongoDB mongo)
        {
            _mongo = mongo;
        }
        public bool HasProcessed(long id)
        {
            var collection = _mongo.DB.GetCollection<Message>("cap.processed");
            return collection.CountDocuments(Builders<Message>.Filter.Eq(x => x.Id, id)) > 0;
        }

        public void MarkAsProcessed(long id)
        {
            var collection = _mongo.DB.GetCollection<Message>("cap.processed");
            collection.InsertOne(new Message(id));
        }
    }
}