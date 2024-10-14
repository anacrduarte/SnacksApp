using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp.Pages;

public partial class FavoritePage : ContentPage
{
    private readonly FavoriteService _favoriteService;
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    public FavoritePage(ApiService apiService, IValidator validator, FavoriteService favoriteService)
	{
		InitializeComponent();
        _favoriteService = favoriteService;
        _apiService = apiService;
        _validator = validator;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetFavoriteProducts();
    }

    private async Task GetFavoriteProducts()
    {
        try
        {
            var favoriteProducts = await _favoriteService.ReadAllAsync();

            if (favoriteProducts is null || favoriteProducts.Count == 0)
            {
                CvProducts.ItemsSource = null;//limpa a lista atual
                LblWarning.IsVisible = true; //mostra o aviso
            }
            else
            {
                CvProducts.ItemsSource = favoriteProducts;
                LblWarning.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
        }
    }

    private void CvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as FavoriteProduct;

        if (currentSelection == null) return;

        Navigation.PushAsync(new DetailsProductPage(currentSelection.ProductId,
                                                     currentSelection.Name!,
                                                     _apiService, _validator, _favoriteService));

        ((CollectionView)sender).SelectedItem = null;
    }
}

 
