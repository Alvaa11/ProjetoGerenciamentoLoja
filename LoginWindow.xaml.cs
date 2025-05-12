using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ProjetoLojaAutoPeça.Context;
using ProjetoLojaAutoPeça.Model;

namespace ProjetoLojaAutoPeça
{
    /// <summary>
    /// Lógica interna para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        /// Método chamado quando o botão de login é clicado
        /// Verifica se o usuario e senha estão corretos, caso sim, abre a tela principal
        public void OnSubmit(object s, RoutedEventArgs e)
        {
            using (GerenciamentoContext context = new GerenciamentoContext())
            {
                bool userFound = context.Usuarios.Any(context => context.Usuario == UserTxt.Text &&
                                                            context.Senha == PassTxt.Password);
                if (userFound)
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Usuário ou senha inválidos.");
                }
            }
        }

        /// Método chamado quando o botão de fechar é clicado
        public void OnClose(object s, RoutedEventArgs e)
        {
            this.Close();
        }

        /// Método chamado quando o botão esquerdo do mouse é segurado e arrastado
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {

                this.DragMove();
            }
        }
    }
}
