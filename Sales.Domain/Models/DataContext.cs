

namespace Sales.Domain.Models
{
    using System.Data.Entity;
    using Common.Models;

    public class DataContext :DbContext
    {
        public DataContext():base("DefaultConnection")
        {

        }

        public System.Data.Entity.DbSet<Sales.Common.Models.Product> Products { get; set; }
    }
}
