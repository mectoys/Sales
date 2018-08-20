

namespace Sales.Common
{
   public class Response
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        //generico para cualquier cosa
        public object Result { get; set; }

    }
}
