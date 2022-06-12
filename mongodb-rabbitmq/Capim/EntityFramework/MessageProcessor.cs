using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Capim.EntityFramework
{
    public class MessageProcessor<MessageType, ContextType> : IMessageProcessor<MessageType, ContextType> where ContextType : DbContext
    {
        ContextType _context;
        DbSet<MessageTracker> _messages;

        public MessageProcessor(ContextType context)
        {
            _context = context;
            _messages = context.Set<MessageTracker>();
        }

        public async Task<bool> Process(
            string messageId,
            string messageType,
            ReadOnlyDictionary<string, string> messageHeaders,
            MessageType messagePayload,
            Func<ContextType, ReadOnlyDictionary<string, string>, MessageType, Task> processingFunction,
            bool autoCommit = false)
        {
            if (HasProcessed(messageId, messageType))
                return false;

            MarkAsProcessed(messageId, messageType);

            await processingFunction(_context, messageHeaders, messagePayload);

            if (autoCommit)
                _context.SaveChanges();
            
            return true;
        }

        private bool HasProcessed(string messageId, string messageType)
            => _messages.Where(x => x.Id == messageId && x.Type == messageType).Count() > 0;

        private void MarkAsProcessed(string messageId, string messageType)
            => _messages.Add(new MessageTracker(messageId, messageType));
    }
}