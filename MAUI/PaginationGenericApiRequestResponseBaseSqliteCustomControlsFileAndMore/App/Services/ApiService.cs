using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace sysmed.Services
{
    public class ApiService
    {
        public static Response<List<T>> ReturnResponseOfList<T>(List<T> list, string message)
        {
            return new Response<List<T>>
            {
                IsSuccess = true,
                Message = message,
                Data = list,
            };
        }
        public static Response<T> ReturnResponseOfElement<T>(T element, string message)
        {
            return new Response<T>
            {
                IsSuccess = true,
                Message = message,
                Data = element,
            };
        }
        public static Response<T> ReturnResponse<T>(string message)
        {
            return new Response<T>
            {
                IsSuccess = true,
                Message = message
            };
        }
        public static Response<T> ReturnResponseIsNotSuccess<T>(HttpResponseMessage response)
        {
            return new Response<T>
            {
                IsSuccess = false,
                Code = response.StatusCode.ToString(),
                Message = response.ReasonPhrase.ToString(),
            };
        }
        public static Response<T> ReturnResponseException<T>(Exception ex)
        {
            return new Response<T>
            {
                IsSuccess = false,
                Message = ex.Message,
            };
        }
        public async Task<TokenResponse> GetTokenMvc(string urlBase, string username, string password)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var response = await client.PostAsync("Token",
                    new StringContent(string.Format("grant_type=password&username={0}&password={1}",
                    username, password),
                        Encoding.UTF8, "application/x-www-form-urlencoded"));
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TokenResponse>(resultJson);
                return result;
            }
            catch
            {
                return null;
            }
        }
        public async Task<Response<List<T>>> Get<T>(string urlBase, string servicePrefix, string controller)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return ReturnResponseIsNotSuccess<List<T>>(response);

                var result = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<T>>(result);
                return ReturnResponseOfElement<List<T>>(list, "Ok");
            }
            catch (Exception ex)
            {
                return ReturnResponseException<List<T>>(ex);
            }
        }
        public async Task<Response<List<T>>> Get<T>(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return ReturnResponseIsNotSuccess<List<T>>(response);

                var result = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<T>>(result);
                return ReturnResponseOfElement<List<T>>(list, "Ok");
            }
            catch (Exception ex)
            {
                return ReturnResponseException<List<T>>(ex);
            }
        }
        public async Task<Response<T>> Get<T>(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken, Request request)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                client.BaseAddress = new Uri(urlBase);
                var url = $"{servicePrefix}{controller}?criteria={request.SearchTerm}&page={request.PageNumber}&pageSize={request.PageSize}&all={request.All}";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return ReturnResponseIsNotSuccess<T>(response);

                var result = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<Response<T>>(result);
                return responseData;
            }
            catch (Exception ex)
            {
                return ReturnResponseException<T>(ex);
            }
        }
        public async Task<Response<T>> Get<T>(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken, string searchTerm, int id)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}/{2}/{3}", servicePrefix, controller, id, searchTerm);
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return ReturnResponseIsNotSuccess<T>(response);

                var result = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<T>(result);
                return ReturnResponseOfElement(list, "Ok");
            }
            catch (Exception ex)
            {
                return ReturnResponseException<T>(ex);
            }
        }
        public async Task<Response<T>> PasswordRecovery<T>(string urlBase, string servicePrefix, string controller, string email)
        {
            try
            {
                var userRequest = new UserRequest { Email = email, };
                var request = JsonConvert.SerializeObject(userRequest);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return ReturnResponseIsNotSuccess<T>(response);

                return ReturnResponse<T>("Ok");
            }
            catch (Exception ex)
            {
                return ReturnResponseException<T>(ex);
            }
        }
        public async Task<Response<UserBasicInfo>> GetUserByEmail<UserBasicInfo>(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken, string email)
        {
            try
            {
                var userRequest = new UserRequest { Email = email, };
                var request = JsonConvert.SerializeObject(userRequest);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return ReturnResponseIsNotSuccess<UserBasicInfo>(response);

                var result = await response.Content.ReadAsStringAsync();
                var newRecord = JsonConvert.DeserializeObject<UserBasicInfo>(result);

                return ReturnResponseOfElement(newRecord, "Ok");
            }
            catch (Exception ex)
            {
                return ReturnResponseException<UserBasicInfo>(ex);
            }
        }
        public async Task<Response<string>> ChangePassword<T>(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken, ChangePasswordRequest changePasswordRequest)
        {
            try
            {
                var request = JsonConvert.SerializeObject(changePasswordRequest);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return ReturnResponseIsNotSuccess<string>(response);
                return ReturnResponseOfElement<string>("Contraseña cambiada satisfactoriamente!!!", "Registro Actualizado");
            }
            catch (Exception ex)
            {
                return ReturnResponseException<string>(ex);
            }
        }
        public async Task<Response<T>> Post<T>(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken, T model)
        {
            try
            {
                StringContent content;
                HttpClient client;
                CreateHeaderForRequest(urlBase, tokenType, accessToken, model, out content, out client);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return ReturnResponseIsNotSuccess<T>(response);

                var result = await response.Content.ReadAsStringAsync();
                var newRecord = JsonConvert.DeserializeObject<T>(result);

                return ReturnResponseOfElement(newRecord, "Registro Añadido!!!");
            }
            catch (Exception ex)
            {
                return ReturnResponseException<T>(ex);
            }
        }
        public async Task<Response<T>> Put<T>(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken, T model)
        {
            try
            {
                StringContent content;
                HttpClient client;
                CreateHeaderForRequest(urlBase, tokenType, accessToken, model, out content, out client);
                var url = string.Format("{0}{1}/{2}", servicePrefix, controller, model.GetHashCode());
                var response = await client.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return ReturnResponseIsNotSuccess<T>(response);

                var result = await response.Content.ReadAsStringAsync();
                var recordUpdated = JsonConvert.DeserializeObject<T>(result);
                return ReturnResponseOfElement(recordUpdated, "Registro Actualizado!!!");
            }
            catch (Exception ex)
            {
                return ReturnResponseException<T>(ex);
            }
        }
        public async Task<Response<T>> Delete<T>(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken, T model)
        {
            try
            {
                StringContent content;
                HttpClient client;
                CreateHeaderForRequest(urlBase, tokenType, accessToken, model, out content, out client);
                var url = string.Format("{0}{1}/{2}", servicePrefix, controller, model.GetHashCode());
                var response = await client.DeleteAsync(url);

                if (!response.IsSuccessStatusCode)
                    return ReturnResponseIsNotSuccess<T>(response);

                return ReturnResponse<T>("Registro Eliminado!!!");
            }
            catch (Exception ex)
            {
                return ReturnResponseException<T>(ex);
            }
        }
        private static void CreateHeaderForRequest<T>(string urlBase, string tokenType, string accessToken, T model, out StringContent content, out HttpClient client)
        {
            ////client.Timeout = TimeSpan.FromMinutes(3);
            var request = JsonConvert.SerializeObject(model);
            content = new StringContent(request, Encoding.UTF8, "application/json");
            client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
            client.BaseAddress = new Uri(urlBase);
        }
    }
}
