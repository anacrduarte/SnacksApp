using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp.Pages;

public partial class OrderDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    private bool _loginPageDisplayed = false;

    public OrderDetailsPage(int orderId,
                              decimal totalValue, ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;

        LblTotalPrice.Text = " €" + totalValue;

        GetOrderDetails(orderId);
    }

    private async void GetOrderDetails(int orderId)
    {
        try
        {
            loadIndicator.IsRunning = true;
            loadIndicator.IsVisible = true;
            var (orderDetails, errorMessage) = await _apiService.GetOrderDetails(orderId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }

            if (orderDetails is null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter os detalhes do pedido.", "OK");
                return;
            }
            else
            {
                CvOrderDetails.ItemsSource = orderDetails;
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Erro", "Ocorreu um erro ao obter os detalhes. Tente novamente mais tarde.", "OK");
        }
        finally
        {
            // Esconde o indicador de carregamento
            loadIndicator.IsRunning = false;
            loadIndicator.IsVisible = false;
        }

    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}