using SnacksApp.Services;
using SnacksApp.Validations;
using System.ComponentModel.DataAnnotations;

namespace SnacksApp.Pages;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoriteService _favoriteService;

    public LoginPage(ApiService apiService, IValidator validator, FavoriteService favoriteService)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _favoriteService = favoriteService;
    }

    private async void BtnSignIn_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(EntEmail.Text))
        {
            await DisplayAlert("Erro", "Informe o email", "Cancelar");
            return;
        }

        if (string.IsNullOrEmpty(EntPassword.Text))
        {
            await DisplayAlert("Erro", "Informe a senha", "Cancelar");
            return;
        }

        var response = await _apiService.Login(EntEmail.Text, EntPassword.Text);

        if (!response.HasError)
        {
            Application.Current!.MainPage = new AppShell(_apiService, _validator, _favoriteService);
        }
        else
        {
            await DisplayAlert("Erro", "Algo deu errado", "Cancelar");
        }
    }

    private async void TapRegister_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(_apiService, _validator, _favoriteService));
    }

}