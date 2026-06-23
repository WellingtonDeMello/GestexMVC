using System.ComponentModel.DataAnnotations;

namespace GestexMVC.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O tipo é obrigatório")]
        [Display(Name = "Tipo")]
        public string Tipo { get; set; } // "PF" ou "PJ"

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        [Display(Name = "Nome / Razão Social")]
        public string Nome { get; set; }

        [StringLength(14)]
        [Display(Name = "CPF")]
        public string? CPF { get; set; }

        [StringLength(18)]
        [Display(Name = "CNPJ")]
        public string? CNPJ { get; set; }

        [StringLength(20)]
        [Display(Name = "RG")]
        public string? RG { get; set; }

        [StringLength(18)]
        [Display(Name = "Inscrição Estadual")]
        public string? InscricaoEstadual { get; set; }

        [Display(Name = "Data de Nascimento")]
        public DateTime? DataNascimento { get; set; }

        [StringLength(15)]
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [Display(Name = "E-mail")]
        public string? Email { get; set; }

        [StringLength(200)]
        [Display(Name = "Endereço")]
        public string? Endereco { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;
    }
}