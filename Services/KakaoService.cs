using System.Text.Json;
using System.Net.Http.Headers;

namespace AspNetMvcApp.Services
{
    public class KakaoService : IKakaoService
    {
        private readonly HttpClient _httpClient;
        private const string KAKAO_API_BASE_URL = "https://kapi.kakao.com";
        private const string KAKAO_AUTH_BASE_URL = "https://kauth.kakao.com";
        private readonly string _clientId;
        private readonly string _redirectUri;
        private readonly string _clientSecret;

        public KakaoService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _clientId = configuration["Kakao:ClientId"] ?? throw new InvalidOperationException("Kakao:ClientId not configured");
            _redirectUri = configuration["Kakao:RedirectUri"] ?? throw new InvalidOperationException("Kakao:RedirectUri not configured");
            _clientSecret = configuration["Kakao:ClientSecret"] ?? throw new InvalidOperationException("Kakao:ClientSecret not configured");
        }

        private void SetHeaders(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task<string> SendRequest(HttpMethod method, string url, HttpContent content = null)
        {
            try
            {
                var request = new HttpRequestMessage(method, url);
                if (content != null)
                {
                    request.Content = content;
                }

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Serialize(new { error = $"HTTP {(int)response.StatusCode}: {responseContent}" });
                }

                return responseContent;
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message });
            }
        }

        public Task<string> GetAuthorizationUrl(string scope)
        {
            var authUrl = $"{KAKAO_AUTH_BASE_URL}/oauth/authorize?client_id={_clientId}&redirect_uri={_redirectUri}&response_type=code";
            if (!string.IsNullOrEmpty(scope))
            {
                authUrl += $"&scope={scope}";
            }
            return Task.FromResult(authUrl);
        }

        public async Task<string> GetAccessToken(string code)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("redirect_uri", _redirectUri),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_secret", _clientSecret)
            });
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await SendRequest(HttpMethod.Post, $"{KAKAO_AUTH_BASE_URL}/oauth/token", content);
            var responseData = JsonSerializer.Deserialize<JsonElement>(response);
            
            if (responseData.TryGetProperty("error", out var error))
            {
                return response;
            }

            if (!responseData.TryGetProperty("access_token", out var accessToken))
            {
                return JsonSerializer.Serialize(new { error = "Access token is missing from response" });
            }
                
            return JsonSerializer.Serialize(new { access_token = accessToken.GetString() });
        }

        public async Task<(bool Success, string Response)> Redirect(string code)
        {
            if (string.IsNullOrEmpty(code))
                return (false, JsonSerializer.Serialize(new { error = "Authorization code not found" }));

            var response = await GetAccessToken(code);
            var responseData = JsonSerializer.Deserialize<JsonElement>(response);
            
            if (responseData.TryGetProperty("error", out var error))
            {
                return (false, response);
            }

            var accessToken = responseData.GetProperty("access_token").GetString();
            if (string.IsNullOrEmpty(accessToken))
            {
                return (false, response);
            }

            return (true, accessToken);
        }

        public async Task<(bool Success, string Response)> Profile(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return (false, JsonSerializer.Serialize(new { error = "Not logged in" }));

            SetHeaders(accessToken);
            var response = await SendRequest(HttpMethod.Get, $"{KAKAO_API_BASE_URL}/v2/user/me");
            return (true, response);
        }

        public async Task<(bool Success, string Response)> Logout(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return (false, JsonSerializer.Serialize(new { error = "Not logged in" }));

            SetHeaders(accessToken);
            var response = await SendRequest(HttpMethod.Post, $"{KAKAO_API_BASE_URL}/v1/user/logout");
            return (true, response);
        }

        public async Task<(bool Success, string Response)> Unlink(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return (false, JsonSerializer.Serialize(new { error = "Not logged in" }));

            SetHeaders(accessToken);
            var response = await SendRequest(HttpMethod.Post, $"{KAKAO_API_BASE_URL}/v1/user/unlink");
            return (true, response);
        }

        public async Task<(bool Success, string Response)> Friends(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return (false, JsonSerializer.Serialize(new { error = "Not logged in" }));

            SetHeaders(accessToken);
            var response = await SendRequest(HttpMethod.Get, $"{KAKAO_API_BASE_URL}/v1/api/talk/friends");
            return (true, response);
        }

        public async Task<(bool Success, string Response)> SendMessage(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return (false, JsonSerializer.Serialize(new { error = "Not logged in" }));

            SetHeaders(accessToken);
            
            var templateObject = new
            {
                object_type = "text",
                text = "Hello from ASP.NET MVC!",
                link = new
                {
                    web_url = "https://developers.kakao.com",
                    mobile_web_url = "https://developers.kakao.com"
                }
            };

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("template_object", JsonSerializer.Serialize(templateObject))
            });
            
            var response = await SendRequest(HttpMethod.Post, $"{KAKAO_API_BASE_URL}/v2/api/talk/memo/default/send", content);
            return (true, response);
        }

        public async Task<(bool Success, string Response)> SendFriendMessage(string accessToken, string uuid)
        {
            if (string.IsNullOrEmpty(accessToken))
                return (false, JsonSerializer.Serialize(new { error = "Not logged in" }));

            SetHeaders(accessToken);
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("receiver_uuids", $"[{uuid}]"),
                new KeyValuePair<string, string>("template_object", JsonSerializer.Serialize(new
                {
                    object_type = "text",
                    text = "Hello, world!",
                    link = new
                    {
                        web_url = "https://developers.kakao.com",
                        mobile_web_url = "https://developers.kakao.com"
                    }
                }))
            });

            var response = await SendRequest(HttpMethod.Post, $"{KAKAO_API_BASE_URL}/v1/api/talk/friends/message/default/send", content);
            return (true, response);
        }
    }
} 