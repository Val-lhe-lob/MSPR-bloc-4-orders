using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MSPR_bloc_4_orders.Data;
using Microsoft.AspNetCore.Authentication;
using System.Linq;

namespace MSPR_bloc_4_orders.UnitTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Retirer le DbContext SQL Server
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<OrdersDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Ajouter DbContext InMemory
                services.AddDbContext<OrdersDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Mock Authentication pour tests d'intégration
                services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
            });
        }
    }
}
