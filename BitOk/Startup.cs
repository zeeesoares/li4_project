using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using BitOk.Data;
using BitOk.Auth;
using BitOk.Hubs;
using BitOk.Data.Services;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using MudBlazor;

namespace BitOk
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

            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter; 
                config.SnackbarConfiguration.HideTransitionDuration = 200; 
                config.SnackbarConfiguration.ShowTransitionDuration = 200; 
                config.SnackbarConfiguration.VisibleStateDuration = 1800;
                config.SnackbarConfiguration.MaxDisplayedSnackbars = 5; 
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled; 
            });


            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddAuthenticationCore();
            services.AddScoped<AuthenticationStateProvider, AuthMain>();

            services.AddTransient<ISqlDataAccess, SqlDataAccess>();
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IOrderServiceBackground, OrderServiceBackground>();
            services.AddTransient<IPecaService, PecaService>();

            services.AddHostedService<OrderUpdateService>();

            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");

                endpoints.MapHub<MyHub>("/myhub");
            });
        }
    }
}
