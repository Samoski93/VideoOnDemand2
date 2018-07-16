using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VideoOnDemand.Data.Data;
using VideoOnDemand.Data.Data.Entities;

namespace VideoOnDemand.Data
{
    public class Startup
    {
        // Configuration. This will make it possible to read from the application.json configuration file, where the connection string is stored.
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // The SqlServer provider to be able to create and call databases. Call the AddDBContext method on the service object and
            // specify the VODContext as the context to use. Use the options action to specify the database provider and connection string.
            services.AddDbContext<VODContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection")));

            // Call the AddIdentity method on the service object to use the User entity class when creating the AspNetUsers table in the database.
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<VODContext>()
                .AddDefaultTokenProviders();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
