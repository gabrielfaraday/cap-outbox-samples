using MongoDB.Bson.Serialization.Attributes;

namespace CAP
{
    [BsonIgnoreExtraElements]
    public class PaymentCondition
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int[] Deadline { get; set; }
    }
}