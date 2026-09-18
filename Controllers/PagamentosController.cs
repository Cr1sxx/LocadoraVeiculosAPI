using LocadoraApi.Data;
using LocadoraApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PagamentosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/pagamentos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pagamento>>> GetPagamentos()
        {
            return Ok(await _context.Pagamentos.ToListAsync());
        }

        // GET: api/pagamentos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pagamento>> GetPagamento(int id)
        {
            var pagamento = await _context.Pagamentos.FindAsync(id);

            if (pagamento == null)
                return NotFound($"Pagamento com Id {id} não encontrado.");

            return Ok(pagamento);
        }

        // POST: api/pagamentos
        [HttpPost]
        public async Task<ActionResult<Pagamento>> PostPagamento(Pagamento pagamento)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var aluguelExiste = await _context.Alugueis.AnyAsync(a => a.Id == pagamento.AluguelId);
            if (!aluguelExiste)
                return BadRequest($"Aluguel com Id {pagamento.AluguelId} não existe.");

            _context.Pagamentos.Add(pagamento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPagamento), new { id = pagamento.Id }, pagamento);
        }

        // PUT: api/pagamentos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPagamento(int id, Pagamento pagamento)
        {
            if (id != pagamento.Id)
                return BadRequest("O Id da rota difere do Id do corpo da requisição.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existente = await _context.Pagamentos.FindAsync(id);
            if (existente == null)
                return NotFound($"Pagamento com Id {id} não encontrado.");

            existente.DataPagamento = pagamento.DataPagamento;
            existente.Metodo = pagamento.Metodo;
            existente.Valor = pagamento.Valor;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/pagamentos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePagamento(int id)
        {
            var pagamento = await _context.Pagamentos.FindAsync(id);
            if (pagamento == null)
                return NotFound($"Pagamento com Id {id} não encontrado.");

            _context.Pagamentos.Remove(pagamento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =====================================================================
        // FILTRO 5 [INNER JOIN via Include]
        // Histórico de pagamentos em um período, com dados do Aluguel e Cliente
        // GET: api/pagamentos/historico?inicio=2026-01-01&fim=2026-12-31
        // =====================================================================
        [HttpGet("historico")]
        public async Task<ActionResult> GetHistoricoPagamentos([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
        {
            if (fim < inicio)
                return BadRequest("A data final não pode ser anterior à data inicial.");

            var resultado = await _context.Pagamentos
                .Include(p => p.Aluguel)                 // INNER JOIN implícito
                    .ThenInclude(a => a!.Cliente)         // INNER JOIN implícito (encadeado)
                .Where(p => p.DataPagamento >= inicio && p.DataPagamento <= fim)
                .Select(p => new
                {
                    p.Id,
                    p.DataPagamento,
                    p.Metodo,
                    p.Valor,
                    AluguelId = p.Aluguel!.Id,
                    Cliente = p.Aluguel.Cliente!.Nome
                })
                .ToListAsync();

            return Ok(resultado);
        }
    }
}