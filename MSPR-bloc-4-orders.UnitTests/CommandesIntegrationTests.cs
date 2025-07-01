using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using MSPR_bloc_4_orders.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MSPR_bloc_4_orders.UnitTests
{
    public class CommandesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public CommandesIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetCommandes_ShouldReturnSuccessAndList()
        {

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MSPR_bloc_4_orders.Data.OrdersDbContext>();

                db.Commandes.Add(new Commande
                {
                    IdCommande = 1,
                    IdClient = 123,
                    Createdate = DateTime.UtcNow
                });

                db.SaveChanges();
            }

            // Act
            var response = await _client.GetAsync("/api/Commandes");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var commandes = await response.Content.ReadFromJsonAsync<List<Commande>>();
            commandes.Should().NotBeNull();
            commandes.Should().ContainSingle(c => c.IdCommande == 1);
        }
    }
}
