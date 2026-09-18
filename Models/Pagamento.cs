using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocadoraApi.Models
{
    public class Pagamento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Aluguel))]
        public int AluguelId { get; set; }
        public Aluguel? Aluguel { get; set; }

        [Required]
        public DateTime DataPagamento { get; set; }

        [Required]
        [StringLength(20)]
        public string Metodo { get; set; } // PIX, Cartao, Boleto, Dinheiro...

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Valor { get; set; }
    }
}