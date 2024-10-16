using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp.Pages;

public partial class OrdersPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    private bool _loginPageDisplayed = false;
    public OrdersPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;

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
            loadIndicator.IsRunning = true;
            loadIndicator.IsVisible = true;
            var (orders, errorMessage) = await _apiService.GetOrderPerUser(Preferences.Get("userid", 0));

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }
            if (errorMessage == "NotFound")
            {
                await DisplayAlert("Aviso", "Não existem pedidos para o cliente.", "OK");
                return;
            }
            if (orders is null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter pedidos.", "OK");
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
        finally
        {
            // Esconde o indicador de carregamento
            loadIndicator.IsRunning = false;
            loadIndicator.IsVisible = false;
        }
    }


    private void CvOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        var selectedItem = e.CurrentSelection.FirstOrDefault() as OrdersPerUser;

        if (selectedItem == null) return;

        Navigation.PushAsync(new OrderDetailsPage(selectedItem.Id,
                                                    selectedItem.TotalValue,
                                                    _apiService,
                                                    _validator));

        ((CollectionView)sender).SelectedItem = null;


    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}