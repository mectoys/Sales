

namespace Sales.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Plugin.Connectivity;
    using Sales.Common;
    using Sales.Helpers;

    public class ApiService
    {
        public async Task<Response> CheckConnection()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = Languages.TurnOnInternet,
                };
            }
            //metodo que hace ping 
            var isReachable = await CrossConnectivity.Current.IsRemoteReachable("google.com");
            if (!isReachable)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = Languages.NoInternet,
                };
            }

            return new Response
            {
                IsSuccess = true,
            };
        }
            //crear un metodo generico para traer cualquier lista
            //T es clase generica.
            public async Task<Response> GetList<T>(string urlBase, string prefix, string controller)
        {
            try
            {
                //video 06
                //sirve para hacer la conexion
                var cliente = new HttpClient();
                //cargasrla direccion
                cliente.BaseAddress = new Uri(urlBase);
                //concatenera el prefijo y el controlador
                //string.Format("{0}{1}", prefix, controller);
                var url = $"{prefix}{controller}";
                var response = await cliente.GetAsync(url);
                //leer la respuesta
                var answer = await response.Content.ReadAsStringAsync();
                //todo el json es answer
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = answer,
                    };
                }
                //convertir el jason en string para mostrar la lista de objetos.
                var list = JsonConvert.DeserializeObject<List<T>>(answer);
                return new Response
                {
                    IsSuccess = true,
                    Result=list,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
    }
}
