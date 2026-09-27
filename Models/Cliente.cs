using System.ComponentModel.DataAnnotations;

namespace LocadoraApi.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter exatamente 11 dígitos numéricos.")]
        public string Cpf { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        public ICollection<Aluguel>? Alugueis { get; set; }
    }
}