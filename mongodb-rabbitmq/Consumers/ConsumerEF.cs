using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using mongodb_rabbitmq.Capim.EF;

namespace mongodb_rabbitmq.Consumers
{
    public class ConsumerEF : IConsumer<PaymentConditionCreatedEF>, ICapSubscribe
    {
        private readonly ILogger<ConsumerEF> _logger;
        private readonly IMessageProcessor<PaymentConditionCreatedEF, ExampleDbContext> _trackerEF;
        private readonly MongoClient _mongo;

        public ConsumerEF(
            ILogger<ConsumerEF> logger,
            IMessageProcessor<PaymentConditionCreatedEF, ExampleDbContext> trackerMongo,
            MongoClient mongo)
        {
            _logger = logger;
            _trackerEF = trackerMongo;
            _mongo = mongo;
        }

        [CapSubscribe("myapp.paymentCondition.created", Group = "ef.paymentCondition.created")]
        public async Task ConsumeMessage(PaymentConditionCreatedEF message, [FromCap] CapHeader header)
        {
            if (!await _trackerEF.Process(header["cap-msg-id"], header, message, ProcessMessage))
            {
                _logger.LogWarning($"[ConsumerEF] Message {header["cap-msg-id"]} already processed");
                return;
            }
        }

        private async Task ProcessMessage(ExampleDbContext context, ReadOnlyDictionary<string, string> header, PaymentCondition message)
        {
            await context.AddAsync(message);
        }
    }
}