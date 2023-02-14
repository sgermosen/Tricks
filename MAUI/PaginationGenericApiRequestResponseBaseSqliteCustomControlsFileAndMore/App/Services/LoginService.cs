using Newtonsoft.Json;
using System.Text;
namespace sysmed.Services
{
    public class LoginService : ILoginService<LoginModel>
    {
        public LoginService()
        {
        }
        public async Task<LoginResponseModel> PerformLogin(LoginModel userModel)
        {
            LoginResponseModel objLogIndetail = null;
            try
            {
                if (!string.IsNullOrEmpty(userModel.Email) || !string.IsNullOrEmpty(userModel.Password))
                {
                    string baseURL = AppConstant.BaseUrl + "/api/account/login";
                    var json = JsonConvert.SerializeObject(userModel);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpClient _client = new HttpClient();
                    var task = await _client.PostAsync(baseURL, content);
                    if (task.IsSuccessStatusCode)
                    {
                        objLogIndetail = new LoginResponseModel();
                        var responsecontent = await task.Content.ReadAsStringAsync();
                        objLogIndetail = JsonConvert.DeserializeObject<LoginResponseModel>(responsecontent);

                    }
                    return await Task.FromResult(objLogIndetail);
                }
                return await Task.FromResult(objLogIndetail);

            }
            catch (Exception ex)
            {
                DependencyService.Get<Toast>().Show(ex.Message);
                return objLogIndetail;
            }
        }
    }
}
