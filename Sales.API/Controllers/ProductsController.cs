

namespace Sales.API.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Common.Models;
    using Domain.Models;
    using Sales.API.Helpers;

    [Authorize]
    public class ProductsController : ApiController
    {
        private DataContext db = new DataContext();

        // GET: api/Products
        //revisamos la conexion a la BD
        public IQueryable<Product> GetProducts()
        {
            //si deseo que me entrege ordenados por descripcion 
            return this.db.Products.OrderBy(p=>p.Description);
        }

        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProduct(int id)
        {
            Product product = await this.db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            //coge un producto y lo vuelve JSON
            return Ok(product);
        }

        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProduct(int id, Product product)
        {
           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ProductId)
            {
                return BadRequest();
            }

            //Actualiza el producto
            //cambiar la imagen en la BD
            //preparamos para guardar la imagen
            if (product.ImageArray != null && product.ImageArray.Length > 0)
            {
                //cogemos la image arrray y la convertimos en un stream
                var stream = new MemoryStream(product.ImageArray);
                //Guid codigo alfanumerico que no se repite
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "~/Content/Products";
                var fullPath = $"{folder}/{file}";
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    product.ImagePath = fullPath;
                }
            }

            this.db.Entry(product).State = EntityState.Modified;

            try
            {
                await this.db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //DevuelVa el producto como quedo en la BD  
            return Ok(product);
        }

        // POST: api/Products
        [ResponseType(typeof(Product))]
        //recive el producto
        public async Task<IHttpActionResult> PostProduct(Product product)
        {
            product.IsAvailable = true;
            //grabamos la hora en formato UTCde Londres del meridiano Zero en caso 
            //que cambie de servidor nuestra BD por la 
            //ubicacion
            product.PublishOn = DateTime.Now.ToUniversalTime();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //preparamos para guardar la imagen
            if (product.ImageArray != null && product.ImageArray.Length > 0)
            {
                //cogemos la image arrray y la convertimos en un stream
                var stream = new MemoryStream(product.ImageArray);
                //Guid codigo alfanumerico que no se repite
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "~/Content/Products";
                var fullPath = $"{folder}/{file}";
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    product.ImagePath = fullPath;
                }
            }

            this.db.Products.Add(product);
            await this.db.SaveChangesAsync();
            //devuelve el producto como en la BD
            return CreatedAtRoute("DefaultApi", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            Product product = await this.db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            this.db.Products.Remove(product);
            await this.db.SaveChangesAsync();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return this.db.Products.Count(e => e.ProductId == id) > 0;
        }
    }
}