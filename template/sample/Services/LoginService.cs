using ChatWithDocsMobileApp.Shared.Services;
using System;
using System.Text.Json;
namespace ChatWithDocsMobileApp.Services
{
    public class LoginService
    {
        private readonly ApiManager _apiManager;

        public LoginService()
        {
            _apiManager = new ApiManager();
        }

        public async Task<(bool Success, string? Message)> loginSSOUser(GoogleUserInfo userInfo)
        {
            string? responseMsg = null;
            var response = await _apiManager.SendRequestAsync("auth/CreateSSOUser", ApiManager.RequestMethod.POST, userInfo);
            var deserializeResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);
            if (deserializeResponse != null && deserializeResponse.TryGetValue("message", out var message) && message != null)
            {
                responseMsg = message.ToString();
            }
            if (deserializeResponse != null && deserializeResponse.TryGetValue("access_token", out var accessToken) )
            {
                // Save the access token
                Preferences.Set("access_token", accessToken.ToString());
                Console.WriteLine($"Access Token: {Preferences.Get("access_token", null)}");

                // Check for additional response data
                if (deserializeResponse.TryGetValue("data", out var user) && user != null)
                {
                    Preferences.Set("user", user.ToString());
                    return (true, responseMsg);
                }
                
            }
            return (false,responseMsg);
        }

        public async Task<(bool Success, string? Message)> loginInAppUser(string email, string password)
        {
            string responseMsg = null;

            var requestBody = new {
                 email = email,
                 password = password
            };
            
            var response = await _apiManager.SendRequestAsync("auth/signIn", ApiManager.RequestMethod.POST, requestBody);
            var deserializeResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);
            if (deserializeResponse != null && deserializeResponse.TryGetValue("message", out var message) && message != null)
            {
                responseMsg = message.ToString() ?? "";
            }
            Console.Write(responseMsg);
            if (deserializeResponse != null && deserializeResponse.TryGetValue("access_token", out var accessToken))
            {
                // Save the access token
                Preferences.Set("access_token", accessToken.ToString());
                Console.WriteLine($"Access Token: {Preferences.Get("access_token", null)}");

                // Check for additional response data
                if (deserializeResponse.TryGetValue("data", out var user) && user != null)
                {
                    Preferences.Set("user", user.ToString());
                    return (true, responseMsg);
                }

            }
            return (false, responseMsg);
        }
      
        public dynamic getUser()
        {
            var user = Preferences.Get("user", "");
            var deserializedUser = JsonSerializer.Deserialize<dynamic>(user);
        
            return deserializedUser;
        }
    }
}

