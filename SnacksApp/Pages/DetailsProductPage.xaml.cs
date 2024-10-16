using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp.Pages;

public partial class DetailsProductPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoriteService _favoriteService = new FavoriteService();
    private int _productId;
    private bool _loginPageDisplayed = false;
    private string? _imageUrl;
    public DetailsProductPage(int productId,
                                string productName,
                                ApiService apiService,
                                IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _productId = productId;
        Title = productName ?? "Detalhe do Produto";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProductDetails(_productId);
        UpdateFavoriteButton();
    }
    private async Task<Product?> GetProductDetails(int produtoId)
    {
        var (productDetail, errorMessage) = await _apiService.GetProductDetails(produtoId);

        if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
        {
            await DisplayLoginPage();
            return null;
        }

        // Verificar se houve algum erro na obtenção das produtos
        if (productDetail == null)
        {
            // Lidar com o erro, exibir mensagem ou logar
            await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter o produto.", "OK");
            return null;
        }

        if (productDetail != null)
        {
            // Atualizar as propriedades dos controles com os dados do produto
            ImageProduct.Source = productDetail.ImagePath;
            LblProductName.Text = productDetail.Name;
            LblProductPrice.Text = productDetail.Price.ToString();
            LblProductDescription.Text = productDetail.Details;
            LblTotalPrice.Text = productDetail.Price.ToString();
            _imageUrl = productDetail.ImagePath;
        }
        else
        {
            await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter os detalhes do produto.", "OK");
            return null;
        }
        return productDetail;
    }
    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private async void ImgBtnFavorite_Clicked(object sender, EventArgs e)
    {
        try
        {
            var existsFavorite = await _favoriteService.ReadAsync(_productId);
            if (existsFavorite is not null)
            {
                await _favoriteService.DeleteAsync(existsFavorite);
            }
            else
            {
                var favoriteProduct = new FavoriteProduct()
                {
                    ProductId = _productId,
                    IsFavorite = true,
                    Details = LblProductDescription.Text,
                    Name = LblProductName.Text,
                    Price = Convert.ToDecimal(LblProductPrice.Text),
                    ImageUrl = _imageUrl
                };

                await _favoriteService.CreateAsync(favoriteProduct);
            }
            UpdateFavoriteButton();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
        }

    }

    private async void UpdateFavoriteButton()
    {
        var existeFavorito = await
               _favoriteService.ReadAsync(_productId);

        if (existeFavorito is not null)
           ImgBtnFavorite.Source = "heartfill";
        else
            ImgBtnFavorite.Source = "heart";
    }

    private void BtnRemove_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantity.Text, out int quantity) &&
           decimal.TryParse(LblProductPrice.Text, out decimal unitPrice))
        {
            // Decrementa a quantidade, e n o permite que seja menor que 1
            quantity = Math.Max(1, quantity - 1);
            LblQuantity.Text = quantity.ToString();

            // Calcula o pre o total
            var totalPrice = quantity * unitPrice;
            LblTotalPrice.Text = totalPrice.ToString();
        }
        else
        {
            // Tratar caso as convers es falhem
            DisplayAlert("Erro", "Valores inválidos", "OK");
        }

    }

    private void BtnAdd_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantity.Text, out int quantity) &&
             decimal.TryParse(LblProductPrice.Text, out decimal unitPrice))
        {
            // Incrementa a quantidade
            quantity++;
            LblQuantity.Text = quantity.ToString();

            // Calcula o pre o total
            var totalPrice = quantity * unitPrice;
            LblTotalPrice.Text = totalPrice.ToString(); // Formata como moeda
        }
        else
        {
            // Tratar caso as convers es falhem
            DisplayAlert("Erro", "Valores inválidos", "OK");
        }
    }

    private async void BtnAddToCart_Clicked(object sender, EventArgs e)
    {
        try
        {
            var shoppingCart = new CartItem()
            {
                Quantity = Convert.ToInt32(LblQuantity.Text),
                UnitPrice = Convert.ToDecimal(LblProductPrice.Text),
                TotalValue = Convert.ToDecimal(LblTotalPrice.Text),
                ProductId = _productId,
                ClientId = Preferences.Get("userid", 0)
            };
            var response = await _apiService.AddItemShoppingCart(shoppingCart);
            if (response.Data)
            {
                await DisplayAlert("Sucesso", "Item adicionado ao carrinho !", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Erro", $"Falha ao adicionar item: {response.ErrorMessage}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
        }
    }
}