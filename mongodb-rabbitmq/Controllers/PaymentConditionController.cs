using System.Collections.Generic;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace mongodb_rabbitmq.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentConditionController : ControllerBase
    {
        private readonly ILogger<PaymentConditionController> _logger;
        private readonly MongoClient _mongo;
        private readonly ICapPublisher _capBus;

        public PaymentConditionController(ILogger<PaymentConditionController> logger, MongoClient mongo, ICapPublisher capBus)
        {
            _logger = logger;
            _mongo = mongo;
            _capBus = capBus;
        }

        [HttpPost]
        public IActionResult Post(PaymentConditionCreatedMongo newPaymentCondition)
        {
            using (var session = _mongo.StartTransaction(_capBus, autoCommit: false))
            {
                var collection = _mongo.GetDatabase("testCap").GetCollection<PaymentConditionCreatedMongo>("paymentConditions");

                newPaymentCondition.Name += " - controller";

                collection.InsertOne(session, newPaymentCondition);

                var headers = new Dictionary<string, string>();
                headers.Add("merchant", "loja-virtual");

                _capBus.Publish("myapp.paymentCondition.created", newPaymentCondition, headers);

                session.CommitTransaction();
            }

            return Created("Created", null);
        }

        [HttpPut("{code}")]
        public IActionResult Put(PaymentConditionCreatedMongo paymentCondition, string code)
        {
            using (var session = _mongo.StartTransaction(_capBus, autoCommit: false))
            {
                var collection = _mongo.GetDatabase("testCap").GetCollection<PaymentConditionCreatedMongo>("paymentConditions");

                var filter = Builders<PaymentConditionCreatedMongo>.Filter.Eq(s => s.Code, code);

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
