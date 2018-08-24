using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;

namespace vk_runner
{
    internal class AccessTokenAuthorizer : IAuthorizer
    {
        private readonly ICredentialsProvider _credentialsProvider;
        private readonly IAccessTokenStorage _tokenStorage;

        public AccessTokenAuthorizer(ICredentialsProvider credentialsProvider, IAccessTokenStorage tokenStorage)
        {
            _credentialsProvider = credentialsProvider;
            _tokenStorage = tokenStorage;
        }

        public VkApi Authorize(Settings settings = null, ILogger<VkApi> logger = null)
        {
            if (settings == null)
                settings = Settings.All;

            if (_tokenStorage.IsAccessTokenExists())
            {
                try
                {
                    var api = new VkApi(logger);
                    var authParams = new ApiAuthParams
                    {
                        AccessToken = _tokenStorage.ReadAccessToken(),
                        Settings = settings
                    };
                    api.Authorize(authParams);
                    api.Database.GetCountriesById(1); // делаем какой-либо запрос, чтобы проверить access_token
                    return api;
                }
                catch (AccessTokenInvalidException e)
                {
                    Console.WriteLine(e);
                }
                catch (UserAuthorizationFailException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Relogin();
            return Authorize(settings, logger);
        }

        private void Relogin()
        {
            var appId = _credentialsProvider.GetAppId();
            var login = _credentialsProvider.GetLogin();
            var password = _credentialsProvider.GetPassword();

            using (var api = new VkApi())
            {
                ApiAuthParams authParams = new ApiAuthParams()
                {
                    ApplicationId = appId,
                    Login = login,
                    Password = password,
                    Settings = Settings.All,
                    TwoFactorAuthorization = _credentialsProvider.GetTwoFactorCode
                };
                api.Authorize(authParams);

                _tokenStorage.WriteAccessToken(api.Token);
            }
        }
    }
}