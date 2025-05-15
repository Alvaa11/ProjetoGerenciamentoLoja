using System.Collections.ObjectModel;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ClosedXML.Excel;
using Microsoft.Win32;
using ProjetoLojaAutoPeça.Context;
using ProjetoLojaAutoPeça.Model;

namespace ProjetoLojaAutoPeça
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<VendasModel> Selecionados { get; set; } = new ObservableCollection<VendasModel>();
        ProdutosModel NewProduct = new ProdutosModel();
        ProdutosModel selectProduct = new ProdutosModel();
        ProdutosModel ProductToDelete = new ProdutosModel();

        public MainWindow()
        {
            InitializeComponent();
            LoadProducts();
            DataContext = this;
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

        // Abre a tela de cadastro de usuários
        public void OpenAddUser(object s, RoutedEventArgs e)
        {
            AddUser addUser = new AddUser();
            addUser.Show();
        }

        // Adiciona o produto no banco de dados
        public void AddItem(object s, RoutedEventArgs e)
        {
            if (mercadoriatxt.Text == null || nometxt.Text == null || precotxt.Text == null || estoquetxt.Text == null)
            {
                MessageBox.Show("Por favor, preencha todos os campos!");
                return;
            }
            else
            {
                int mercadoria;
                string nome = nometxt.Text;
                double preco;
                int estoque;

                bool converterMercadoria = int.TryParse(mercadoriatxt.Text, out mercadoria);
                bool converterPreco = double.TryParse(precotxt.Text, out preco);
                bool converterEstoque = int.TryParse(estoquetxt.Text, out estoque);

                if (!converterEstoque || !converterMercadoria || !converterPreco)
                {
                    MessageBox.Show("Por favor, insira uma mercadoria válida!");
                    return;
                };

                using (GerenciamentoContext context = new GerenciamentoContext())
                {
                    var newProduct = new ProdutosModel(mercadoria, nome, preco, estoque);
                    context.Produtos.Add(newProduct);
                    context.SaveChanges();
                    LoadProducts();
                    NovoProdutoGrid.DataContext = newProduct;
                    MessageBox.Show("Produto adicionado com sucesso!");
                    CampNewProduct.Visibility = Visibility.Hidden;
                }
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
                    var produto = context.Produtos.FirstOrDefault(p => p.Equals(selectProduct));
                    if (produto == null)
                    {
                        MessageBox.Show("Produto não encontrado! Por favor, verifique se o produto ainda existe na lista!");
                        return;
                    }
                    else
                    {
                        if(nomeupdatetxt.Text == "" || precoupdatetxt.Text == "" || quantidadeupdatetxt.Text == "")
                        {
                            MessageBox.Show("Por favor, preencha todos os campos!");
                            return;
                        }
                        else
                        {

                            string nome = nomeupdatetxt.Text;
                            double preco;
                            int quantidade;

                            bool convertPreco = double.TryParse(precoupdatetxt.Text, out preco);
                            bool convertQuantidade = int.TryParse(quantidadeupdatetxt.Text, out quantidade);
                            if (!convertPreco || !convertQuantidade)
                            {
                                MessageBox.Show("Por favor, insira uma mercadoria válida!");
                                return;
                            };   

                            produto.Nome = nome;
                            produto.Preco = preco;
                            produto.Estoque = quantidade;

                            context.Produtos.Update(produto);
                            context.SaveChanges();
                            LoadProducts();
                            MessageBox.Show("Produto atualizado com sucesso!");
                            CampUpdateProduct.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Preencha todos os campos corretamente.");
                var ProductUpdate = (s as FrameworkElement).DataContext as ProdutosModel;

            }
            nomeupdatetxt.Text = "";
            precoupdatetxt.Text = "";
            quantidadeupdatetxt.Text = "";
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
        DateTime data = DateTime.Now;

        // Registra a mercadoria e quantidade no grid
        private void Register(object s, RoutedEventArgs e)
        {
            if (TextRegister.Text == "" || TextQuantity.Text == "")
            {
                MessageBox.Show("Por favor, insira uma mercadoria válida!");
                return;
            }
            
            using (GerenciamentoContext context = new GerenciamentoContext())
            {
                int mercadoria;
                int quantidade;              

                bool converterMercadoria = int.TryParse(TextRegister.Text, out mercadoria);
                bool converterQuantidade = int.TryParse(TextQuantity.Text, out quantidade);
                var produto = context.Produtos.FirstOrDefault(p => p.Mercadoria == mercadoria);
                if (!converterMercadoria || !converterQuantidade)
                {
                    MessageBox.Show("Por favor, insira uma mercadoria válida!");
                    return;
                }
                else if (produto == null || produto.Mercadoria != mercadoria || produto.Estoque < 1 || produto.Estoque < quantidade)
                {
                    MessageBox.Show("Produto não encontrado ou estoque insuficiente!");
                    return;
                }
                else
                {
                    GridProdutos.RowDefinitions.Add(new RowDefinition());
                    var mercadoriaTextBlock = new TextBlock
                    {
                        Text = $"{mercadoria} -------- ",
                        FontSize = 18,
                        FontWeight = FontWeights.Bold,
                        FontFamily = new System.Windows.Media.FontFamily("Helvetica"),
                        FontStyle = FontStyles.Italic

                    };
                    Grid.SetRow(mercadoriaTextBlock, linhaAtualMercadoria);
                    Grid.SetColumn(mercadoriaTextBlock, 0);

                    var infoTextBLock = new TextBlock
                    {
                        Text = $"{produto.Nome} X {quantidade}",
                        FontSize = 18,
                        FontWeight = FontWeights.Bold,
                        FontFamily = new System.Windows.Media.FontFamily("Helvetica"),
                        FontStyle = FontStyles.Italic
                    };

                    Grid.SetRow(infoTextBLock, linhaAtualProdutos);
                    Grid.SetColumn(infoTextBLock, 1);


                    var venda = new VendasModel
                    {
                        Mercadoria = produto.Mercadoria,
                        Produto = produto.Nome,
                        Quantidade = quantidade,
                        Total = quantidade * produto.Preco
                    };

                    var removeButton = new Button
                    {
                        Content = "X",
                        Width = 30,
                        Height = 30,
                        Background = System.Windows.Media.Brushes.Red,
                        Foreground = System.Windows.Media.Brushes.White,
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        Tag = venda
                    };
                    Grid.SetRow(removeButton, linhaAtualProdutos);
                    Grid.SetColumn(removeButton, 2);
                    removeButton.Click += RemoverProduto;


                    GridProdutos.Children.Add(mercadoriaTextBlock);
                    GridProdutos.Children.Add(infoTextBLock);
                    GridProdutos.Children.Add(removeButton);
                    
                    linhaAtualMercadoria++;
                    linhaAtualProdutos++;
                    total = total + (produto.Preco * quantidade);
                    TotalInput.Text = $"Total: R${total:F2}";

                    Selecionados.Add(venda);
                    TextRegister.Text = "";
                    TextQuantity.Text = "";

                    return;
                }
            }
        }

        private void RemoverProduto(object s, RoutedEventArgs e)
        {
            var button = (Button)s;
            var venda = (VendasModel)button.Tag;

           if(venda != null)
            {
                Selecionados.Remove(venda);

                total -= venda.Total;
                TotalInput.Text = $"Total: R${total:F2}";

                var elementosParaRemover = GridProdutos.Children
                    .OfType<UIElement>()
                    .Where(el => Grid.GetRow(el) == Grid.GetRow(button))
                    .ToList();

                foreach(var el in elementosParaRemover)
                {
                    GridProdutos.Children.Remove(el);
                }

                int linhas = GridProdutos.RowDefinitions.Count;
                GridProdutos.RowDefinitions.RemoveAt(Grid.GetRow(button));
                for (int i = Grid.GetRow(button) + 1; i < linhas; i++)
                {
                    foreach( UIElement child in GridProdutos.Children)
                    {
                        if (Grid.GetRow(child) == i)
                        {
                            Grid.SetRow(child, i - 1);
                        }
                    }
                }

                linhaAtualMercadoria--;
                linhaAtualProdutos--;
            }
        }

        // Define a forma de pagamento
        private void PagamentoDinheiro(object s, RoutedEventArgs e)
        {
            pagamento = "Dinheiro";
            PagamentoDinheiroButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            PagamentoDinheiroButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);

            PagamentoCartaoDebitoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoCartaoDebitoButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            PagamentoCartaoCreditoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoCartaoCreditoButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            PagamentoPixButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoPixButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
        }
        private void PagamentoDebito(object s, RoutedEventArgs e)
        {
            pagamento = "Debito";
            PagamentoDinheiroButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoDinheiroButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            PagamentoCartaoDebitoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            PagamentoCartaoDebitoButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);

            PagamentoCartaoCreditoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoCartaoCreditoButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            PagamentoPixButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoPixButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
        }
        private void PagamentoCredito(object s, RoutedEventArgs e)
        {
            pagamento = "Credito";
            PagamentoDinheiroButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoDinheiroButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            PagamentoCartaoDebitoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoCartaoDebitoButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            PagamentoCartaoCreditoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            PagamentoCartaoCreditoButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);

            PagamentoPixButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoPixButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
        }
        private void PagamentoPix(object s, RoutedEventArgs e)
        {
            pagamento = "Pix";
            PagamentoDinheiroButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoDinheiroButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            PagamentoCartaoDebitoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoCartaoDebitoButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            PagamentoCartaoCreditoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            PagamentoCartaoCreditoButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            PagamentoPixButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            PagamentoPixButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
        }


        // Finaliza a venda e atualiza o estoque
        private void FinalizarVenda(object s, RoutedEventArgs e)
        {
            using (GerenciamentoContext context = new GerenciamentoContext())
            {
                string dataHoje = $"{data.Day}/{data.Month}/{data.Year} - {data.Hour}:{data.Minute}";


                if (total == 0 || pagamento == null)
                {
                    MessageBox.Show("Por favor, insira uma mercadoria ou forma de pagamento antes de finalizar a venda!");
                    return;
                }
                else
                {
                    foreach(var item in Selecionados)
                    {
                        int mercadoria = item.Mercadoria;
                        var produto = context.Produtos.FirstOrDefault(p => p.Mercadoria == mercadoria);
                        produto.Estoque = produto.Estoque - item.Quantidade;
                        context.Produtos.Update(produto);
                        var newVenda = new VendasModel(dataHoje, item.Mercadoria, item.Produto, pagamento, item.Quantidade, item.Total);

                        context.Vendas.Add(newVenda);
                    }

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
                    PagamentoDinheiroButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

                    PagamentoCartaoDebitoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
                    PagamentoCartaoDebitoButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

                    PagamentoCartaoCreditoButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
                    PagamentoCartaoCreditoButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

                    PagamentoPixButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
                    PagamentoPixButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

                    TotalInput.Text = $"Total: R$00,00";
                    GridProdutos.Children.Clear();
                    GridProdutos.RowDefinitions.Clear();
                    Selecionados.Clear();

                }
            }
        }

        // Gera planilha de excel com as vendas
        private void GerarRelatorioVendas(List<VendasModel> vendas)
        {
            using (var workbook = new XLWorkbook())
            {
                var aba = workbook.Worksheets.Add("Relatório de Vendas");

                // Cabeçalhos
                aba.Cell("A1").Value = "Data";
                aba.Cell("B1").Value = "Mercadoria";
                aba.Cell("C1").Value = "Produto";
                aba.Cell("D1").Value = "Forma de Pagamento";
                aba.Cell("E1").Value = "Quantidade";
                aba.Cell("F1").Value = "Total";

                int row = 2;
                foreach (var venda in vendas)
                {
                    aba.Cell(row, 1).Value = venda.Data;
                    aba.Cell(row, 2).Value = venda.Mercadoria;
                    aba.Cell(row, 3).Value = venda.Produto;
                    aba.Cell(row, 4).Value = venda.FormaDePagamento;
                    aba.Cell(row, 5).Value = venda.Quantidade;
                    aba.Cell(row, 6).Value = venda.Total;
                    row++;
                }

                var dialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    FileName = "RelatorioVendas.xlsx"
                };

                if (dialog.ShowDialog() == true)
                {
                    workbook.SaveAs(dialog.FileName);
                    MessageBox.Show("Relatório de vendas gerado com sucesso!");
                }
            }
        }


    


        // Verifica se o produto está cadastrado corretamente
        private bool ValidaDados(ProdutosModel product)
        {
            if (product == null || product.Nome == null || product.Preco == 0 || product.Estoque == 0) return false;
            return true;
        }

        private void DataRelatorio(object s, RoutedEventArgs e)
        {
            try
            {
                if (ComeçoPicker.Text == "" || FinalPicker.Text == "")
                {
                    MessageBox.Show("Por favor, selecione um intervalo de datas válido!");
                    return;
                }

                string dataInicio = ComeçoPicker.SelectedDate.Value.ToString("dd/MM/yyyy");
                string dataFinal = FinalPicker.SelectedDate.Value.ToString("dd/MM/yyyy");

                if (DateTime.Parse(dataInicio) > DateTime.Parse(dataFinal))
                {
                    MessageBox.Show("A data de início não pode ser maior que a data final!");
                    return;
                }
                else if (DateTime.Parse(dataInicio) > DateTime.Now || DateTime.Parse(dataFinal) > DateTime.Now)
                {
                    MessageBox.Show("As datas não podem ser maiores que a data de hoje!");
                    return;
                }

                var vendas = BuscarPelaData(dataInicio, dataFinal);
                GerarRelatorioVendas(vendas);
            }catch(Exception ex)
            {
                MessageBox.Show($"Erro ao gerar o relatorio: {ex.Message}");
            }
        }

        private List<VendasModel> BuscarPelaData(string datainicio, string dataFinal)
        {
            using (GerenciamentoContext context = new GerenciamentoContext())
            {
                var vendas = context.Vendas
                    .AsEnumerable()
                    .Where(v => DateTime.Parse(v.Data.Replace(" - ", " "))>= DateTime.Parse(datainicio.Replace(" - ", " "))&& DateTime.Parse(v.Data.Replace(" - ", " "))<= DateTime.Parse(dataFinal.Replace(" - ", " ")))
                    .ToList();
                

                return vendas;
            }
        }
    }   
}