using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace CAP
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

            services.AddSingleton<MongoDB>();

            services.AddCap(x =>
            {
                x.UseRabbitMQ(opt =>
                {
                    opt.Port = 5672;
                    opt.HostName = "localhost";
                    opt.Password = "guest";
                    opt.UserName = "guest";
                    opt.VirtualHost = "/";
                    opt.CustomHeaders = h => new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("merchant", "loja-virtual")
                    };
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
