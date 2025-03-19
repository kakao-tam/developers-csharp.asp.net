using Microsoft.AspNetCore.Mvc;
using AspNetMvcApp.Services;
using System.Text.Json;

namespace AspNetMvcApp.Controllers
{
    [Route("api/kakao")]
    public class KakaoController : Controller
    {
        private readonly IKakaoService _kakaoService;

        public KakaoController(IKakaoService kakaoService)
        {
            _kakaoService = kakaoService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("authorize")]
        public async Task<IActionResult> Login([FromQuery] string scope)
        {
            var authUrl = await _kakaoService.GetAuthorizationUrl(scope);
            return Redirect(authUrl);
        }

        [HttpGet("redirect")]
        public async Task<IActionResult> RedirectFromKakao([FromQuery] string code)
        {
            var (success, response) = await _kakaoService.Redirect(code);
            if (!success)
            {
                return BadRequest(response);
            }

            HttpContext.Session.SetString("AccessToken", response);
            return RedirectToAction("Index");
        }

        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Not logged in" });
            }

            var (success, response) = await _kakaoService.Profile(accessToken);
            if (!success)
            {
                return Unauthorized(response);
            }

            return Ok(JsonSerializer.Deserialize<object>(response));
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Not logged in" });
            }

            var (success, response) = await _kakaoService.Logout(accessToken);
            if (!success)
            {
                return BadRequest(response);
            }

            HttpContext.Session.Remove("AccessToken");
            return Ok(JsonSerializer.Deserialize<object>(response));
        }

        [HttpGet("unlink")]
        public async Task<IActionResult> Unlink()
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Not logged in" });
            }

            var (success, response) = await _kakaoService.Unlink(accessToken);
            if (!success)
            {
                return BadRequest(response);
            }

            HttpContext.Session.Remove("AccessToken");
            return Ok(JsonSerializer.Deserialize<object>(response));
        }

        [HttpGet("friends")]
        public async Task<IActionResult> Friends()
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Not logged in" });
            }

            var (success, response) = await _kakaoService.Friends(accessToken);
            if (!success)
            {
                return Unauthorized(response);
            }

            return Ok(JsonSerializer.Deserialize<object>(response));
        }

        [HttpGet("message")]
        public async Task<IActionResult> SendMessage()
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Not logged in" });
            }

            var (success, response) = await _kakaoService.SendMessage(accessToken);
            if (!success)
            {
                return BadRequest(response);
            }

            return Ok(JsonSerializer.Deserialize<object>(response));
        }

        [HttpGet("friend-message")]
        public async Task<IActionResult> SendFriendMessage([FromQuery] string uuid)
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Not logged in" });
            }

            var (success, response) = await _kakaoService.SendFriendMessage(accessToken, uuid);
            if (!success)
            {
                return BadRequest(response);
            }

            return Ok(JsonSerializer.Deserialize<object>(response));
        }
    }
} 