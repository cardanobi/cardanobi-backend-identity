// using Serilog;
// using Microsoft.EntityFrameworkCore;

using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
// using IdentityServerHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Reflection;

namespace IdentityServer;

internal static class HostingExtensions
{
    private static void InitializeDatabase(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        {
            serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            context.Database.Migrate();
            if (!context.Clients.Any())
            {
                foreach (var client in Config.Clients)
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var resource in Config.ApiScopes)
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
        }
    }

    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

        builder.Services.AddIdentityServer()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseNpgsql(builder.Configuration.GetConnectionString("DbSyncPgsqlDatabase"),
                    sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseNpgsql(builder.Configuration.GetConnectionString("DbSyncPgsqlDatabase"),
                    sql => sql.MigrationsAssembly(migrationsAssembly));
            });
        // .AddTestUsers(TestUsers.Users);

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // InitializeDatabase(app);

        // uncomment if you want to add a UI
        //app.UseStaticFiles();
        // app.UseRouting();

        app.UseIdentityServer();

        // uncomment if you want to add a UI
        //app.UseAuthorization();
        //app.MapRazorPages().RequireAuthorization();

        return app;
    }
}
