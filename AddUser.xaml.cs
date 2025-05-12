using System.Windows;
using ProjetoLojaAutoPeça.Context;
using ProjetoLojaAutoPeça.Model;

namespace ProjetoLojaAutoPeça
{
    /// <summary>
    /// Lógica interna para AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        UsuariosModel NewUser = new UsuariosModel();
        public AddUser()
        {
            InitializeComponent();
            LoadUsers();
            NovoUserGrid.DataContext = NewUser;
        }

        // Método para carregar os usuários do banco de dados no grid
        private void LoadUsers()
        {
            using (GerenciamentoContext context = new GerenciamentoContext())
            {
                var users= context.Usuarios.ToList();
                DataGridUsers.ItemsSource = users;
            }
        }

        // Método para adicionar um novo usuário
        private void Register(object s, RoutedEventArgs e)
        {
            using (GerenciamentoContext context = new GerenciamentoContext())
            {
                if (VerificarUsuario(NewUser) == true)
                {
                    MessageBox.Show("Usuário já existe! Por favor, escolha outro nome de usuário.");
                }
                else
                {
                    string isChecked = AdminCheckBox.IsChecked == true ? "Sim" : "Não";
                    UsuariosModel newUser = new UsuariosModel(UserTxt.Text, PassTxt.Password, isChecked);
                    context.Usuarios.Add(newUser);
                    context.SaveChanges();
                    LoadUsers();
                    NovoUserGrid.DataContext = NewUser;
                    MessageBox.Show("Usuário adicionado com sucesso!");
                }
            }
        }

        // Método para remover um usuário
        private void RemoveUser(object s, RoutedEventArgs e)
        {
            using (GerenciamentoContext context = new GerenciamentoContext())
            {
                var UserDelete = (s as FrameworkElement).DataContext as UsuariosModel;
                context.Usuarios.Remove(UserDelete);
                context.SaveChanges();
                LoadUsers();
            }
        }

        // Método para cancelar a operação
        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Método para verificar se o usuário já existe
        private bool VerificarUsuario(UsuariosModel user)
        {
            using (GerenciamentoContext context = new GerenciamentoContext())
            {
                bool userFound = context.Usuarios.Any(u => u.Usuario == UserTxt.Text);
                return userFound;
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
