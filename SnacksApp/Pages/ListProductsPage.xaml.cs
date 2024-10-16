using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp.Pages;

public partial class ListProductsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    private int _categoryId;
    private bool _loginPageDisplayed = false;

    public ListProductsPage(int categoryId, string categoryName, ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _categoryId = categoryId;
        Title = categoryName ?? "Produtos";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProductsList(_categoryId);
    }

    private async Task<IEnumerable<Product>> GetProductsList(int categoryId)
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("categoria", categoryId.ToString());

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

          
            if (products is null)
            {
                await DisplayAlert("Erro", errorMessage ?? "N�o foi poss�vel obter as categorias.", "OK");
                return Enumerable.Empty<Product>();
            }


            CvProducts.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void CvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Product;

        if (currentSelection is null)
            return;

        Navigation.PushAsync(new DetailsProductPage(currentSelection.Id,
                                                     currentSelection.Name!,
                                                     _apiService,
                                                     _validator));

        ((CollectionView)sender).SelectedItem = null;
    }
}