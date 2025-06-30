using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MSPR_bloc_4_orders.UnitTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // 1️⃣ Supprimer le DbContext configuré pour SQL Server
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<MSPR_bloc_4_orders.Data.OrdersDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // 2️⃣ Ajouter InMemory DbContext
                services.AddDbContext<MSPR_bloc_4_orders.Data.OrdersDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // 3️⃣ Configurer l'authentification test
                services.PostConfigure<Microsoft.AspNetCore.Authentication.AuthenticationOptions>(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                });

                services.AddAuthentication("Test")
                    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

                // 4️⃣ S'assurer que la DB est créée
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<MSPR_bloc_4_orders.Data.OrdersDbContext>();
                db.Database.EnsureCreated();
            });
        }
    }
}
