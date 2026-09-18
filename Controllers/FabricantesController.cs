using LocadoraApi.Data;
using LocadoraApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FabricantesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FabricantesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/fabricantes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Fabricante>>> GetFabricantes()
        {
            return Ok(await _context.Fabricantes.ToListAsync());
        }

        // GET: api/fabricantes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Fabricante>> GetFabricante(int id)
        {
            var fabricante = await _context.Fabricantes.FindAsync(id);

            if (fabricante == null)
                return NotFound($"Fabricante com Id {id} não encontrado.");

            return Ok(fabricante);
        }

        // POST: api/fabricantes
        [HttpPost]
        public async Task<ActionResult<Fabricante>> PostFabricante(Fabricante fabricante)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Fabricantes.Add(fabricante);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFabricante), new { id = fabricante.Id }, fabricante);
        }

        // PUT: api/fabricantes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFabricante(int id, Fabricante fabricante)
        {
            if (id != fabricante.Id)
                return BadRequest("O Id da rota difere do Id do corpo da requisição.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existente = await _context.Fabricantes.FindAsync(id);
            if (existente == null)
                return NotFound($"Fabricante com Id {id} não encontrado.");

            existente.Nome = fabricante.Nome;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/fabricantes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFabricante(int id)
        {
            var fabricante = await _context.Fabricantes.FindAsync(id);
            if (fabricante == null)
                return NotFound($"Fabricante com Id {id} não encontrado.");

            _context.Fabricantes.Remove(fabricante);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}