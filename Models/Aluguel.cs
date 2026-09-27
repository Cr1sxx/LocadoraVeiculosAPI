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
        [Range(0, double.MaxValue, ErrorMessage = "A quilometragem inicial não pode ser negativa.")]
        public double KmInicial { get; set; }

        // Só é preenchido na devolução
        [Range(0, double.MaxValue, ErrorMessage = "A quilometragem final não pode ser negativa.")]
        public double? KmFinal { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor da diária deve ser maior que zero.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorDiaria { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor total deve ser maior que zero.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorTotal { get; set; }

        public ICollection<Pagamento>? Pagamentos { get; set; }
    }
}