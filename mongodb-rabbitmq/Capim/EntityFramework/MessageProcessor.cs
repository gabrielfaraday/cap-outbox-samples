using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace mongodb_rabbitmq.Capim.EF
{
    public interface IMessageProcessor<T, C> where C : DbContext
    {
        Task<bool> Process(string id, ReadOnlyDictionary<string, string> header, T message, Func<C, ReadOnlyDictionary<string, string>, T, Task> function);
    }

    public class MessageProcessor<T, C> : IMessageProcessor<T, C> where C : DbContext
    {
        C _context;
        DbSet<MessageTracker> _messages;
        string _id;

        public MessageProcessor(C context)
        {
            _context = context;
            _messages = context.Set<MessageTracker>();
        }

        public async Task<bool> Process(string id, ReadOnlyDictionary<string, string> header, T message, Func<C, ReadOnlyDictionary<string, string>, T, Task> function)
        {
            if (HasProcessed(id))
                return false;

            _id = id;

            await function(_context, header, message);

            SaveChanges();
            return true;
        }

        private bool HasProcessed(string id)
        {
            return _messages.Where(x => x.Id == id).Count() > 0;
        }

        private void MarkAsProcessed(string id)
        {
            _messages.Add(new MessageTracker(id));
        }

        private void SaveChanges()
        {
            MarkAsProcessed(_id);
            _context.SaveChanges();
        }
    }
}