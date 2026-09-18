using System.ComponentModel.DataAnnotations;

namespace LocadoraApi.Models
{
    public class Fabricante
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        // Navegação (1:N) — um fabricante possui vários veículos
        public ICollection<Veiculo>? Veiculos { get; set; }
    }
}