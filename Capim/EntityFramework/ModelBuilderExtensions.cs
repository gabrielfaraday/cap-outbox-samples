using Microsoft.EntityFrameworkCore;

namespace Capim.EntityFramework
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder MapMessageTracker(this ModelBuilder builder, string tableName = null)
        {
            return builder.Entity<MessageTracker>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(255);
                entity.Property(e => e.Type).HasMaxLength(255);

                entity.HasKey(e => new
                {
                    e.Id,
                    e.Type
                });

                if (!string.IsNullOrWhiteSpace(tableName))
                    entity.ToTable(tableName);
            });
        }
    }
}