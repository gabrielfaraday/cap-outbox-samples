using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using mongodb_rabbitmq.Capim.MongoDB;

namespace mongodb_rabbitmq
{
    public class PaymentConditionCreated : PaymentCondition { }

    public interface IConsumer<T> where T : class
    {
        Task ConsumeMessage(T message, [FromCap] CapHeader header);
    }

    public class ConsumerPaymentConditionCreated : IConsumer<PaymentConditionCreated>, ICapSubscribe
    {
        private readonly ILogger<ConsumerPaymentConditionCreated> _logger;
        private readonly IMessageProcessor<PaymentConditionCreated> _tracker;
        private readonly MongoClient _mongo;

        public ConsumerPaymentConditionCreated(ILogger<ConsumerPaymentConditionCreated> logger, IMessageProcessor<PaymentConditionCreated> tracker, MongoClient mongo)
        {
            _logger = logger;
            _tracker = tracker;
            _mongo = mongo;
        }

        [CapSubscribe("myapp.paymentCondition.created", Group = "myapp2.paymentCondition.created")]
        public async Task ConsumeMessage(PaymentConditionCreated message, [FromCap] CapHeader header)
        {
            if (!await _tracker.Process(header["cap-msg-id"], header, message, ProcessMessage))
            {
                _logger.LogWarning($"[ConsumerPaymentConditionCreated] Message {header["cap-msg-id"]} already processed");
                return;
            }
        }

        private async Task ProcessMessage(IClientSessionHandle session, ReadOnlyDictionary<string, string> header, PaymentCondition message)
        {
            var collection = _mongo.GetDatabase("testCap").GetCollection<PaymentCondition>("paymentConditions2");
            await collection.InsertOneAsync(session, message);
        }
    }
}