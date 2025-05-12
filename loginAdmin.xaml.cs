using System.Windows;
using ProjetoLojaAutoPeça.Context;
using ProjetoLojaAutoPeça.Model;

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
                UsuariosModel UserIsAdmin = new UsuariosModel(UserTxt.Text, PassTxt.Password);

                if (VerificarUsuario(UserIsAdmin) == false)
                {
                    MessageBox.Show("Você não tem autorização para acessar essa área!");
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
            using (GerenciamentoContext context = new GerenciamentoContext())
            {
                bool userFound = context.Usuarios.Any(u => u.Usuario == UserTxt.Text &&
                                                                u.Senha == PassTxt.Password);
                return userFound;
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
