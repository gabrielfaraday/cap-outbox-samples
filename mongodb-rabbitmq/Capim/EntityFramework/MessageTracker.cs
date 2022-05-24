namespace mongodb_rabbitmq.Capim.EF
{
    public class MessageTracker
    {
        public MessageTracker(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}