using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Capim.MongoDB;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace mongodb_rabbitmq.Consumers
{
    public class ConsumerMongo : IConsumer<PaymentConditionCreatedMongo>, ICapSubscribe
    {
        private readonly ILogger<ConsumerMongo> _logger;
        private readonly IMessageProcessor<PaymentConditionCreatedMongo> _trackerMongo;
        private readonly MongoClient _mongo;

        public ConsumerMongo(
            ILogger<ConsumerMongo> logger,
            IMessageProcessor<PaymentConditionCreatedMongo> trackerMongo,
            MongoClient mongo)
        {
            _logger = logger;
            _trackerMongo = trackerMongo;
            _mongo = mongo;
        }

        [CapSubscribe("myapp.paymentCondition.created", Group = "mongo.paymentCondition.created")]
        public async Task ConsumeMessage(PaymentConditionCreatedMongo message, [FromCap] CapHeader header)
        {  
            //TODO


            if (!await _trackerMongo.Process(header["cap-msg-id"], header["cap-msg-type"], header, message, ProcessMessage, autoCommit: false))
            {
                _logger.LogWarning($"[ConsumerMongo] Message {header["cap-msg-id"]} already processed");
                return;
            }
        }

        private async Task ProcessMessage(IClientSessionHandle session, ReadOnlyDictionary<string, string> header, PaymentCondition message)
        {
            var collection = _mongo.GetDatabase("testCap").GetCollection<PaymentCondition>("paymentConditions");
            await collection.InsertOneAsync(session, message);
        }
    }
}