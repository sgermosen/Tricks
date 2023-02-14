namespace sysmed.Services
{
    public interface ILoginService<T>
    {
        Task<LoginResponseModel> PerformLogin(T userModel);
    }
}
