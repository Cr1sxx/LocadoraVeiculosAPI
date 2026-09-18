using LocadoraApi.Data;
using LocadoraApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return Ok(await _context.Clientes.ToListAsync());
        }

        // GET: api/clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound($"Cliente com Id {id} não encontrado.");

            return Ok(cliente);
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cpfExiste = await _context.Clientes.AnyAsync(c => c.Cpf == cliente.Cpf);
            if (cpfExiste)
                return BadRequest("Já existe um cliente cadastrado com esse CPF.");

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
        }

        // PUT: api/clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente cliente)
        {
            if (id != cliente.Id)
                return BadRequest("O Id da rota difere do Id do corpo da requisição.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existente = await _context.Clientes.FindAsync(id);
            if (existente == null)
                return NotFound($"Cliente com Id {id} não encontrado.");

            existente.Nome = cliente.Nome;
            existente.Cpf = cliente.Cpf;
            existente.Email = cliente.Email;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound($"Cliente com Id {id} não encontrado.");

            var possuiAlugueis = await _context.Alugueis.AnyAsync(a => a.ClienteId == id);
            if (possuiAlugueis)
                return BadRequest("Não é possível excluir um cliente que possui aluguéis vinculados.");

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =====================================================================
        // FILTRO 3 [LEFT JOIN via GroupJoin + SelectMany/DefaultIfEmpty]
        // Relatório de clientes e seus aluguéis (inclusive quem nunca alugou)
        // GET: api/clientes/relatorio-alugueis
        // =====================================================================
        [HttpGet("relatorio-alugueis")]
        public async Task<ActionResult> GetRelatorioClientesAlugueis()
        {
            var resultado = await _context.Clientes
                .GroupJoin(
                    _context.Alugueis,
                    cliente => cliente.Id,
                    aluguel => aluguel.ClienteId,
                    (cliente, alugueis) => new { cliente, alugueis }
                )
                .SelectMany(
                    x => x.alugueis.DefaultIfEmpty(), // LEFT JOIN: mantém cliente mesmo sem aluguel
                    (x, aluguel) => new
                    {
                        ClienteId = x.cliente.Id,
                        x.cliente.Nome,
                        x.cliente.Cpf,
                        AluguelId = (int?)aluguel!.Id,
                        DataInicio = (DateTime?)aluguel.DataInicio,
                        DataFim = (DateTime?)aluguel.DataFim,
                        ValorTotal = (decimal?)aluguel.ValorTotal
                    }
                )
                .ToListAsync();

            return Ok(resultado);
        }
    }
}