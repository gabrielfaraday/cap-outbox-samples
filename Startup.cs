using Capim.EntityFramework.Setup;
using Capim.MongoDB.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using mongodb_rabbitmq.Consumers;

namespace mongodb_rabbitmq
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CAP", Version = "v1" });
            });

            //MONGO
            var mongoClient = new MongoClient("mongodb://localhost:27017/?authSource=admin&readPreference=primary&directConnection=true&ssl=false");
            services.AddSingleton<MongoClient>(mongoClient);

            services.AddCapimMongoDB(x =>
            {
                x.MongoDatabaseName  = "testCap";
                //x.MongoCollectionName = "message-tracker";
                x.AddMessageProcessor<PaymentConditionCreatedMongo>();
            });

            services.AddTransient<IConsumer<PaymentConditionCreatedMongo>, ConsumerMongo>();

            //EF
            services.AddDbContext<ExampleDbContext>(options =>
                options.UseSqlServer("Server=localhost,5100;Initial Catalog=CAP;User ID=sa;Password=Password1;"));

            services.AddCapimEntityFramework(x =>
            {
                x.AddMessageProcessor<PaymentConditionCreatedEF, ExampleDbContext>();
            });

            services.AddTransient<IConsumer<PaymentConditionCreatedEF>, ConsumerEF>();

            //CAP
            services.AddCap(x =>
            {
                x.UseEntityFramework<ExampleDbContext>();

                x.UseDashboard();

                x.UseRabbitMQ(opt =>
                {
                    opt.Port = 5672;
                    opt.HostName = "localhost";
                    opt.Password = "guest";
                    opt.UserName = "guest";
                    opt.VirtualHost = "/"; 
                });

                x.UseMongoDB(opt =>
                {
                    opt.DatabaseConnection = "mongodb://localhost:27017/?authSource=admin&readPreference=primary&directConnection=true&ssl=false";
                    opt.DatabaseName = "testCap";
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CAP v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
