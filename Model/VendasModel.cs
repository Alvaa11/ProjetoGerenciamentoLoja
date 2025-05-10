using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjetoLojaAutoPeça.Model
{
    public class VendasModel
    {

        public int VendaId { get; set; }
        public DateTime Data { get; set; }

        public int ProdutoId { get; set; }
        public ProdutosModel? Produto { get; set; }

        public int UsuarioId { get; set; }
        public UsuariosModel? Usuario { get; set; }

        public string? FormaDePagamento { get; set; }

        public int Quantidade { get; set; }
        public double Total => Quantidade * Produto.Preco;

    }
}
