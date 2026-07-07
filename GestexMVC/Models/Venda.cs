using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestexMVC.Models
{
    public class Venda
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Data da Venda")]
        public DateTime DataVenda { get; set; } = DateTime.Now;

        [Display(Name = "Cliente")]
        public int? ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        [Required]
        [Display(Name = "Funcionário")]
        public int FuncionarioId { get; set; }
        public Funcionario? Funcionario { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "EmAberto";

        [Required(ErrorMessage = "A forma de pagamento é obrigatória")]
        [Display(Name = "Forma de Pagamento")]
        public string FormaPagamento { get; set; } = "Dinheiro";

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Valor Recebido")]
        public decimal? ValorRecebido { get; set; }

        [StringLength(255)]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total")]
        public decimal Total { get; set; } = 0;

        public List<VendaItem> Itens { get; set; } = new List<VendaItem>();
    }
}