using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RN_TaskManager.DAL.Context;
using RN_TaskManager.DAL.Repositories;
using RN_TaskManager.Web.AutoMapperProfiles;
using RN_TaskManager.Web.Services;

namespace RN_TaskManager.Web
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

            #region AuthConfig

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IISDefaults.AuthenticationScheme;
            });
            
            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = true;
            });

            #endregion

            #region DbContexts

            // TODO: добавить логику для определения строки подключения
            var connectionString = Configuration.GetConnectionString("devConnectionString");

            services.AddDbContext<RN_TaskManagerContext>(opt =>
            {
                opt.UseSqlServer(connectionString, builder => builder.CommandTimeout(300));
            });

            #endregion

            #region Repositoryies

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectTaskRepository, ProjectTaskRepository>();
            services.AddScoped<IProjectTaskStatusRepository, ProjectTaskStatusRepository>();
            services.AddScoped<IProjectTaskTypeRepository, ProjectTaskTypeRepository>();
            services.AddScoped<IProjectTaskPerformerRepository, ProjectTaskPerformerRepository>();

            #endregion

            #region Services

            services.AddScoped<IUserService, UserService>();

            #endregion

            services.AddAutoMapper(typeof(TaskManagerAutoMapperProfile));

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddControllers().AddJsonOptions(options =>
            {
                // Use the default property (Pascal) casing.
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
