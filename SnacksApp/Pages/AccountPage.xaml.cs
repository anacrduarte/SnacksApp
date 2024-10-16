using SnacksApp.Services;

namespace SnacksApp.Pages;

public partial class AccountPage : ContentPage
{
    private readonly ApiService _apiService;

    private const string NameUserKey = "username";
    private const string EmailUserKey = "useremail";
    private const string PhoneUserKey = "userphonenumber";
    public AccountPage(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        UploadUserInformation();
        ImgBtnProfile.Source = await GetImageProfileAsync();
    }

    private async Task<string?> GetImageProfileAsync()
    {
        string defaultImage = AppConfig.ProfileDefaultImage;

        var (response, errorMessage) = await _apiService.GetProfileImageUser();

        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    await DisplayAlert("Erro", "Não autorizado", "OK");
                    return defaultImage;
                default:
                    await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter a imagem.", "OK");
                    return defaultImage;
            }
        }

        if (response?.UrlImage is not null)
        {
            return response.ImagePath;
        }
        return defaultImage;

    }

    private void UploadUserInformation()
    {
        LblNameUser.Text = Preferences.Get(NameUserKey, string.Empty);
        EntName.Text = LblNameUser.Text;
        EntEmail.Text = Preferences.Get(EmailUserKey, string.Empty);
        EntPhone.Text = Preferences.Get(PhoneUserKey, string.Empty);
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        // Salva as informa  es alteradas pelo usu rio nas prefer ncias
        Preferences.Set(NameUserKey, EntName.Text);
        Preferences.Set(EmailUserKey, EntEmail.Text);
        Preferences.Set(PhoneUserKey, EntPhone.Text);
        await DisplayAlert("Informações guardadas", "Suas informações foram guardadas com sucesso!", "OK");

    }
}