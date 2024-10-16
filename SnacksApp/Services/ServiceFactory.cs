using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnacksApp.Services
{
    public class ServiceFactory
    {
        public static FavoriteService CreateFavoritesService()
        {
            return new FavoriteService();
        }
    }
}
