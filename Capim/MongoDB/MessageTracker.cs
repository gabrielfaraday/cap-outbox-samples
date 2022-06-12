using MongoDB.Bson.Serialization.Attributes;

namespace Capim.MongoDB
{
    public class MessageTracker
    {
       public MessageTracker(string id, string type)
        {
            Id = id;
            Type = type;
        }

        [BsonId]
        public string Id { get; private set; }
        public string Type { get; private set; }
    }
}