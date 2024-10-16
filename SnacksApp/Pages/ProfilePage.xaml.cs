using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoriteService _favoriteService;
    private bool _loginPageDisplayed = false;
    public ProfilePage(ApiService apiService, IValidator validator, FavoriteService favoriteService)
    {
        InitializeComponent();
        LblUserName.Text = Preferences.Get("username", string.Empty);
        _apiService = apiService;
        _validator = validator;
        _favoriteService = favoriteService;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ImgBtnProfile.Source = await GetProfileImage();
    }

    private async Task<ImageSource> GetProfileImage()
    {
        // Obtenha a imagem padr o do AppConfig
        string defaultImage = AppConfig.ProfileDefaultImage;

        var (response, errorMessage) = await _apiService.GetProfileImageUser();

        // Lida com casos de erro
        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    if (!_loginPageDisplayed)
                    {
                        await DisplayLoginPage();
                        return null;
                    }
                    break;
                default:
                    await DisplayAlert("Erro", errorMessage ?? "N o foi possível obter a imagem.", "OK");
                    return defaultImage;
            }
        }

        if (response?.UrlImage is not null)
        {
            return response.ImagePath;
        }

        return defaultImage;

    }

    private async void ImgBtnProfile_Clicked(object sender, EventArgs e)
    {
        try
        {
            var imageArray = await SelectImageAsync();
            if (imageArray is null)
            {
                await DisplayAlert("Erro", "Não foi possível carregar a imagem", "Ok");
                return;
            }
            ImgBtnProfile.Source = ImageSource.FromStream(() => new MemoryStream(imageArray));

            var response = await _apiService.UploadUserImage(imageArray);
            if (response.Data)
            {
                await DisplayAlert("", "Imagem enviada com sucesso", "Ok");
            }
            else
            {
                await DisplayAlert("Erro", response.ErrorMessage ?? "Ocorreu um erro desconhecido", "Cancelar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "Ok");
        }


    }
    private async Task<byte[]?> SelectImageAsync()
    {
        try
        {
            var file = await MediaPicker.PickPhotoAsync();

            if (file is null) return null;

            using (var stream = await file.OpenReadAsync())
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Erro", "A funcionalidade não suportada no dispositivo", "Ok");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Erro", "Permissões não concedidas para acessar a camera ou galeria", "Ok");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao selecionar a imagem: {ex.Message}", "Ok");
        }
        return null;
    }

    private void TapOrders_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void MyAccount_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void Questions_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void BtnLogout_Clicked(object sender, EventArgs e)
    {

    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _favoriteService));
    }
}