using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace up_02.Models
{
    public class AdViewModel
    {
        public int AdId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string PriceText { get; set; }

        public string CityName { get; set; }

        public string CategoryName { get; set; }

        public string TypeName { get; set; }

        public string StatusName { get; set; }

        public ImageSource Image { get; set; }
    }
}
