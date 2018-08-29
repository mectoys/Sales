namespace Sales.Backend.Models
{
    using Common.Models;
    using System.Web;
    public class ProductView : Product
    {

        //para cargar el archivo sirve
        public HttpPostedFileBase ImageFile { get; set; }

    }
}