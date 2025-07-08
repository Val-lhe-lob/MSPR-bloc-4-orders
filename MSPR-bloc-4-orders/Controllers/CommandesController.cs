using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSPR_bloc_4_orders.Data;
using MSPR_bloc_4_orders.Models;
using Microsoft.AspNetCore.Authorization;

namespace MSPR_bloc_4_orders.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CommandesController : ControllerBase
    {
        private readonly OrdersDbContext _context;

        public CommandesController(OrdersDbContext context)
        {
            _context = context;
        }

        // GET: api/commandes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Commande>>> GetCommandes()
        {
            return await _context.Commandes.ToListAsync();
        }

        // GET: api/commandes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Commande>> GetCommande(int id)
        {
            var commande = await _context.Commandes.FindAsync(id);
            if (commande == null)
                return NotFound();

            return commande;
        }

        // POST: api/commandes
        [HttpPost]
        public async Task<ActionResult<Commande>> PostCommande(Commande commande)
        {
            commande.Createdate = DateTime.Now;
            _context.Commandes.Add(commande);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCommande), new { id = commande.IdCommande }, commande);
        }

        // PUT: api/commandes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommande(int id, Commande commande)
        {
            if (id != commande.IdCommande)
                return BadRequest();

            _context.Entry(commande).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Commandes.Any(e => e.IdCommande == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/commandes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommande(int id)
        {
            var commande = await _context.Commandes.FindAsync(id);
            if (commande == null)
                return NotFound();

            _context.Commandes.Remove(commande);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
