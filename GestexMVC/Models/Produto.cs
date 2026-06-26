using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestexMVC.Models
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        [Display(Name = "Nome do Produto")]
        public string Nome { get; set; }

        [StringLength(255)]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O preço de custo é obrigatório")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Preço de Custo")]
        public decimal PrecoCusto { get; set; }

        [Required(ErrorMessage = "O preço de venda é obrigatório")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Preço de Venda")]
        public decimal PrecoVenda { get; set; }

        [Required]
        [Display(Name = "Quantidade em Estoque")]
        public int QtdEstoque { get; set; }

        [Required]
        [Display(Name = "Estoque Mínimo")]
        public int EstoqueMinimo { get; set; }

        [Display(Name = "Categoria")]
        public int? CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;
    }
}