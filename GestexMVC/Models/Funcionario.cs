using System.ComponentModel.DataAnnotations;

namespace GestexMVC.Models
{
    public class Funcionario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(14)]
        [Display(Name = "CPF")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "O cargo é obrigatório")]
        [StringLength(50)]
        [Display(Name = "Cargo")]
        public string Cargo { get; set; }

        [StringLength(15)]
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [Display(Name = "E-mail")]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Data de Admissão")]
        public DateTime DataAdmissao { get; set; } = DateTime.Today;

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;
    }
}