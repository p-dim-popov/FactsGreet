namespace FactsGreet.Web
{
    using System;
    using System.Reflection;
    using FactsGreet.Data;
    using FactsGreet.Data.Common;
    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Data.Repositories;
    using FactsGreet.Data.Seeding;
    using FactsGreet.Services.Data;
    using FactsGreet.Services.Mapping;
    using FactsGreet.Services.Messaging;
    using FactsGreet.Web.Controllers;
    using FactsGreet.Web.ViewModels;
    using Markdig;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Westwind.AspNetCore.Markdown;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

            services
                // .AddIdentity<ApplicationUser, ApplicationRole>(IdentityOptionsProvider.GetIdentityOptions)
                .AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.Configure<CookiePolicyOptions>(
                options =>
                {
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });

            services.AddControllersWithViews(
                    options => { options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); })
                .AddRazorRuntimeCompilation();
            services.AddRazorPages();

            services.AddSingleton(this.configuration);

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();
            services.AddScoped<ArticlesService>();
            services.AddScoped<EditsService>();
            services.AddScoped<NotificationsService>();
            services.AddScoped<ArticleDeletionRequestsService>();
            services.AddScoped<ApplicationUsersService>();

            // Application services
            services.AddTransient<IEmailSender, NullMessageSender>();
            services.AddTransient<ISettingsService, SettingsService>();

            services.AddMarkdown(config =>
            {
                config.ConfigureMarkdigPipeline = builder =>
                {
                    builder
                        .DisableHtml()
                        .UseEmojiAndSmiley()
                        .UseSoftlineBreakAsHardlineBreak()
                        .UseBootstrap()
                        .UseAdvancedExtensions();
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
                new ApplicationDbContextSeeder()
                    .SeedAsync(dbContext, serviceScope.ServiceProvider)
                    .Wait();
            }

            app.Use(async (context, next) =>
            {
                var destination = context.Request.Path.Value ?? string.Empty;
                if (destination.Contains(' '))
                {
                    context.Response
                        .Redirect(destination
                            .Replace(' ', '_'));
                }

                context.Request.Path = destination.Replace('_', ' ');

                await next.Invoke();
            });

            app.UseStatusCodePagesWithReExecute($"/Errors/{nameof(ErrorsController.StatusCodePage)}/{{0}}");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMarkdown();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapRazorPages();
                });
        }
    }
}
