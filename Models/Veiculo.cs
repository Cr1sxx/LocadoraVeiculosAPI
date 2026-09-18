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
        public int AnoFabricacao { get; set; }

        [Required]
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