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
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Cliente), "ValidarDataNascimento")]
        public DateTime? DataNascimento { get; set; }

        public static ValidationResult? ValidarDataNascimento(DateTime? data, ValidationContext context)
        {
            if (data == null) return ValidationResult.Success;
            if (data.Value.Year < 1900 || data.Value > DateTime.Today)
                return new ValidationResult("Data de nascimento inválida. Deve ser entre 1900 e hoje.");
            return ValidationResult.Success;
        }

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