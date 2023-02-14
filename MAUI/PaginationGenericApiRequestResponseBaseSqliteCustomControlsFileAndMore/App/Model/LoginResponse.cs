namespace sysmed.Model
{
    public class LoginResponse
    {
       // public string Token { get; set; }
        public string Access_Token { get; set; }
        public string Token_Type { get; set; }
        public string UserName { get; set; }
        public UserBasicInfo UserDetail { get; set; } = new UserBasicInfo();
    }
}
