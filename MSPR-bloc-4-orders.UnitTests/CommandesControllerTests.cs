using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using MSPR_bloc_4_orders.Data;
using MSPR_bloc_4_orders.Controllers;
using MSPR_bloc_4_orders.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MSPR_bloc_4_orders.UnitTests
{
    public class CommandesControllerTests
    {
        private OrdersDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<OrdersDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new OrdersDbContext(options);
            context.Commandes.Add(new Commande
            {
                IdCommande = 1,
                Createdate = DateTime.Now,
                IdClient = 42
            });
            context.SaveChanges();

            return context;
        }

        private CommandesController GetControllerWithAuth(OrdersDbContext context)
        {
            var controller = new CommandesController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, "testuser")
                    }, "TestAuthType"))
                }
            };
            return controller;
        }

        [Fact]
        public async Task GetCommandes_ReturnsAllCommandes()
        {
            var context = GetDbContext();
            var controller = GetControllerWithAuth(context);

            var result = await controller.GetCommandes();

            result.Result.Should().BeNull();
            result.Value.Should().NotBeNull().And.HaveCount(1);
        }

        [Fact]
        public async Task GetCommande_WithValidId_ReturnsCommande()
        {
            var context = GetDbContext();
            var controller = GetControllerWithAuth(context);

            var result = await controller.GetCommande(1);

            // Vérifie que la valeur n'est pas nulle
            result.Value.Should().NotBeNull();

            // Vérifie que c'est bien la commande attendue
            result.Value.IdCommande.Should().Be(1);
        }


        [Fact]
        public async Task PostCommande_AddsCommande()
        {
            var context = GetDbContext();
            var controller = GetControllerWithAuth(context);

            var newCommande = new Commande
            {
                IdCommande = 2,
                Createdate = DateTime.Now,
                IdClient = 99
            };

            var result = await controller.PostCommande(newCommande);

            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result.Result as CreatedAtActionResult;
            var createdCommande = createdResult.Value as Commande;

            createdCommande.Should().NotBeNull();
            createdCommande.IdClient.Should().Be(99);
        }

        [Fact]
        public async Task PutCommande_UpdatesCommande()
        {
            var context = GetDbContext();
            var controller = GetControllerWithAuth(context);

            // Détacher l'entité existante pour éviter le suivi multiple
            var existingCommande = await context.Commandes.FindAsync(1);
            context.Entry(existingCommande!).State = EntityState.Detached;

            var updatedCommande = new Commande
            {
                IdCommande = 1,
                Createdate = DateTime.Now,
                IdClient = 100
            };

            var result = await controller.PutCommande(1, updatedCommande);

            result.Should().BeOfType<NoContentResult>();

            var updated = await context.Commandes.FindAsync(1);
            updated.IdClient.Should().Be(100);
        }

        [Fact]
        public async Task DeleteCommande_RemovesCommande()
        {
            var context = GetDbContext();
            var controller = GetControllerWithAuth(context);

            var result = await controller.DeleteCommande(1);

            result.Should().BeOfType<NoContentResult>();

            var deleted = await context.Commandes.FindAsync(1);
            deleted.Should().BeNull();
        }
    }
}
