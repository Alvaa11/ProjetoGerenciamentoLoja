using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ProjetoLojaAutoPeça.Context;
using ProjetoLojaAutoPeça.Model;

namespace ProjetoLojaAutoPeça
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ProdutosModel NewProduct = new ProdutosModel();
        ProdutosModel selectProduct = new ProdutosModel();
        ProdutosModel ProductToDelete = new ProdutosModel();

        public MainWindow()
        {
            InitializeComponent();
            LoadProducts();
            NovoProdutoGrid.DataContext = NewProduct;
        }

        private void VerifyUser(object s, RoutedEventArgs e)
        {
            loginAdmin login = new loginAdmin();
            login.Show();
        }

        // Carrega os produtos do banco de dados
        private void LoadProducts()
        {
            using (GerenciamentoContext context = new GerenciamentoContext())
            {
                var products = context.Produtos.ToList();
                DataGridProdutos.ItemsSource = products;
            }
        }

        // Filtra os produtos pelo nome
        private void FilterBy(object s, RoutedEventArgs e)
        {
            string filter = SearchProduct.Text.ToLower();
            using (GerenciamentoContext context = new GerenciamentoContext())
            {
                var products = context.Produtos.Where(p => p.Nome.ToLower().Contains(filter)).ToList();
                DataGridProdutos.ItemsSource = products;
            }
        }

        // Abre a tela de cadastro de produtos
        public void Button_Click(object s, RoutedEventArgs e)
        {
            CampNewProduct.Visibility = Visibility.Visible;
        }
        public void OpenAddUser(object s, RoutedEventArgs e)
        {
            AddUser addUser = new AddUser();
            addUser.Show();
        }

        // Adiciona o produto no banco de dados
        public void AddItem(object s, RoutedEventArgs e)
        {
            if (ValidaDados(NewProduct))
            {
                using (GerenciamentoContext context = new GerenciamentoContext())
                {
                    context.Produtos.Add(NewProduct);
                    context.SaveChanges();
                    LoadProducts();
                    NewProduct = new ProdutosModel();
                    NovoProdutoGrid.DataContext = NewProduct;
                    MessageBox.Show("Produto adicionado com sucesso!");
                    CampNewProduct.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                MessageBox.Show("Preencha todos os campos corretamente.");

            }
        }

        // Remove o produto selecionado
        public void RemoveItem(object s, RoutedEventArgs e)
        {
            using (GerenciamentoContext context = new GerenciamentoContext())
            {
                ProductToDelete = (s as FrameworkElement).DataContext as ProdutosModel;
                bool searchProduct = context.Produtos.Any(p => p.Equals(ProductToDelete));
                if (searchProduct == false)
                {
                    MessageBox.Show("Não é possível excluir um produto inexistente!");
                    return;
                }
                else
                {
                    var ProductDelete = (s as FrameworkElement).DataContext as ProdutosModel;

                    context.Produtos.Remove(ProductDelete);
                    context.SaveChanges();
                    LoadProducts();
                }
            }
            
        }

        // Abre a tela de edição do produto
        private void SelectProdutoEditar(object s, RoutedEventArgs e)
        {
            selectProduct = (s as FrameworkElement).DataContext as ProdutosModel;
            EditarProdutoGrid.DataContext = selectProduct;
            CampUpdateProduct.Visibility = Visibility.Visible;
        }
        // Atualiza o produto selecionado
        public void UpdateItem(object s, RoutedEventArgs e)
        {
            if (ValidaDados(selectProduct))
            {
                using (GerenciamentoContext context = new GerenciamentoContext())
                {
                    bool searchProduct = context.Produtos.Any(p => p.Equals(selectProduct));
                    if (searchProduct == false)
                    {
                        MessageBox.Show("Produto não encontrado! Por favor, verifique se o produto ainda existe na lista!");
                        return;
                    }
                    else
                    {
                        context.Update(selectProduct);
                        var ProductUpdate = (s as FrameworkElement).DataContext as ProdutosModel;
                        context.Produtos.Update(ProductUpdate);
                        context.SaveChanges();
                        LoadProducts();
                        MessageBox.Show("Produto atualizado com sucesso!");
                        CampUpdateProduct.Visibility = Visibility.Hidden;
                    }
                }
            }
            else
            {
                MessageBox.Show("Preencha todos os campos corretamente.");
                var ProductUpdate = (s as FrameworkElement).DataContext as ProdutosModel;

            }
        }

        // Fecha a tela de cadastro de produtos
        private void HiddenEstoque(object s, RoutedEventArgs e)
        {
            NewButton.Visibility = Visibility.Hidden;
            SearchProduct.Visibility = Visibility.Hidden;
            SearchButton.Visibility = Visibility.Hidden;
            DataGridProdutos.Visibility = Visibility.Hidden;

            CampRegister.Visibility = Visibility.Visible;
            ProductsList.Visibility = Visibility.Visible;
            PaymentsList.Visibility = Visibility.Visible;
            Utilities.Visibility = Visibility.Visible;

            if (CampNewProduct.Visibility == Visibility.Visible)
            {
                CampNewProduct.Visibility = Visibility.Hidden;
            }
            if (CampUpdateProduct.Visibility == Visibility.Visible)
            {
                CampUpdateProduct.Visibility = Visibility.Hidden;
            }
        }

        // Fecha a tela de caixa
        private void HiddenCaixa(object s, RoutedEventArgs e)
        {
            NewButton.Visibility = Visibility.Visible;
            SearchProduct.Visibility = Visibility.Visible;
            SearchButton.Visibility = Visibility.Visible;
            DataGridProdutos.Visibility = Visibility.Visible;

            CampRegister.Visibility = Visibility.Hidden;
            ProductsList.Visibility = Visibility.Hidden;
            PaymentsList.Visibility = Visibility.Hidden;
            Utilities.Visibility = Visibility.Hidden;

        }


        private int linhaAtualMercadoria = 0;
        private int linhaAtualProdutos = 0;
        private double total = 0;
        private string pagamento;

        // Registra a mercadoria e quantidade no grid
        private void Register(object s, RoutedEventArgs e)
        {
            if (TextRegister.Text == "")
            {
                MessageBox.Show("Por favor, insira uma mercadoria válida!");
                return;
            }

            if (TextQuantity.Text == "")
            {
                MessageBox.Show("Por favor, insira uma quantidade válida!");
                return;
            }
            
            using (GerenciamentoContext context = new GerenciamentoContext())
            {
                int mercadoria = int.Parse(TextRegister.Text);
                int quantidade = int.Parse(TextQuantity.Text);
                var produto = context.Produtos.FirstOrDefault(p => p.Mercadoria == mercadoria);
                if (produto == null | produto.Estoque < 1 | produto.Estoque < quantidade)
                {
                    MessageBox.Show("Produto não encontrado ou sem estoque!");
                    return;
                }
                else
                {
                    
                    GridProdutos.RowDefinitions.Add(new RowDefinition());
                    var mercadoriaTextBlock = new TextBlock
                    {
                        Text = $"{mercadoria} -------- ",
                        FontSize = 15,
                        FontWeight = FontWeights.Bold,
                        FontFamily = new System.Windows.Media.FontFamily("Helvetica"),
                        FontStyle = FontStyles.Italic

                    };
                    Grid.SetRow(mercadoriaTextBlock, linhaAtualMercadoria);
                    Grid.SetColumn(mercadoriaTextBlock, 0);

                    var infoTextBLock = new TextBlock
                    {
                        Text = $"{produto.Nome} X {quantidade}",
                        FontSize = 15,
                        FontWeight = FontWeights.Bold,
                        FontFamily = new System.Windows.Media.FontFamily("Helvetica"),
                        FontStyle = FontStyles.Italic
                    };
                    Grid.SetRow(infoTextBLock, linhaAtualProdutos);
                    Grid.SetColumn(infoTextBLock, 1);

                    GridProdutos.Children.Add(mercadoriaTextBlock);
                    GridProdutos.Children.Add(infoTextBLock);
                    
                    linhaAtualMercadoria++;
                    linhaAtualProdutos++;
                    total = total + (produto.Preco * quantidade);
                    TotalInput.Text = $"Total: R${total:F2}";

                    return;
                }
            }
        }

        // Define a forma de pagamento
        private void PagamentoDinheiro(object s, RoutedEventArgs e)
        {
            pagamento = "Dinheiro";
            PagamentoDinheiroButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            PagamentoCartaoDebitoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoCartaoCreditoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoPixButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
        }
        private void PagamentoDebito(object s, RoutedEventArgs e)
        {
            pagamento = "Debito";
            PagamentoDinheiroButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoCartaoDebitoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            PagamentoCartaoCreditoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoPixButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
        }
        private void PagamentoCredito(object s, RoutedEventArgs e)
        {
            pagamento = "Credito";
            PagamentoDinheiroButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoCartaoDebitoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoCartaoCreditoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            PagamentoPixButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
        }
        private void PagamentoPix(object s, RoutedEventArgs e)
        {
            pagamento = "Pix";
            PagamentoDinheiroButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoCartaoDebitoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoCartaoCreditoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoPixButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
        }

        // Finaliza a venda e atualiza o estoque
        private void FinalizarVenda(object s, RoutedEventArgs e)
        {
            using (GerenciamentoContext context = new GerenciamentoContext())
            {
                DateTime data = DateTime.Now;
                string dataHoje = $"{data.Day}/{data.Month}/{data.Year} - {data.Hour}:{data.Minute}";

                var produto = context.Produtos.FirstOrDefault(p => p.Mercadoria == int.Parse(TextRegister.Text));

                if (total == 0 | pagamento == null)
                {
                    MessageBox.Show("Por favor, insira uma mercadoria ou forma de pagamento antes de finalizar a venda!");
                    return;
                }
                else
                {
                    produto.Estoque = produto.Estoque - int.Parse(TextQuantity.Text);
                    context.Produtos.Update(produto);

                    VendasModel newVenda = new VendasModel(dataHoje, produto.Mercadoria, produto.Nome, pagamento, int.Parse(TextQuantity.Text), total);
                    context.Vendas.Add(newVenda);
                    context.SaveChanges();
                    LoadProducts();
                    MessageBox.Show($"Venda finalizada com sucesso! Total: R$ {total:F2}; Pagamento em: {pagamento}; Data: {data.Day}/{data.Month}/{data.Year} - {data.Hour}:{data.Minute}");

                    total = 0;
                    linhaAtualMercadoria = 0;
                    linhaAtualProdutos = 0;
                    pagamento = "";
                    TextRegister.Text = "";
                    TextQuantity.Text = "";

                    PagamentoDinheiroButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
                    PagamentoCartaoDebitoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
                    PagamentoCartaoCreditoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
                    PagamentoPixButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);

                    TotalInput.Text = $"Total: R$00,00";
                    GridProdutos.Children.Clear();
                    GridProdutos.RowDefinitions.Clear();

                }
            }
        }

        private void GerarRelatorioVendas(object s, RoutedEventArgs e)
        {

        }

        private bool ValidaDados(ProdutosModel product)
        {
            if (product.Nome == null | product.Preco == 0 | product.Estoque == 0) return false;
            return true;
        }
    
    }   
}