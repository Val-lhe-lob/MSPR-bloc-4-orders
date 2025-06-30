using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MSPR_bloc_4_orders.Data;

namespace MSPR_bloc_4_orders.UnitTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Supprimer l'enregistrement existant du DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<OrdersDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Ajouter InMemoryDatabase
                services.AddDbContext<OrdersDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Configurer l'auth test
                services.PostConfigure<Microsoft.AspNetCore.Authentication.AuthenticationOptions>(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                });

                services.AddAuthentication("Test")
                    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, TestAuthHandler>(
                        "Test", _ => { });

                // Assurer la création de la base à chaque test
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
                db.Database.EnsureCreated();
            });
        }
    }
}
