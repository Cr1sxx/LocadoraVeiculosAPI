using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocadoraApi.Models
{
    public class Aluguel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Cliente))]
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        [Required]
        [ForeignKey(nameof(Veiculo))]
        public int VeiculoId { get; set; }
        public Veiculo? Veiculo { get; set; }

        [Required]
        public DateTime DataInicio { get; set; }

        [Required]
        public DateTime DataFim { get; set; }

        // Anulável -> aluguel "em andamento" quando null
        public DateTime? DataDevolucao { get; set; }

        [Required]
        public double KmInicial { get; set; }

        // Só é preenchido na devolução
        public double? KmFinal { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorDiaria { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorTotal { get; set; }

        public ICollection<Pagamento>? Pagamentos { get; set; }
    }
}