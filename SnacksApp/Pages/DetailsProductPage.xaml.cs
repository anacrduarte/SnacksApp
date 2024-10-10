using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp.Pages;

public partial class DetailsProductPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private int _productId;
    private bool _loginPageDisplayed = false;
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

    private void ImgBtnFavorite_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnRemove_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnAdd_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnAddToCart_Clicked(object sender, EventArgs e)
    {

    }
}