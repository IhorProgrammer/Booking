using BookingLibrary.Helpers.HttpClientHelperNamespace;
using Microsoft.AspNetCore.WebUtilities;
using Token.API.Models;

namespace Token.API.Services
{
    public class GoogleOAuthService
    {
        private const string ClientId = "711827789320-nb53rqb07tpb7g8teohd7a3hjmvro5iv.apps.googleusercontent.com";
        private const string ClientSecret = "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";
        private const string OAuthServerEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TokenServerEndpoint = "https://oauth2.googleapis.com/token";

        public static string GenerateOAuthRequestUrl(string scope, string redirectUrl, string codeChellange, string state)
        {
            Dictionary<string, string?> queryParams = new Dictionary<string, string?>
            {
                { "client_id", ClientId },
                { "redirect_uri", redirectUrl },
                { "response_type", "code" },
                { "scope", scope },
                { "code_challenge", codeChellange },
                { "code_challenge_method", "S256" },
                { "access_type", "offline" },
                { "state", state }
            };

            var url = QueryHelpers.AddQueryString(OAuthServerEndpoint, queryParams);
            return url;
        }

        public static async Task<GoogleOAuthTokenResult?> ExchangeCodeOnTokenAsync(string code, string codeVerifier, string redirectUrl)
        {
            var authParams = new Dictionary<string, string>
            {
                { "client_id", ClientId },
                { "client_secret", ClientSecret },
                { "code", code },
                { "code_verifier", codeVerifier },
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUrl }
            };



            var tokenResult = await HttpClientHelper.SendPostRequest<GoogleOAuthTokenResult>(TokenServerEndpoint, authParams);
            return tokenResult;
        }

        public static async Task<GoogleOAuthTokenResult?> RefreshTokenAsync(string refreshToken)
        {
            var refreshParams = new Dictionary<string, string>
            {
                { "client_id", ClientId },
                { "client_secret", ClientSecret },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            var tokenResult = await HttpClientHelper.SendPostRequest<GoogleOAuthTokenResult>(TokenServerEndpoint, refreshParams);
            return tokenResult;
        }
    }
}
