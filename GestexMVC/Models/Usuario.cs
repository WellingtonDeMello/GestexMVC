using System.ComponentModel.DataAnnotations;

namespace GestexMVC.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O usuário é obrigatório")]
        [StringLength(50)]
        [Display(Name = "Usuário")]
        public string Login { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(255)]
        [Display(Name = "Senha")]
        public string Senha { get; set; }

        [Required]
        [Display(Name = "Perfil")]
        public string Perfil { get; set; } = "Operador"; // Admin ou Operador

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;
    }
}