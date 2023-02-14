using Newtonsoft.Json;

namespace sysmed.Model
{
    public class Response<T>
    {
        public bool IsSuccess { get; set; } 
        public string Code { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
        public int ItemsCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
        [JsonProperty("value")]
        public T Data { get; set; }

    }
}
