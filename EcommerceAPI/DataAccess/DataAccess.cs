using EcommerceAPI.Models;
using System.Data.SqlClient;

namespace EcommerceAPI.DataAccess
{
    public class DataAccess : IDataAccess
    {
        private readonly IConfiguration configuration;
        private readonly string dbconnection;
        private readonly string dateformat;
        public DataAccess(IConfiguration configuration)
        {
            this.configuration = configuration;
            dbconnection = this.configuration["ConnectionStrings:DB"];
            dateformat = this.configuration["Constants:DateFormat"];
        }
        public Offer GetOffer(int id)
        {   
            var offer = new Offer();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "Select * FROM Offers WHERE OfferId=" + id + ";";
                command.CommandText = query;

                connection.Open();
                SqlDataReader r = command.ExecuteReader();
                while(r.Read())
                {
                    offer.Id = (int)r["OfferId"];
                    offer.Title = (string)r["Title"];
                    offer.Discount = (int)r["Discount"];
                }
            }
            return offer;
        }
     
        public List<ProductCategory> GetProductCategories()
        {
            var productCategories = new List<ProductCategory>();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "SELECT * FROM ProductCategories;";
                command.CommandText = query;

                connection.Open();
                SqlDataReader reader= command.ExecuteReader();
                while (reader.Read())
                {
                    var category = new ProductCategory()
                    {
                        Id = (int)reader["CategoryId"],
                        Category = (string)reader["Category"] ,
                        SubCategory = (string)reader["SubCategory"] ,
                    };
                    productCategories.Add(category);
                }
            }
                return productCategories;
        }
        
        public ProductCategory GetProductCategory(int id)
        {
            var productCategory = new ProductCategory();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "Select * FROM ProductCategories WHERE CategoryId=" + id + ";";
                command.CommandText = query;

                connection.Open();
                SqlDataReader r = command.ExecuteReader();
                while (r.Read())
                {
                    productCategory.Id = (int)r["CategoryId"];
                    productCategory.Category= (string)r["Category"];
                    productCategory.SubCategory = (string)r["SubCategory"];
                }
            }
            return productCategory;
        }
        public List<Product> GetProducts(string category, string subcategory , int count)
        {
            var products = new List<Product>();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "Select TOP " + count + " * FROM Products WHERE CategoryId=(SELECT CategoryId FROM ProductCategories WHERE Category = @c AND SubCategory = @s) ORDER BY  newid();";
                command.CommandText = query;
                command.Parameters.Add("@c", System.Data.SqlDbType.NVarChar).Value= category;
                command.Parameters.Add("@s", System.Data.SqlDbType.NVarChar).Value= subcategory;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var product = new Product()
                    {
                        Id = (int)reader["ProductId"],
                        Title = (string)reader["Title"],
                        Description = (string)reader["Description"],
                        Price = (double)reader["Price"],
                        Quantity = (int)reader["Quantity"],
                        ImageName = (string)reader["ImageName"]
                    };
                    var categoryid = (int)reader["CategoryId"];
                    product.ProductCategory = GetProductCategory(categoryid);

                    var offerid = (int)reader["OfferId"];
                    product.Offer = GetOffer(offerid);

                    products.Add(product);
                }
            }
            return products;
        }
    }
}
