﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Couchbase.Extensions.DependencyInjection;
using CheckoutApi.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CheckoutApi.Services;

namespace CheckoutApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)                
                .AddEnvironmentVariables();

            configuration = builder.Build();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration.GetSection("PATH").Get<Configuration>());
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://securetoken.google.com/homedepotcs420";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "https://securetoken.google.com/homedepotcs420",
                        ValidateAudience = true,
                        ValidAudience = "homedepotcs420",
                        ValidateLifetime = true
                    };
                }
                );
            
            services
                .AddCouchbase(Configuration.GetSection("Couchbase"))
                .AddCouchbaseBucket<UserInfoContext>("UserInfo")
                .AddCouchbaseBucket<IMyBucketProvider>("Checkout");

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "ChecoutAPI" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //            app.UseHttpsRedirection();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseMvc();
            app.UseSwagger()
                .UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CheckoutAPI"));
            
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                app.ApplicationServices.GetRequiredService<ICouchbaseLifetimeService>().Close();
            });
            
        }
    }
}
