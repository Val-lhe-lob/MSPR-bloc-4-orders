using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSPR_bloc_4_orders.Data;
using MSPR_bloc_4_orders.Models;
using MSPR_bloc_4_orders.Services;
using MSPR_bloc_4_orders.Events;
using Microsoft.AspNetCore.Authorization;

namespace MSPR_bloc_4_orders.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProduitCommandesController : ControllerBase
    {
        private readonly OrdersDbContext _context;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;

        public ProduitCommandesController(OrdersDbContext context, IRabbitMqPublisher rabbitMqPublisher)
        {
            _context = context;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        // GET: api/produitcommandes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProduitCommande>>> GetProduitCommandes()
        {
            return await _context.ProduitCommandes.ToListAsync();
        }

        // GET: api/produitcommandes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProduitCommande>> GetProduitCommande(int id)
        {
            var produitCommande = await _context.ProduitCommandes.FindAsync(id);
            if (produitCommande == null)
                return NotFound();

            return produitCommande;
        }

        // POST: api/produitcommandes
        [HttpPost]
        public async Task<ActionResult<IEnumerable<ProduitCommande>>> PostProduitCommandes(List<ProduitCommande> produits)
        {
            foreach (var produit in produits)
            {
                produit.CreatedAt = DateTime.Now;
                _context.ProduitCommandes.Add(produit);
            }
            await _context.SaveChangesAsync();

            var idCommande = produits.First().IdCommande;
            var produitsCommande = await _context.ProduitCommandes
                .Where(pc => pc.IdCommande == idCommande)
                .ToListAsync();

            var orderEvent = new OrderCreatedEvent
            {
                OrderId = idCommande,
                Products = produitsCommande.Select(p => new ProductOrderItem
                {
                    ProductId = p.IdProduit,
                    Quantity = p.Quantite ?? 0
                }).ToList()
            };

            await _rabbitMqPublisher.PublishOrderCreated(orderEvent);

            return Ok(produitsCommande);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduitCommande(int id, ProduitCommande produitCommande)
        {
            if (id != produitCommande.IdProduitcommande)
            {
                return BadRequest();
            }

            try
            {
                var existingEntity = await _context.ProduitCommandes.FindAsync(id);
                if (existingEntity == null)
                {
                    return NotFound();
                }

                // Detach the existing tracked entity to avoid tracking conflict
                _context.Entry(existingEntity).State = EntityState.Detached;

                _context.Update(produitCommande);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProduitCommandeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // DELETE: api/produitcommandes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduitCommande(int id)
        {
            var produitCommande = await _context.ProduitCommandes.FindAsync(id);
            if (produitCommande == null)
                return NotFound();

            _context.ProduitCommandes.Remove(produitCommande);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProduitCommandeExists(int id)
        {
            return _context.ProduitCommandes.Any(pc => pc.IdProduitcommande == id);
        }
    }
}
