using LocadoraApi.Data;
using LocadoraApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlugueisController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AlugueisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/alugueis
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Aluguel>>> GetAlugueis()
        {
            return Ok(await _context.Alugueis.ToListAsync());
        }

        // GET: api/alugueis/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Aluguel>> GetAluguel(int id)
        {
            var aluguel = await _context.Alugueis.FindAsync(id);

            if (aluguel == null)
                return NotFound($"Aluguel com Id {id} não encontrado.");

            return Ok(aluguel);
        }

        // POST: api/alugueis
        [HttpPost]
        public async Task<ActionResult<Aluguel>> PostAluguel(Aluguel aluguel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == aluguel.ClienteId);
            if (!clienteExiste)
                return BadRequest($"Cliente com Id {aluguel.ClienteId} não existe.");

            var veiculoExiste = await _context.Veiculos.AnyAsync(v => v.Id == aluguel.VeiculoId);
            if (!veiculoExiste)
                return BadRequest($"Veículo com Id {aluguel.VeiculoId} não existe.");

            if (aluguel.DataFim < aluguel.DataInicio)
                return BadRequest("A data de fim não pode ser anterior à data de início.");

            _context.Alugueis.Add(aluguel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAluguel), new { id = aluguel.Id }, aluguel);
        }

        // PUT: api/alugueis/5
        [HttpPut("{id}")]
public async Task<IActionResult> PutAluguel(int id, Aluguel aluguel)
{
    if (id != aluguel.Id)
        return BadRequest("O Id da rota difere do Id do corpo da requisição.");

    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var existente = await _context.Alugueis.FindAsync(id);
    if (existente == null)
        return NotFound($"Aluguel com Id {id} não encontrado.");

    // Regra de negócio: devolução não pode ser antes do início
    if (aluguel.DataDevolucao.HasValue && aluguel.DataDevolucao < existente.DataInicio)
        return BadRequest("A data de devolução não pode ser anterior à data de início do aluguel.");

    // Regra de negócio: km final não pode ser menor que km inicial
    if (aluguel.KmFinal.HasValue && aluguel.KmFinal < existente.KmInicial)
        return BadRequest("A quilometragem final não pode ser menor que a quilometragem inicial.");

    existente.DataInicio = aluguel.DataInicio;
    existente.DataFim = aluguel.DataFim;
    existente.DataDevolucao = aluguel.DataDevolucao;
    existente.KmInicial = aluguel.KmInicial;
    existente.KmFinal = aluguel.KmFinal;
    existente.ValorDiaria = aluguel.ValorDiaria;
    existente.ValorTotal = aluguel.ValorTotal;

    await _context.SaveChangesAsync();
    return NoContent();
}

        // DELETE: api/alugueis/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAluguel(int id)
        {
            var aluguel = await _context.Alugueis.FindAsync(id);
            if (aluguel == null)
                return NotFound($"Aluguel com Id {id} não encontrado.");

            _context.Alugueis.Remove(aluguel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =====================================================================
        // FILTRO 2 [INNER JOIN via Include]
        // Aluguéis "Em Andamento" (DataDevolucao == null), com Cliente e Veículo
        // GET: api/alugueis/em-andamento
        // =====================================================================
        [HttpGet("em-andamento")]
        public async Task<ActionResult> GetAlugueisEmAndamento()
        {
            var resultado = await _context.Alugueis
                .Include(a => a.Cliente)  // INNER JOIN implícito
                .Include(a => a.Veiculo)  // INNER JOIN implícito
                .Where(a => a.DataDevolucao == null)
                .Select(a => new
                {
                    a.Id,
                    Cliente = a.Cliente!.Nome,
                    Veiculo = a.Veiculo!.Modelo,
                    a.DataInicio,
                    a.DataFim,
                    a.ValorDiaria
                })
                .ToListAsync();

            return Ok(resultado);
        }
    }
}