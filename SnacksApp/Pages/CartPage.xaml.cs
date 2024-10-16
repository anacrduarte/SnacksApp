using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;
using System.Collections.ObjectModel;
using static System.Net.Mime.MediaTypeNames;

namespace SnacksApp.Pages;

public partial class CartPage : ContentPage
{
    private bool _loginPageDisplayed = false;
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    private bool _isNavigatingToEmptyCartPage = false;

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
        

        if (IsNavigatingToEmptyCartPage()) return;

        bool hasItems = await GetShoppingCartItem();

        if (hasItems)
        {
            DisplayAddress();
        }
        else
        {
            await NavigateToEmptyCart();
        }

    }


    private bool IsNavigatingToEmptyCartPage()
    {
        if (_isNavigatingToEmptyCartPage)
        {
            _isNavigatingToEmptyCartPage = false;
            return true;
        }
        return false;
    }
    private void DisplayAddress()
    {
        bool saveAddress = Preferences.ContainsKey("address");

        if (saveAddress)
        {
            string name = Preferences.Get("name", string.Empty);
            string address = Preferences.Get("address", string.Empty);
            string phoneNumber = Preferences.Get("phonenumber", string.Empty);

            LblAddress.Text = $"{name}\n {address}\n{phoneNumber}";
        }
        else
        {
             LblAddress.Text = "Informe a sua morada.";
        }
    }

    private async Task NavigateToEmptyCart()
    {
        LblAddress.Text = string.Empty;
        _isNavigatingToEmptyCartPage = true;
        await Navigation.PushAsync(new EmptyCartPage());
    }

    private async void BtnDecrease_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is ShoppingCartItem itemCart)
        {
            if (itemCart.Quantity == 1) return;
            else
            {
                itemCart.Quantity--;
                UpdateTotalPrice();
                await _apiService.UpdateShoppingCartItemQuantity(itemCart.ProductId, "diminuir");
            }
        }
    }

    private async void BtnIncrease_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is ShoppingCartItem itemCart)
        {
            itemCart.Quantity++;
            UpdateTotalPrice();
            await _apiService.UpdateShoppingCartItemQuantity(itemCart.ProductId, "aumentar");
        }
    }

    private async void BtnDelete_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is ShoppingCartItem itemCart)
        {
            bool response = await DisplayAlert("Confirmar",
                          "Tem certeza que deseja excluir este item do carrinho?", "Sim", "Não");
            if (response)
            {
                ShoppingCartItems.Remove(itemCart);
                UpdateTotalPrice();
                await _apiService.UpdateShoppingCartItemQuantity(itemCart.ProductId, "deletar");
            }
        }
    }

    private void BtnEditAddress_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AddressPage());

    }

    private async void TapConfirmOrder_Tapped(object sender, TappedEventArgs e)
    {
        if (ShoppingCartItems == null || !ShoppingCartItems.Any())
        {
            await DisplayAlert("Informação", "Seu carrinho está vazio ou o pedido já foi confirmado.", "OK");
            return;
        }

        var order = new Order()
        {
            Adress = LblAddress.Text,
            UserId = Preferences.Get("userid", 0),
            TotalValue = Convert.ToDecimal(LblTotalPrice.Text)
        };

        var response = await _apiService.ConfirmOrder(order);

        if (response.HasError)
        {
            if (response.ErrorMessage == "Unauthorized")
            {
                // Redirecionar para a p gina de login
                await DisplayLoginPage();
                return;
            }
            await DisplayAlert("Opa !!!", $"Algo deu errado: {response.ErrorMessage}", "Cancelar");
            return;
        }

        ShoppingCartItems.Clear();
        LblAddress.Text = "Informe o seu endereço";
       LblTotalPrice.Text = "0.00";

        await Navigation.PushAsync(new ConfirmedOrderPage());


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
            UpdateTotalPrice(); // Atualizar o preco total ap?s atualizar os itens do carrinho

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

    private void UpdateTotalPrice()
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