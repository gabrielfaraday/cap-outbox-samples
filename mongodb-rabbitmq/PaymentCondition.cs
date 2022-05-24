using MongoDB.Bson.Serialization.Attributes;

namespace mongodb_rabbitmq
{
    [BsonIgnoreExtraElements]
    public class PaymentCondition
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class PaymentConditionCreatedMongo : PaymentCondition { }
    public class PaymentConditionCreatedEF : PaymentCondition
    {
        public long Id { get; set; }
    }
}