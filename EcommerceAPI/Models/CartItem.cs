using EcommerceAPI.Models;
namespace ECommerceAPI.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public Product Product { get; set; } = new Product();
    }
}