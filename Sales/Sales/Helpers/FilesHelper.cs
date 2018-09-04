

namespace Sales.Helpers
{
    using System.IO;

    public class FilesHelper
    {

        //cogo la foto en un stream y luego lo convierto en byte para ser enviado
        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}