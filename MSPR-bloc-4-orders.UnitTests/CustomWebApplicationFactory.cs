using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MSPR_bloc_4_orders.Data;
using MSPR_bloc_4_orders.Services;
using Microsoft.AspNetCore.Authentication;

namespace MSPR_bloc_4_orders.UnitTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Supprime OrdersDbContext SqlServer
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<OrdersDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<OrdersDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryOrdersTestDb");
                });

                // Mock RabbitMQ
                services.AddScoped<IRabbitMqPublisher, FakeRabbitMqPublisher>();

                // Remplace l'authentification par un schéma de test
                services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

                // Initialise la DB
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
                db.Database.EnsureCreated();
            });
        }
    }
}
