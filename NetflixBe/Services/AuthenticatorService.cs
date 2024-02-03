using System.Text.Encodings.Web;

namespace NetflixBe.Services
{
    public class AuthenticatorService
    {
        private UrlEncoder _urlEncoder;
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public string GenerateQrCode(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Code Maze Two-Factor Auth"),
                _urlEncoder.Encode(email),
                unformattedKey
            );
        }
    }
}
