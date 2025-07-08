using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using MSPR_bloc_4_orders.Data;
using MSPR_bloc_4_orders.Controllers;
using MSPR_bloc_4_orders.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using MSPR_bloc_4_orders.Services;

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
            IRabbitMqPublisher fakePublisher = new FakeRabbitMqPublisher(); // ✅ utilise l'interface
            var controller = new CommandesController(context, fakePublisher);
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

            result.Value.Should().NotBeNull();
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

        [Fact]
        public async Task GetProduitCommandes_ReturnsAll()
        {
            var context = GetDbContext();
            context.ProduitCommandes.Add(new ProduitCommande
            {
                IdProduitcommande = 1,
                IdCommande = 1,
                IdProduit = 100,
                Nom = "Produit Test",
                Quantite = 5,
                CreatedAt = DateTime.Now
            });
            context.SaveChanges();

            var controller = new ProduitCommandesController(context, new FakeRabbitMqPublisher());

            var result = await controller.GetProduitCommandes();

            result.Value.Should().NotBeNull().And.HaveCount(1);
        }

        [Fact]
        public async Task GetProduitCommande_ById_ReturnsEntity()
        {
            var context = GetDbContext();
            context.ProduitCommandes.Add(new ProduitCommande
            {
                IdProduitcommande = 2,
                IdCommande = 1,
                IdProduit = 101,
                Nom = "Produit X",
                Quantite = 3,
                CreatedAt = DateTime.Now
            });
            context.SaveChanges();

            var controller = new ProduitCommandesController(context, new FakeRabbitMqPublisher());

            var result = await controller.GetProduitCommande(2);

            result.Value.Should().NotBeNull();
            result.Value.IdProduitcommande.Should().Be(2);
        }

        [Fact]
        public async Task PostProduitCommandes_AddsEntities()
        {
            var context = GetDbContext();
            var controller = new ProduitCommandesController(context, new FakeRabbitMqPublisher());

            var produits = new List<ProduitCommande>
    {
        new ProduitCommande { IdProduitcommande = 3, IdCommande = 1, IdProduit = 102, Quantite = 2, CreatedAt = DateTime.Now },
        new ProduitCommande { IdProduitcommande = 4, IdCommande = 1, IdProduit = 103, Quantite = 4, CreatedAt = DateTime.Now }
    };

            var result = await controller.PostProduitCommandes(produits);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var returnedList = okResult.Value as List<ProduitCommande>;
            returnedList.Should().NotBeNull().And.HaveCount(2);
        }

        [Fact]
        public async Task PutProduitCommande_UpdatesEntity()
        {
            var context = GetDbContext();
            context.ProduitCommandes.Add(new ProduitCommande
            {
                IdProduitcommande = 5,
                IdCommande = 1,
                IdProduit = 104,
                Nom = "Produit Update",
                Quantite = 1,
                CreatedAt = DateTime.Now
            });
            context.SaveChanges();

            var controller = new ProduitCommandesController(context, new FakeRabbitMqPublisher());

            var updatedProduit = new ProduitCommande
            {
                IdProduitcommande = 5,
                IdCommande = 1,
                IdProduit = 104,
                Nom = "Produit Updated",
                Quantite = 10,
                CreatedAt = DateTime.Now
            };

            var result = await controller.PutProduitCommande(5, updatedProduit);

            result.Should().BeOfType<NoContentResult>();

            var entity = await context.ProduitCommandes.FindAsync(5);
            entity.Quantite.Should().Be(10);
            entity.Nom.Should().Be("Produit Updated");
        }

        [Fact]
        public async Task DeleteProduitCommande_RemovesEntity()
        {
            var context = GetDbContext();
            context.ProduitCommandes.Add(new ProduitCommande
            {
                IdProduitcommande = 6,
                IdCommande = 1,
                IdProduit = 105,
                Nom = "Produit Delete",
                Quantite = 1,
                CreatedAt = DateTime.Now
            });
            context.SaveChanges();

            var controller = new ProduitCommandesController(context, new FakeRabbitMqPublisher());

            var result = await controller.DeleteProduitCommande(6);

            result.Should().BeOfType<NoContentResult>();

            var entity = await context.ProduitCommandes.FindAsync(6);
            entity.Should().BeNull();
        }

    }
}
