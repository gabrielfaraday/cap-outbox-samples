using System;
using MongoDB.Driver;

namespace mongodb_rabbitmq
{
    public class MongoDB
    {
        public IMongoDatabase DB { get; }
        public MongoClient Client { get; }

        public MongoDB()
        {
            try
            {
                Client = new MongoClient("mongodb://localhost:27017/?authSource=admin&readPreference=primary&directConnection=true&ssl=false");
                DB = Client.GetDatabase("testCap");
            }
            catch (Exception ex)
            {
                throw new MongoException("Não foi possivel se conectar ao MongoDB", ex);
            }
        }
    }
}