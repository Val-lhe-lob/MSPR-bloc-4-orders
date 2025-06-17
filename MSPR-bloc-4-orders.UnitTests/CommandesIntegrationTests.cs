using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using MSPR_bloc_4_orders.Models;

namespace MSPR_bloc_4_orders.UnitTests
{
    public class CommandesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public CommandesIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient(); 
        }

        [Fact]
        public async Task GetCommandes_ShouldReturnSuccessAndList()
        {
            var response = await _client.GetAsync("/api/Commandes");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var commandes = await response.Content.ReadFromJsonAsync<List<Commande>>();
            commandes.Should().NotBeNull();
        }



    }
}
