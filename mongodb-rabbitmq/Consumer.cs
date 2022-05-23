using System;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;

namespace mongodb_rabbitmq
{
    public class PaymentConditionCreated : PaymentCondition { }

    public interface IConsumer<T> where T : class
    {
        void ConsumeMessage(T message, [FromCap]CapHeader header);
    }

    public class ConsumerPaymentConditionCreated : IConsumer<PaymentConditionCreated>, ICapSubscribe
    {
        private readonly ILogger<ConsumerPaymentConditionCreated> _logger;
        private readonly IMessageTracker _tracker;

        public ConsumerPaymentConditionCreated(ILogger<ConsumerPaymentConditionCreated> logger, IMessageTracker tracker)
        {
            _logger = logger;
            _tracker = tracker;
        }

        [CapSubscribe("myapp.paymentCondition.created", Group = "myapp2.paymentCondition.created")]
        public void ConsumeMessage(PaymentConditionCreated message, [FromCap]CapHeader header)
        {
            var id = Convert.ToInt64(header["cap-msg-id"]);
            if (_tracker.HasProcessed(id))
            {
                _logger.LogWarning($"[ConsumerPaymentConditionCreated] Message {header["cap-msg-id"]} already processed");
                return;
            }

            _logger.LogInformation($"[ConsumerPaymentConditionCreated] {message.Code} - {message.Name} - {header["merchant"]} - {header["cap-msg-id"]} - {header["cap-msg-name"]}");
            _tracker.MarkAsProcessed(id);
        }
    }

    public class ConsumerPaymentCondition : IConsumer<PaymentCondition>, ICapSubscribe
    {
        private readonly ILogger<ConsumerPaymentCondition> _logger;
        private readonly IMessageTracker _tracker;

        public ConsumerPaymentCondition(ILogger<ConsumerPaymentCondition> logger, IMessageTracker tracker)
        {
            _logger = logger;
            _tracker = tracker;
        }

        [CapSubscribe("myapp.paymentCondition.updated", Group = "myapp2.paymentCondition.updated")]
        public void ConsumeMessage(PaymentCondition message, [FromCap]CapHeader header)
        {
            var id = Convert.ToInt64(header["cap-msg-id"]);
            if (_tracker.HasProcessed(id))
            {
                _logger.LogWarning($"[ConsumerPaymentCondition] Message {header["cap-msg-id"]} already processed");
                return;
            }

            _logger.LogInformation($"[ConsumerPaymentCondition] {message.Code} - {message.Name} - {header["merchant"]} - {header["cap-msg-id"]} - {header["cap-msg-name"]}");
            _tracker.MarkAsProcessed(id);
        }
    }

}