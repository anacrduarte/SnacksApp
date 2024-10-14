using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;
using System.Collections.ObjectModel;

namespace SnacksApp.Pages;

public partial class CartPage : ContentPage
{
    private bool _loginPageDisplayed = false;
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    private ObservableCollection<ShoppingCartItem> ShoppingCartItems = new ObservableCollection<ShoppingCartItem>();

    public CartPage(ApiService apiService,
                                IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetShoppingCartItem();
    }
    private void BtnDecrease_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnIncrease_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnDelete_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnEditAddress_Clicked(object sender, EventArgs e)
    {

    }

    private void TapConfirmOrder_Tapped(object sender, TappedEventArgs e)
    {

    }

    private async Task<bool> GetShoppingCartItem()
    {
        try
        {
            var userId = Preferences.Get("userid", 0);
            var (shoppingCartItems, errorMessage) = await
                     _apiService.GetShoppingCartItems(userId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                // Redirecionar para a p?gina de login
                await DisplayLoginPage();
                return false;
            }

            if (shoppingCartItems == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possivel obter os itens do carrinho de compra.", "OK");
                return false;
            }

            ShoppingCartItems.Clear();
            foreach (var item in shoppingCartItems)
            {
                ShoppingCartItems.Add(item);
            }

            CvShoppingCart.ItemsSource = ShoppingCartItems;
            AtualizaPrecoTotal(); // Atualizar o preco total ap?s atualizar os itens do carrinho

            if (!ShoppingCartItems.Any())
            {
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return false;
        }
    }

    private void AtualizaPrecoTotal()
    {
        try
        {
            var totalPrice = ShoppingCartItems.Sum(item => item.Price * item.Quantity);
            LblTotalPrice.Text = totalPrice.ToString();
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"Ocorreu um erro ao atualizar o pre?o total: {ex.Message}", "OK");
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

}