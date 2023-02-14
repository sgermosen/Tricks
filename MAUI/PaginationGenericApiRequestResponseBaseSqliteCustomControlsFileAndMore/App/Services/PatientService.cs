using Newtonsoft.Json;
using SQLite;

namespace sysmed.Services
{
    public interface IPatientService
    {
        Task<Response<T>> Get<T>(Request request);
        Task<Response<Patient>> GetPatient<Patient>(string searchTerm, int personId);
        Task<Response<T>> Add<T>(Patient model);
        Task<int> Delete(Patient model);
        Task<int> Update(Patient model);
    }
    public class PatientLocalService : IPatientService
    {
        private SQLiteAsyncConnection _dbConnection;
        public PatientLocalService()
        {
            SetUpDb();
        }

        private async void SetUpDb()
        {
            if (_dbConnection == null)
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Patient.db3");
                _dbConnection = new SQLiteAsyncConnection(dbPath);
                await _dbConnection.CreateTableAsync<Patient>();
            }
        }

        public Task<int> Add(Patient model)
        {
            return _dbConnection.InsertAsync(model);
        }

        public Task<int> Delete(Patient model)
        {
            return _dbConnection.DeleteAsync(model);
        }

        public async Task<Response<List<Patient>>> Get<T>(Request request)
        {
            var list = await _dbConnection.Table<Patient>().ToListAsync();
            return ApiService.ReturnResponseOfElement(list, "Ok");
        }

        public Task<int> Update(Patient model)
        {
            return _dbConnection.UpdateAsync(model);
        }


        Task<Response<T>> IPatientService.Add<T>(Patient model)
        {
            throw new NotImplementedException();
        }


        public Task<Response<Patient>> GetPatient<Patient>(string searchTerm, int personId)
        {
            throw new NotImplementedException();
        }


        Task<Response<T>> IPatientService.Get<T>(Request request)
        {
            throw new NotImplementedException();
        }
    }

    public class PatientApiService : IPatientService
    {
        private readonly ApiService _apiService;

        public PatientApiService()
        {
            _apiService = new ApiService();
        }
        public async Task<Response<T>> Get<T>(Request request)
        {
            UserBasicInfo user = App.UserDetails;
            return await Get<T>(user, request);
        }

        private async Task<Response<T>> Get<T>(UserBasicInfo user, Request request)
        {
            var response = await _apiService.Get<T>(AppConstant.BaseUrl, "/api", "/patients/getPatients", user.TokenType, user.AccessToken, request);
            if (!response.IsSuccess)
                return new Response<T> { IsSuccess = false, Message = response.Message };
            return response;
        }

        public async Task<Response<T>> Add<T>(Patient model)
        {
            try
            {
                UserBasicInfo user = App.UserDetails;
                string baseURL = AppConstant.BaseUrl + "/api/Patients/postPatient";
                var fileBytes = model.ImageArray;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.AccessToken);
                MultipartFormDataContent content = new MultipartFormDataContent();
                if (fileBytes != null)
                {
                    ByteArrayContent byteContent = new ByteArrayContent(fileBytes);

                    content.Add(byteContent, "File", model.Name);
                }

                StringContent personId = new StringContent(model.PersonId.ToString());
                content.Add(personId, "PersonId");

                StringContent PatientId = new StringContent(model.PatientId.ToString());
                content.Add(PatientId, "PatientId");
                StringContent InsuranceId = new StringContent(model.InsuranceId.ToString());
                content.Add(InsuranceId, "InsuranceId");

                StringContent InsuranceNumber = new StringContent(model.InsuranceNumber.ToString());
                content.Add(InsuranceNumber, "InsuranceNumber");
                StringContent BloodTypeId = new StringContent(model.BloodTypeId.ToString());
                content.Add(BloodTypeId, "BloodTypeId");
                StringContent Rnc = new StringContent(model.Rnc.ToString());
                content.Add(Rnc, "Rnc");
                StringContent Name = new StringContent(model.Name.ToString());
                content.Add(Name, "Name");
                StringContent LastName = new StringContent(model.LastName.ToString());
                content.Add(LastName, "LastName");
                StringContent GenderId = new StringContent(model.GenderId.ToString());
                content.Add(GenderId, "GenderId");
                StringContent CountryId = new StringContent(model.CountryId.ToString());
                content.Add(CountryId, "CountryId");
                StringContent Email = new StringContent(model.Email.ToString());
                content.Add(Email, "Email");
                StringContent Tel = new StringContent(model.Tel.ToString());
                content.Add(Tel, "Tel");
                StringContent MaritalSituationId = new StringContent(model.MaritalSituationId.ToString());
                content.Add(MaritalSituationId, "MaritalSituationId");
                StringContent OcupationId = new StringContent(model.OcupationId.ToString());
                content.Add(OcupationId, "OcupationId");
                StringContent ReligionId = new StringContent(model.ReligionId.ToString());
                content.Add(ReligionId, "ReligionId");
                StringContent Address = new StringContent(model.Address.ToString());
                content.Add(Address, "Address");
                StringContent Record = new StringContent(model.Record.ToString());
                content.Add(Record, "Record");

                var response = await _client.PostAsync(baseURL, content);

                if (!response.IsSuccessStatusCode)
                    return ApiService.ReturnResponseIsNotSuccess<T>(response);

                var result = await response.Content.ReadAsStringAsync();
                var newRecord = JsonConvert.DeserializeObject<T>(result);

                return ApiService.ReturnResponseOfElement<T>(newRecord, "Registro Añadido!!!");

            }
            catch (Exception ex)
            {
                return ApiService.ReturnResponseException<T>(ex);
            }
        }

        public Task<int> Delete(Patient model)
        {
            throw new NotImplementedException();
        }

        public Task<int> Update(Patient model)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<Patient>> GetPatient<Patient>(UserBasicInfo user, string searchTerm, int personId)
        {
            var response = await _apiService.Get<Patient>(AppConstant.BaseUrl, "/api", "/Patients/getPatient", user.TokenType, user.AccessToken, searchTerm, personId);
            return response;
        }

        public Task<Response<Patient>> GetPatient<Patient>(string searchTerm, int personId)
        {
            throw new NotImplementedException();
        }
    }
}
