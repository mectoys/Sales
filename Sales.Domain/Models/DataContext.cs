

namespace Sales.Domain.Models
{
    using System.Data.Entity;
    using Common.Models;

    public class DataContext :DbContext
    {
        public DataContext():base("DefaultConnection")
        {

        }
        //hay un modelo product usted me lo va a mapear en la bd y se llama products
        //mapea la clase product en la bD  tabla products
        public DbSet<Product> Products { get; set; }
    }
}
