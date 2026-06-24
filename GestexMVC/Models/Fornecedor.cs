using System.ComponentModel.DataAnnotations;

namespace GestexMVC.Models
{
    public class Fornecedor
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A razão social é obrigatória")]
        [StringLength(100)]
        [Display(Name = "Razão Social")]
        public string RazaoSocial { get; set; }

        [Required(ErrorMessage = "O CNPJ é obrigatório")]
        [StringLength(18)]
        [Display(Name = "CNPJ")]
        public string CNPJ { get; set; }

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

        [StringLength(100)]
        [Display(Name = "Nome do Contato")]
        public string? NomeContato { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;
    }
}