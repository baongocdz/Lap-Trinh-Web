using CUOIKY.Repository.Models;
using X.PagedList;

namespace CUOIKY.Models
{
    public class HomeIndexViewModel
    {
        public IPagedList<Product> GameAccounts { get; set; }
        public IPagedList<Product> GameItems { get; set; }
        public List<User> TopUsers { get; set; }
    }
}
