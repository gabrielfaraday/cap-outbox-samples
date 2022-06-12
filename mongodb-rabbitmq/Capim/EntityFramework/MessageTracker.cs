namespace Capim.EntityFramework
{
    public class MessageTracker
    {
        public MessageTracker(string id, string type)
        {
            Id = id;
            Type = type;
        }

        public string Id { get; private set; }
        public string Type { get; private set; }
    }
}