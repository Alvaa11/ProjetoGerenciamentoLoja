using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjetoLojaAutoPeça.Model
{
    // Classe que representa o modelo de vendas
    public class VendasModel
    {

        public VendasModel()
        {
            
        }

        public VendasModel(string data, int mercadoria, string produto, string pagamento, int quantidade, double total)
        {
            Data = data;
            Mercadoria = mercadoria;
            Produto = produto;
            FormaDePagamento = pagamento;
            Quantidade = quantidade;
            Total = total;
        }

        [Key]
        public int VendaId { get; set; }
        public string Data { get; set; }

        public int Mercadoria { get; set; }
        public string Produto { get; set; }

        public string? FormaDePagamento { get; set; }

        public int Quantidade { get; set; }
        public double Total { get; set; }

    }
}
