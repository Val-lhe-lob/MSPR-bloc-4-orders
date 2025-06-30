using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.TestHost;

namespace MSPR_bloc_4_orders.UnitTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // Retirer le DbContext SQL Server
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<MSPR_bloc_4_orders.Data.OrdersDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Ajouter InMemory
                services.AddDbContext<MSPR_bloc_4_orders.Data.OrdersDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Authentification de test
                services.PostConfigure<Microsoft.AspNetCore.Authentication.AuthenticationOptions>(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                });

                services.AddAuthentication("Test")
                    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
            });
        }
    }
}
