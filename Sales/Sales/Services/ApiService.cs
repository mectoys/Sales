

namespace Sales.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Plugin.Connectivity;

    using Sales.Common;
    using Sales.Common.Models;
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

        //OBTENER EL TOKEN
        public async Task<TokenResponse> GetToken(string urlBase, string username,  string password)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var response = await client.PostAsync("Token",
                    new StringContent(string.Format(
                    "grant_type=password&username={0}&password={1}",
                    username, password),
                    Encoding.UTF8, "application/x-www-form-urlencoded"));
                var resultJSON = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TokenResponse>(
                    resultJSON);
                return result;
            }
            catch
            {
                return null;
            }
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

        public async Task<Response> GetList<T>(string urlBase, string prefix, string controller,string tokenType,
            string accessToken)
        {
            try
            {
                //video 06
                //sirve para hacer la conexion
                var cliente = new HttpClient();
                //cargasrla direccion
                cliente.BaseAddress = new Uri(urlBase);
                //Cargamos la autorizacion de token
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
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
                    Result = list,
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


        //metodo generico de hacer un post de cualquier cosa(grabar en la BD)
        public async Task<Response> Post<T>(string urlBase, string prefix, string controller, T model)
        {
            //post no es diferente al get, son muy parecidos

            //hay que hacer un cambio serializar el modelo T y convertirlo en un Content <T model>
            try
            {
                //video 23
                //coje el objeto y lo convierte en String
                var request = JsonConvert.SerializeObject(model);
                //hay que codificarlo para que reconosca las tildes y demas simbolos del idioma
                //UTF8 permite las tildes , viñetas , si es idioma arabe se debe utilizar otro.
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                //sirve para hacer la conexion
                var cliente = new HttpClient();
                //cargar a la direccion
                cliente.BaseAddress = new Uri(urlBase);
                //concatenera el prefijo y el controlador
                //string.Format("{0}{1}", prefix, controller);
                var url = $"{prefix}{controller}";
                //realizamos un envio de la info o POST
                var response = await cliente.PostAsync(url,content);
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
                //deserealizo  objeto T como objeto
                //envio los datos si el ID del producto porque la BD lo genera automaticamente
                //luego me lo devuelve con el ID generado 
                //String a objeto
                var obj = JsonConvert.DeserializeObject<T>(answer);
                return new Response
                {
                    IsSuccess = true,
                    Result = obj,
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


        public async Task<Response> Post<T>(string urlBase, string prefix, string controller, T model, string tokenType,
            string accessToken)
        {
            //post no es diferente al get, son muy parecidos

            //hay que hacer un cambio serializar el modelo T y convertirlo en un Content <T model>
            try
            {
                //video 23
                //coje el objeto y lo convierte en String
                var request = JsonConvert.SerializeObject(model);
                //hay que codificarlo para que reconosca las tildes y demas simbolos del idioma
                //UTF8 permite las tildes , viñetas , si es idioma arabe se debe utilizar otro.
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                //sirve para hacer la conexion
                var cliente = new HttpClient();
                //cargar a la direccion
                cliente.BaseAddress = new Uri(urlBase);
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                //concatenera el prefijo y el controlador
                //string.Format("{0}{1}", prefix, controller);
                var url = $"{prefix}{controller}";
                //realizamos un envio de la info o POST
                var response = await cliente.PostAsync(url, content);
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
                //deserealizo  objeto T como objeto
                //envio los datos si el ID del producto porque la BD lo genera automaticamente
                //luego me lo devuelve con el ID generado 
                //String a objeto
                var obj = JsonConvert.DeserializeObject<T>(answer);
                return new Response
                {
                    IsSuccess = true,
                    Result = obj,
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

        //para el editProducto video 31
        public async Task<Response> Put<T>(string urlBase, string prefix, string controller, T model,int id)
        {
            //post no es diferente al get, son muy parecidos

            //hay que hacer un cambio serializar el modelo T y convertirlo en un Content <T model>
            try
            {
                //video 23
                //coje el objeto y lo convierte en String
                var request = JsonConvert.SerializeObject(model);
                //hay que codificarlo para que reconosca las tildes y demas simbolos del idioma
                //UTF8 permite las tildes , viñetas , si es idioma arabe se debe utilizar otro.
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                //sirve para hacer la conexion
                var cliente = new HttpClient();
                //cargar a la direccion
                cliente.BaseAddress = new Uri(urlBase);
                //concatenera el prefijo y el controlador
                //string.Format("{0}{1}", prefix, controller);
                var url = $"{prefix}{controller}/{id}";
                //realizamos un envio de la info o PUT para editar
                var response = await cliente.PutAsync(url, content);
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
                //deserealizo  objeto T como objeto
                //envio los datos si el ID del producto porque la BD lo genera automaticamente
                //luego me lo devuelve con el ID generado 
                //String a objeto
                var obj = JsonConvert.DeserializeObject<T>(answer);
                return new Response
                {
                    IsSuccess = true,
                    Result = obj,
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

        public async Task<Response> Put<T>(string urlBase, string prefix, string controller, T model, int id,
            string tokenType, string accessToken)
        {
            //post no es diferente al get, son muy parecidos

            //hay que hacer un cambio serializar el modelo T y convertirlo en un Content <T model>
            try
            {
                //video 23
                //coje el objeto y lo convierte en String
                var request = JsonConvert.SerializeObject(model);
                //hay que codificarlo para que reconosca las tildes y demas simbolos del idioma
                //UTF8 permite las tildes , viñetas , si es idioma arabe se debe utilizar otro.
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                //sirve para hacer la conexion
                var cliente = new HttpClient();
                //cargar a la direccion
                cliente.BaseAddress = new Uri(urlBase);
                //Cargamos la autorizacion
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                //concatenera el prefijo y el controlador
                //string.Format("{0}{1}", prefix, controller);
                var url = $"{prefix}{controller}/{id}";
                //realizamos un envio de la info o PUT para editar
                var response = await cliente.PutAsync(url, content);
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
                //deserealizo  objeto T como objeto
                //envio los datos si el ID del producto porque la BD lo genera automaticamente
                //luego me lo devuelve con el ID generado 
                //String a objeto
                var obj = JsonConvert.DeserializeObject<T>(answer);
                return new Response
                {
                    IsSuccess = true,
                    Result = obj,
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

        //metodo no generico para la eliminación del registro
        public async Task<Response> Delete(string urlBase, string prefix, string controller, int id)
        {
            try
            {
                //video 28
                //sirve para hacer la conexion
                var cliente = new HttpClient();
                //cargasrla direccion
                cliente.BaseAddress = new Uri(urlBase);
                //concatenera el prefijo y el controlador
                //string.Format("{0}{1}", prefix, controller);
                //concantenar el id 
                var url = $"{prefix}{controller}/{id}";
                var response = await cliente.DeleteAsync(url);
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
                               
                return new Response
                {
                    IsSuccess = true,
                    
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

        public async Task<Response> Delete(string urlBase, string prefix, string controller, int id, 
            string tokenType, string accessToken)
        {
            try
            {
                //video 28
                //sirve para hacer la conexion
                var cliente = new HttpClient();
                //cargasrla direccion
                cliente.BaseAddress = new Uri(urlBase);
                //Cargamos la autorizacion de token
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                //concatenera el prefijo y el controlador
                //string.Format("{0}{1}", prefix, controller);
                //concantenar el id 
                var url = $"{prefix}{controller}/{id}";
                var response = await cliente.DeleteAsync(url);
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

                return new Response
                {
                    IsSuccess = true,

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

        public async Task<Response> GetUser(string urlBase, string prefix, string controller, string email, string tokenType, string accessToken)
        {
            try
            {
                var getUserRequest = new GetUserRequest
                {
                    Email = email,
                };

                var request = JsonConvert.SerializeObject(getUserRequest);
                var content = new StringContent(request, Encoding.UTF8, "application/json");

                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                var url = $"{prefix}{controller}";
                var response = await client.PostAsync(url, content);
                var answer = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = answer,
                    };
                }

                var user = JsonConvert.DeserializeObject<MyUserASP>(answer);
                return new Response
                {
                    IsSuccess = true,
                    Result = user,
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

