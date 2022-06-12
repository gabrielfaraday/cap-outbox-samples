using MongoDB.Bson.Serialization.Attributes;

namespace mongodb_rabbitmq
{
    [BsonIgnoreExtraElements]
    public class PaymentConditionCreatedMongo
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class PaymentConditionCreatedEF
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}