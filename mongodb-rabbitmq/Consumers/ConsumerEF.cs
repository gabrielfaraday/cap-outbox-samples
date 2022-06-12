using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Capim.EntityFramework;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;

namespace mongodb_rabbitmq.Consumers
{
    public class ConsumerEF : IConsumer<PaymentConditionCreatedEF>, ICapSubscribe
    {
        private readonly ILogger<ConsumerEF> _logger;
        private readonly IMessageProcessor<PaymentConditionCreatedEF, ExampleDbContext> _trackerEF;

        public ConsumerEF(
            ILogger<ConsumerEF> logger,
            IMessageProcessor<PaymentConditionCreatedEF, ExampleDbContext> trackerEF)
        {
            _logger = logger;
            _trackerEF = trackerEF;
        }

        [CapSubscribe("myapp.paymentCondition.created", Group = "ef.paymentCondition.created")]
        public async Task ConsumeMessage(PaymentConditionCreatedEF message, [FromCap] CapHeader header)
        {
            if (!await _trackerEF.Process(header["cap-msg-id"], header["cap-msg-type"], header, message, ProcessMessage, autoCommit: true))
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