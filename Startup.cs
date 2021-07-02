using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
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
            services.AddSingleton<IAuthRepository, AuthRepository>();

            var constants = new Constants();
            Configuration.Bind(nameof(constants), constants);
            services.AddSingleton(constants);
            AuthenticationAndJWTBearer(services);

            services.AddControllers();

            //AddSwaggerGen
            ConfigSwaggerAuthenticate(services);
        }

        private void ConfigSwaggerAuthenticate(IServiceCollection services)
        {
            var Bearer = "Bearer";
            services.AddSwaggerGen(c =>
           {
               c.SwaggerDoc("v1", new OpenApiInfo { Title = "StockBackendMongo", Version = "v1" });

               c.AddSecurityDefinition(Bearer, new OpenApiSecurityScheme
               {
                   BearerFormat = "JWT",
                   Description = "JWT Authorization",
                   Name = "Authorization",
                   In = ParameterLocation.Header,
                   Type = SecuritySchemeType.Http,
                   Scheme = Bearer,
               });

               c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                    new OpenApiSecurityScheme
                    {
                        Reference=new OpenApiReference                        {
                            Id="Bearer",Type=ReferenceType.SecurityScheme,
                        },
                        Scheme=Bearer,Name=Bearer,In=ParameterLocation.Header
                    },
                        new List<string>()
                    }
               });
           });
        }

        public void AuthenticationAndJWTBearer(IServiceCollection services)
        {
            var jwtSettings = new JwtSettings();
            Configuration.Bind(nameof(jwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                        ClockSkew = TimeSpan.Zero
                    };
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

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
