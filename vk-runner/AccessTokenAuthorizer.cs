using System;
using System.IO;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;

namespace vk_runner
{
    class AccessTokenAuthorizer : IAuthorizer
    {
        private const string AccessTokenPath = @"..\..\token.txt";
        private const string LogPath = @"..\..\log.txt";
        private readonly ICredentialsProvider _credentialsProvider;

        public AccessTokenAuthorizer(ICredentialsProvider credentialsProvider)
        {
            _credentialsProvider = credentialsProvider;
        }

        /// <summary>
        /// Инициализация логгера.
        /// </summary>
        /// <returns>Логгер</returns>
        private static ILogger InitLogger()
        {
            var fileTarget = new FileTarget
            {
                Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}",
                FileName = LogPath
            };

            var config = new LoggingConfiguration();
            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));
            LogManager.Configuration = config;
            return LogManager.GetLogger("VkApi");
        }

        public VkApi Authorize()
        {
            if (File.Exists(AccessTokenPath))
            {
                try
                {
                    VkApi api = new VkApi(InitLogger());
                    ApiAuthParams authParams = new ApiAuthParams()
                    {
                        AccessToken = ReadAccessToken(),
                        Settings = Settings.All
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
            return Authorize();
        }

        private void Relogin()
        {
            var appId = _credentialsProvider.GetAppId();
            var login = _credentialsProvider.GetLogin();
            var password = _credentialsProvider.GetPassword();

            using (VkApi api = new VkApi())
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

                WriteAccessToken(api.Token);
            }
        }

        private string ReadAccessToken()
        {
            return File.ReadAllText(AccessTokenPath);
        }

        private void WriteAccessToken(string accessToken)
        {
            File.WriteAllText(AccessTokenPath, accessToken);
        }
    }
}