using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocadoraApi.Models
{
    public class Veiculo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Modelo { get; set; }

        [Required]
        [Range(1900, 2100, ErrorMessage = "Ano de fabricação inválido.")]
        public int AnoFabricacao { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "A quilometragem não pode ser negativa.")]
        public double Quilometragem { get; set; }

        // Chave estrangeira -> Fabricante
        [Required]
        [ForeignKey(nameof(Fabricante))]
        public int FabricanteId { get; set; }

        public Fabricante? Fabricante { get; set; }

        // Navegação (1:N) — um veículo pode ter vários aluguéis ao longo do tempo
        public ICollection<Aluguel>? Alugueis { get; set; }
    }
}