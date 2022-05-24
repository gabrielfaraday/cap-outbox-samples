using Microsoft.EntityFrameworkCore;

namespace mongodb_rabbitmq.Capim.EF
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder MapMessageTracker(this ModelBuilder builder)
        {
            return builder.Entity<MessageTracker>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(255);
                // entity.Property(e => e.Type).HasMaxLength(255);

                entity.HasKey(e => new
                {
                    e.Id,
                    // e.Type
                });
            });
        }
    }
}