using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoLojaAutoPeça.Model
{
    // Classe que representa o modelo de usuários
    public class UsuariosModel
    {
        public UsuariosModel() { }

        public UsuariosModel(string usuario, string senha, string administrador = "Não")
        {
            Usuario = usuario;
            Senha = senha;
            Administrador = administrador;
        }

        [Key]
        public int UsuarioId { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string Administrador { get; set; }

        public ICollection<VendasModel>? Vendas { get; set; }
    }
}
