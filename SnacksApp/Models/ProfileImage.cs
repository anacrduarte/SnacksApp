using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnacksApp.Models
{
    public class ProfileImage
    {
        public string? UrlImage { get; set; }
        public string? ImagePath => AppConfig.BaseUrl + UrlImage;
    }
}
