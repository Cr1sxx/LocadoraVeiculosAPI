using LocadoraApi.Data;
using LocadoraApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeiculosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VeiculosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/veiculos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Veiculo>>> GetVeiculos()
        {
            return Ok(await _context.Veiculos.ToListAsync());
        }

        // GET: api/veiculos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Veiculo>> GetVeiculo(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);

            if (veiculo == null)
                return NotFound($"Veículo com Id {id} não encontrado.");

            return Ok(veiculo);
        }

        // POST: api/veiculos
        [HttpPost]
        public async Task<ActionResult<Veiculo>> PostVeiculo(Veiculo veiculo)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fabricanteExiste = await _context.Fabricantes.AnyAsync(f => f.Id == veiculo.FabricanteId);
            if (!fabricanteExiste)
                return BadRequest($"Fabricante com Id {veiculo.FabricanteId} não existe.");

            _context.Veiculos.Add(veiculo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVeiculo), new { id = veiculo.Id }, veiculo);
        }

        // PUT: api/veiculos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVeiculo(int id, Veiculo veiculo)
        {
            if (id != veiculo.Id)
                return BadRequest("O Id da rota difere do Id do corpo da requisição.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existente = await _context.Veiculos.FindAsync(id);
            if (existente == null)
                return NotFound($"Veículo com Id {id} não encontrado.");

            existente.Modelo = veiculo.Modelo;
            existente.AnoFabricacao = veiculo.AnoFabricacao;
            existente.Quilometragem = veiculo.Quilometragem;
            existente.FabricanteId = veiculo.FabricanteId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/veiculos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVeiculo(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
                return NotFound($"Veículo com Id {id} não encontrado.");

            _context.Veiculos.Remove(veiculo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =====================================================================
        // FILTRO 1 [INNER JOIN via Include]
        // Todos os veículos trazendo os dados do Fabricante atrelado
        // GET: api/veiculos/com-fabricante
        // =====================================================================
        [HttpGet("com-fabricante")]
        public async Task<ActionResult> GetVeiculosComFabricante()
        {
            var resultado = await _context.Veiculos
                .Include(v => v.Fabricante) // INNER JOIN implícito
                .Select(v => new
                {
                    v.Id,
                    v.Modelo,
                    v.AnoFabricacao,
                    v.Quilometragem,
                    Fabricante = v.Fabricante!.Nome
                })
                .ToListAsync();

            return Ok(resultado);
        }

        // =====================================================================
        // FILTRO 4 [LEFT JOIN via GroupJoin + SelectMany/DefaultIfEmpty]
        // Veículos que NUNCA foram alugados
        // GET: api/veiculos/sem-aluguel
        // =====================================================================
        [HttpGet("sem-aluguel")]
        public async Task<ActionResult> GetVeiculosSemAluguel()
        {
            var resultado = await _context.Veiculos
                .GroupJoin(
                    _context.Alugueis,
                    veiculo => veiculo.Id,
                    aluguel => aluguel.VeiculoId,
                    (veiculo, alugueis) => new { veiculo, alugueis }
                )
                .SelectMany(
                    x => x.alugueis.DefaultIfEmpty(), // LEFT JOIN
                    (x, aluguel) => new { x.veiculo, aluguel }
                )
                .Where(x => x.aluguel == null) // filtra apenas os que não têm nenhum aluguel
                .Select(x => new
                {
                    x.veiculo.Id,
                    x.veiculo.Modelo,
                    x.veiculo.AnoFabricacao,
                    x.veiculo.Quilometragem
                })
                .ToListAsync();

            return Ok(resultado);
        }
    }
}