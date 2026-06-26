using System.ComponentModel.DataAnnotations;

namespace GestexMVC.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        [Display(Name = "Nome da Categoria")]
        public string Nome { get; set; }

        [StringLength(255)]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        public List<Produto>? Produtos { get; set; }
    }
}