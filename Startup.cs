using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using StockBackendMongo.DTOS.Response;
using StockBackendMongo.Repositories;
using StockBackendMongo.Settings;

namespace StockBackendMongo
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }


        // This method gets called by the runtime. Use this method to add services to the container.

        public void ConfigureServices(IServiceCollection services)
        {
            MongoDbConnect(services);
            CreateDir();
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<IUploadFileService, UploadFileService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StockBackendMongo", Version = "v1" });
            });
        }

        public void CreateDir()
        {
            string root = "wwwroot/images/";
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
        }


        private void MongoDbConnect(IServiceCollection services)
        {
            // BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            // BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
            MongoDbSetting mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSetting>();

            services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                return new MongoClient(mongoDbSettings.ConnectionString);
            });
        }


        private object MongoDbSettings() => throw new NotImplementedException();

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StockBackendMongo v1"));
            }


            app.UseHttpsRedirection();

            app.UseStaticFiles(); // */images/*

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
