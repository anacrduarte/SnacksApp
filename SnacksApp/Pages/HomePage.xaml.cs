using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;
using System.ComponentModel.DataAnnotations;

namespace SnacksApp.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    private bool _loginPageDisplayed = false;
    private bool _isDataLoaded = false;
    public HomePage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        LblNameUser.Text = "Olá, " + Preferences.Get("userName", string.Empty);
        _validator = validator;
        Title = AppConfig.titleHomePage;

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_isDataLoaded)
        {
            await LoadDataAsync();
            _isDataLoaded = true;
        }
    }

    private async Task LoadDataAsync()
    {
        var categoriasTask = GetListCategories();
        var maisVendidosTask = GetBestSelling();
        var popularesTask = GetPopular();

        await Task.WhenAll(categoriasTask, maisVendidosTask, popularesTask);
    }

    private void CvBestSellers_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
        {
            NavigateToProductDetailsPage(collectionView, e);
        }

    }

    private void CvPopular_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
        {
            NavigateToProductDetailsPage(collectionView, e);
        }
    }
    private void NavigateToProductDetailsPage(CollectionView collectionView, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Product;

        if (currentSelection == null)
            return;

        Navigation.PushAsync(new ListProductsPage(
                                 currentSelection.Id, currentSelection.Name!, _apiService, _validator
        ));

        collectionView.SelectedItem = null;

    }

    private void CvCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Category;

        if (currentSelection == null) return;


        Navigation.PushAsync(new ListProductsPage(currentSelection.Id,
                                                     currentSelection.Name!,
                                                     _apiService,
                                                     _validator));

        ((CollectionView)sender).SelectedItem = null;
    }



    private async Task<IEnumerable<Category>> GetListCategories()
    {
        try
        {
            var (categories, errorMessage) = await _apiService.GetCategories();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Category>();
            }

            if (categories == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter as categorias.", "OK");
                return Enumerable.Empty<Category>();
            }

            CvCategories.ItemsSource = categories;
            return categories;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Category>();
        }
    }

    private async Task<IEnumerable<Product>> GetBestSelling()
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("maisvendido", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (products == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter as categorias.", "OK");
                return Enumerable.Empty<Product>();
            }

           CvBestSellers.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    //-----------------

    private async Task<IEnumerable<Product>> GetPopular()
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("popular", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (products == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter as categorias.", "OK");
                return Enumerable.Empty<Product>();
            }
            CvPopular.ItemsSource = products;
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

}