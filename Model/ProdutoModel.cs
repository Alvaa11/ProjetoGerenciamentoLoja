using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoLojaAutoPeça.Model
{
    // Classe que representa o modelo de produtos
    public class ProdutosModel
    {
        [Key]
        public int ProdutoId { get; set; }
        public int Mercadoria { get; set; }
        public string? Nome { get; set; }
        public double Preco { get; set; }
        public int Estoque { get; set; }
        
        public ICollection<VendasModel>? Vendas { get; set; }
    }
}
