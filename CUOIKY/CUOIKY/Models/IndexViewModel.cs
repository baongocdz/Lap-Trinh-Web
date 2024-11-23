using CUOIKY.Repository.Models;

namespace CUOIKY.Models
{
    public class IndexViewModel
    {
        public User TopUser { get; set; } // Người có nhiều tiền nhất
        public string YouTubeLink { get; set; } // Link video YouTube
        public IEnumerable<Product> Products { get; set; } // Danh sách sản phẩm
    }

}
