using Capim.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace mongodb_rabbitmq
{
    public class ExampleDbContext : DbContext
    {
        public ExampleDbContext(DbContextOptions<ExampleDbContext> options)
            : base(options)
        {
        }

        public DbSet<MessageTracker> Messages { get; set; }
        public DbSet<PaymentConditionCreatedEF> PaymentConditions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.MapMessageTracker("MyTracker");
            MapPaymentCondition(modelBuilder);
        }

        private ModelBuilder MapPaymentCondition(ModelBuilder builder)
        {
            return builder.Entity<PaymentConditionCreatedEF>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasKey(e => e.Id);
            });
        }
    }
}