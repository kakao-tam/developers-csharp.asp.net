using System.Threading.Tasks;

namespace AspNetMvcApp.Services
{
    public interface IKakaoService
    {
        Task<string> GetAuthorizationUrl(string scope);
        Task<string> GetAccessToken(string code);
        Task<(bool Success, string Response)> Redirect(string code);
        Task<(bool Success, string Response)> Profile(string accessToken);
        Task<(bool Success, string Response)> Logout(string accessToken);
        Task<(bool Success, string Response)> Unlink(string accessToken);
        Task<(bool Success, string Response)> Friends(string accessToken);
        Task<(bool Success, string Response)> SendMessage(string accessToken);
        Task<(bool Success, string Response)> SendFriendMessage(string accessToken, string uuid);
    }
} 