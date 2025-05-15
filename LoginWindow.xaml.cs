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
using Cryptography;
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
            try
            {

                using (GerenciamentoContext context = new GerenciamentoContext())
                {
                    UsuariosModel userFound = context.Usuarios.FirstOrDefault(context => context.Usuario == UserTxt.Text);
                    bool userFoundCheck = VerificarUsuario(userFound);

                    if (userFoundCheck)
                    {
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        return;
                    }
                }
            } catch(Exception ex)
            {
                MessageBox.Show("Erro ao conectar ao banco de dados: " + ex.Message);
            }
        }

        /// Método chamado quando o botão de fechar é clicado
        public void OnClose(object s, RoutedEventArgs e)
        {
            this.Close();
        }

        // Metodo para verificar se o usuário existe no banco de dados
        private bool VerificarUsuario(UsuariosModel user)
        {

            if (user == null)
            {
                MessageBox.Show("Usuário não encontrado!");
                return false;
            }
            else
            {
                using (GerenciamentoContext context = new GerenciamentoContext())
                {
                    string password = user.Senha;
                    string passwordToCheck = PassTxt.Password;
                    bool isValid = PasswordManager.VerifyPassword(passwordToCheck, password);
                    if (isValid == false)
                    {
                        MessageBox.Show("Senha incorreta!");
                        return false;
                    }
                    else if (user.Administrador == "Não")
                    {
                        MessageBox.Show("Você não tem autorização para acessar essa área!");
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
            }
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
