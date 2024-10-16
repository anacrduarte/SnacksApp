using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp.Pages;

public partial class OrdersPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoriteService _favoriteService;
    private bool _loginPageDisplayed = false;
    public OrdersPage(ApiService apiService, IValidator validator, FavoriteService favoriteService)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _favoriteService = favoriteService;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetOrdersList();
    }

    private async Task GetOrdersList()
    {
        try
        {
            var (orders, errorMessage) = await _apiService.GetOrderPerUser(Preferences.Get("userid", 0));

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }
            if (errorMessage == "NotFound")
            {
                await DisplayAlert("Aviso", "N�o existem pedidos para o cliente.", "OK");
                return;
            }
            if (orders is null)
            {
                await DisplayAlert("Erro", errorMessage ?? "N�o foi poss�vel obter pedidos.", "OK");
                return;
            }
            else
            {
                CvOrders.ItemsSource = orders;
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Erro", "Ocorreu um erro ao obter os pedidos. Tente novamente mais tarde.", "OK");
        }
    }


    private void CvOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        var selectedItem = e.CurrentSelection.FirstOrDefault() as OrdersPerUser;

        if (selectedItem == null) return;

        Navigation.PushAsync(new OrderDetailsPage(selectedItem.Id,
                                                    selectedItem.TotalValue,
                                                    _apiService,
                                                    _validator, _favoriteService));

        ((CollectionView)sender).SelectedItem = null;


    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _favoriteService));
    }
}