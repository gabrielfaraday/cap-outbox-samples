using System.Collections.Generic;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CAP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentConditionController : ControllerBase
    {
        private readonly ILogger<PaymentConditionController> _logger;
        private readonly MongoDB _mongo;
        private readonly ICapPublisher _capBus;

        public PaymentConditionController(ILogger<PaymentConditionController> logger, MongoDB db, ICapPublisher capBus)
        {
            _logger = logger;
            _mongo = db;
            _capBus = capBus;
        }

        [HttpPost]
        public IActionResult Post(PaymentCondition newPaymentCondition)
        {
            using (var session = _mongo.Client.StartTransaction(_capBus, autoCommit: false))
            {
                var collection = _mongo.DB.GetCollection<PaymentCondition>("paymentConditions");

                collection.InsertOne(session, newPaymentCondition);

                var headers = new Dictionary<string, string>();
                headers.Add("merchant", "loja-virtual");

                _capBus.Publish("myapp.paymentCondition.created", newPaymentCondition, headers);

                session.CommitTransaction();
            }

            return Created("Created", null);
        }

        [HttpPut("{code}")]
        public IActionResult Put(PaymentCondition paymentCondition, string code)
        {
            using (var session = _mongo.Client.StartTransaction(_capBus, autoCommit: false))
            {
                var collection = _mongo.DB.GetCollection<PaymentCondition>("paymentConditions");

                var filter = Builders<PaymentCondition>.Filter.Eq(s => s.Code, code);

                collection.ReplaceOne(session, filter, paymentCondition);

                var headers = new Dictionary<string, string>();
                headers.Add("merchant", "loja-virtual");

                _capBus.Publish("myapp.paymentCondition.updated", paymentCondition, headers);

                session.CommitTransaction();
            }

            return Ok("Updated");
        }
    }
}
