using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetflixBe.Models;
using System.Text.Encodings.Web;

namespace NetflixBe.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private UserManager<ApplicationUser> _userManager;
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private UrlEncoder _urlEncoder;

        public AuthenticationController(UserManager<ApplicationUser> userManager, UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _urlEncoder = urlEncoder;
        }
        private string GenerateQrCode(string email, string unformattedKey)
        {
            return string.Format(
            AuthenticatorUriFormat,
                _urlEncoder.Encode("My Netflix"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        [HttpGet("tfa-setup")]
        public async Task<IActionResult> GetTfaSetup(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var isTfaEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (authenticatorKey == null)
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }
            var formattedKey = GenerateQrCode(email, authenticatorKey);
            return Ok(new TfaSetupDto { IsTfaEnabled = isTfaEnabled, AuthenticatorKey = authenticatorKey, FormattedKey = formattedKey });
        }

        [HttpPost("tfa-setup")]
        public async Task<IActionResult> PostTfaSetup([FromBody] TfaSetupDto tfaModel)
        {
            var user = await _userManager.FindByEmailAsync(tfaModel.Email);
            var isValidCode = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                tfaModel.Code
            );
            if (isValidCode)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                return Ok(new TfaSetupDto { IsTfaEnabled = true });
            } else
            {
                return BadRequest("Invalid Code");
            }
        }

    }
}
