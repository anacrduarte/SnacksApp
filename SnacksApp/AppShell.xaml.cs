using SnacksApp.Pages;
using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        private readonly FavoriteService _favoriteService;

        public AppShell(ApiService apiService, IValidator validator, FavoriteService favoriteService)
        {
            InitializeComponent();
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _validator = validator;
            _favoriteService = favoriteService;
            ConfigureShell();
        }

        private void ConfigureShell()
        {
            var homePage = new HomePage(_apiService, _validator, _favoriteService);
            var carrinhoPage = new CartPage(_apiService, _validator, _favoriteService);
            var favoritosPage = new FavoritePage(_apiService, _validator, _favoriteService);
            var perfilPage = new ProfilePage();

            Items.Add(new TabBar
            {
                Items =
            {
                new ShellContent { Title = "Home",Icon = "home",Content = homePage  },
                new ShellContent { Title = "Carrinho", Icon = "cart",Content = carrinhoPage },
                new ShellContent { Title = "Favoritos",Icon = "heart",Content = favoritosPage },
                new ShellContent { Title = "Perfil",Icon = "profile",Content = perfilPage }
            }
            });
        }
    }
}
