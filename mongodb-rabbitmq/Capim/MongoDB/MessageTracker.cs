using MongoDB.Bson.Serialization.Attributes;

namespace mongodb_rabbitmq.Capim.MongoDB
{
    public class MessageTracker
    {
        public MessageTracker(string id)
        {
            Id = id;
        }

        [BsonId]
        public string Id { get; set; }
    }
}