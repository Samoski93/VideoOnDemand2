using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VideoOnDemand.UI.Services;
using VideoOnDemand.Data.Data;
using VideoOnDemand.Data.Data.Entities;
using VideoOnDemand.UI.Repositories;
using VideoOnDemand.UI.Models.DTOModels;
using VideoOnDemand.Data.Migrations;

namespace VideoOnDemand.UI
{
    public class Startup
    {
        // This will make it possible to read from the appsettings.json configuration file, where the connection string is stored
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<VODContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<VODContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            // Add Application Services
            // To be able to inject objects from classes created (Add service mapping), specify what class will be used to serve up the objects
            services.AddSingleton<IReadRepository, MockReadRepository>();

            services.AddMvc();

            // configuration tells AutoMapper how to map between objects, in this case between entities and DTOs
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                // mapping for the Video entity and VideoDTO classes
                cfg.CreateMap<Video, VideoDTO>();

                cfg.CreateMap<Instructor, InstructorDTO>()
                   .ForMember(dest => dest.InstructorName,
                       src => src.MapFrom(s => s.Name))
                   .ForMember(dest => dest.InstructorDescription,
                       src => src.MapFrom(s => s.Description))
                   .ForMember(dest => dest.InstructorAvatar,
                       src => src.MapFrom(s => s.Thumbnail));

                // mapping for the Download entity and DownloadDTO classes
                // Here specific configuration is necessary since the properties are named differently in the two classes
                cfg.CreateMap<Download, DownloadDTO>()
                     .ForMember(dest => dest.DownloadUrl,
                         src => src.MapFrom(s => s.Url))
                     .ForMember(dest => dest.DownloadTitle,
                         src => src.MapFrom(s => s.Title));

                cfg.CreateMap<Course, CourseDTO>()
                    .ForMember(dest => dest.CourseId, src =>
                        src.MapFrom(s => s.Id))
                    .ForMember(dest => dest.CourseTitle,
                        src => src.MapFrom(s => s.Title))
                    .ForMember(dest => dest.CourseDescription,
                        src => src.MapFrom(s => s.Description))
                    .ForMember(dest => dest.MarqueeImageUrl,
                        src => src.MapFrom(s => s.MarqueeImageUrl))
                    .ForMember(dest => dest.CourseImageUrl,
                        src => src.MapFrom(s => s.ImageUrl));

                cfg.CreateMap<Module, ModuleDTO>()
                    .ForMember(dest => dest.ModuleTitle,
                        src => src.MapFrom(s => s.Title));
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, VODContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // add the seed data when the application is started.
            DbInitializer.Initialize(db);

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
