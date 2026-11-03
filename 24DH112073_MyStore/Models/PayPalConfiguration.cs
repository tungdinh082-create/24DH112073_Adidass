using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PayPal.Api;

namespace _24DH112073_MyStore.Models
{
    public class PayPalConfiguration
    {
        // Dán Client ID
        public readonly static string ClientId = "AQO7y0dvk4oFylkkL16FlthORQPHkOqUwlT6-V3CSwLJwUdY3_J4Wg1HUA7lSI72WITnR8Pch6GEcH73";

        // Dán Secret Key 2 (MỚI) vào đây
        public readonly static string ClientSecret = "EJQyGgZeDJRq_Buwl_9VptokYs2sORXDVsCq6jdM2_9w84hqLcQ9Qkk8gUODltFf2BWe8NM43h8H9bIa";

        // Hàm lấy Token xác thực
        public static APIContext GetAPIContext()
        {
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }

        // Hàm lấy AccessToken từ PayPal
        private static string GetAccessToken()
        {
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
            return accessToken;
        }

        // Cấu hình chế độ Sandbox
        private static Dictionary<string, string> GetConfig()
        {
            return new Dictionary<string, string>()
            {
                { "mode", "sandbox" },
                { "connectionTimeout", "360000" },
                { "requestRetries", "1" },
                { "clientId", ClientId },
                { "clientSecret", ClientSecret }
            };
        }
    }
}