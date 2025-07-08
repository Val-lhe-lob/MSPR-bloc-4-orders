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

        [Fact]
        public async Task GetProduitCommandes_ShouldReturnSuccessAndList()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MSPR_bloc_4_orders.Data.OrdersDbContext>();

                db.ProduitCommandes.Add(new ProduitCommande
                {
                    IdProduitcommande = 1,
                    IdCommande = 1,
                    IdProduit = 100,
                    Nom = "Test Produit",
                    Quantite = 2,
                    CreatedAt = DateTime.UtcNow
                });
                db.SaveChanges();
            }

            var response = await _client.GetAsync("/api/ProduitCommandes");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var produits = await response.Content.ReadFromJsonAsync<List<ProduitCommande>>();
            produits.Should().NotBeNull();
            produits.Should().ContainSingle(p => p.IdProduitcommande == 1);
        }

        [Fact]
        public async Task PostProduitCommandes_ShouldCreateAndReturnList()
        {
            var produits = new List<ProduitCommande>
    {
        new ProduitCommande { IdProduitcommande = 2, IdCommande = 1, IdProduit = 101, Quantite = 3, CreatedAt = DateTime.UtcNow },
        new ProduitCommande { IdProduitcommande = 3, IdCommande = 1, IdProduit = 102, Quantite = 4, CreatedAt = DateTime.UtcNow }
    };

            var response = await _client.PostAsJsonAsync("/api/ProduitCommandes", produits);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returned = await response.Content.ReadFromJsonAsync<List<ProduitCommande>>();
            returned.Should().NotBeNull();
            returned.Count.Should().BeGreaterThan(1);
        }

    }
}
