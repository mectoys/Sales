 

namespace Sales.Backend.Models
{
    using Domain.Models;

    public class LocalDataContext : DataContext
    {
    /// public System.Data.Entity.DbSet<Sales.Common.Models.Product> Products { get; set; }
        //facilitamos lamigracion de la BD por asistente en el controlador
    }
}