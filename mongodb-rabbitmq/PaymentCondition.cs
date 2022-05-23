using MongoDB.Bson.Serialization.Attributes;

namespace mongodb_rabbitmq
{
    [BsonIgnoreExtraElements]
    public class PaymentCondition
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int[] Deadline { get; set; }
    }
}