using System.Windows;
using ProjetoLojaAutoPeça.Context;
using ProjetoLojaAutoPeça.Model;
using Cryptography;

namespace ProjetoLojaAutoPeça
{
    /// <summary>
    /// Lógica interna para AddUser.xaml
    /// </summary>
    public partial class loginAdmin : Window
    {
        public loginAdmin()
        {
            InitializeComponent();
        }

        // Verifica se o usuário é um administrador e abre a tela de adicionar usuário
        private void Open(object s, RoutedEventArgs e)
        {

            using (GerenciamentoContext context = new GerenciamentoContext())
            {
                var UserIsAdmin = context.Usuarios.FirstOrDefault(u => u.Usuario == UserTxt.Text);
                bool verifyUser = VerificarUsuario(UserIsAdmin);

                if (verifyUser == false)
                {
                    return;
                }
                else
                {
                    AddUser addUser = new AddUser();
                    addUser.Show();
                    this.Close();
                    return;
                }
            }
        }

        // Fecha a tela de login
        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Verifica se o usuário existe no banco de dados
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

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {

                this.DragMove();
            }
        }
    }
}
