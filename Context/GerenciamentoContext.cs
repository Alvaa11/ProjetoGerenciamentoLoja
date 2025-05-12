using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjetoLojaAutoPeça.Model;

namespace ProjetoLojaAutoPeça.Context
{
    // Classe que representa o contexto do banco de dados
    public class GerenciamentoContext : DbContext
    {
        public DbSet<ProdutosModel> Produtos { get; set; }
        public DbSet<UsuariosModel> Usuarios { get; set; }
        public DbSet<VendasModel> Vendas { get; set; }

        // Construtor que recebe as opções do contexto
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProdutosModel>(p =>
            {
                p.HasKey(p => p.ProdutoId);
                p.Property(p => p.Mercadoria).IsRequired();
                p.Property(p => p.Nome).IsRequired();
                p.Property(p => p.Preco).IsRequired();
                p.Property(p => p.Estoque).IsRequired();
            });

            modelBuilder.Entity<UsuariosModel>(u =>
            {
                u.HasKey(u => u.UsuarioId);
                u.Property(u => u.Usuario).IsRequired();
                u.Property(u => u.Senha).IsRequired();
            });

            modelBuilder.Entity<VendasModel>(v =>
            {
                v.HasKey(v => v.VendaId);
                v.Property(v => v.Data).IsRequired();
                v.Property(v => v.Quantidade).IsRequired();
                v.Property(v => v.Produto).IsRequired();
                v.Property(v => v.FormaDePagamento).IsRequired();
                v.Property(v => v.Total).IsRequired();
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=C:\\Users\\010454164\\Desktop\\Projetos\\ProjetoLojaAutoPeça\\LojaAutoPeças.db");
            }
        }
    }
    
}
