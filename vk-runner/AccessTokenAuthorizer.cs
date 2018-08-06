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

		private string GetTwoFactorCode()
		{
			Console.WriteLine("Enter code from SMS or VK administrators message for completing the two factor authorize:");
			Console.Write("> ");
			return Console.ReadLine();
		}

		private void Relogin()
        {
            var appId = GetAppId();
            var login = GetLogin();
            var password = GetPassword();

            using (VkApi api = new VkApi())
            {
                ApiAuthParams authParams = new ApiAuthParams()
                {
                    ApplicationId = appId,
                    Login = login,
                    Password = password,
                    Settings = Settings.All,
                    TwoFactorAuthorization = GetTwoFactorCode
                };
                api.Authorize(authParams);

                WriteAccessToken(api.Token);
            }
        }

        private static string GetPassword()
        {
            string password = null;

            while (true)
            {
                Console.WriteLine("Enter your password:");
                Console.Write("> ");
                password = Console.ReadLine();

                if (!string.IsNullOrEmpty(password))
                {
                    break;
                }
                Console.WriteLine("The password is invalid, try again");
            }

            return password;
        }

        private static string GetLogin()
        {
            string login = null;

            while (true)
            {
                Console.WriteLine("Enter your phone number or email:");
                Console.Write("> ");
                login = Console.ReadLine();

                if (!string.IsNullOrEmpty(login))
                {
                    break;
                }
                Console.WriteLine("The phone number or email is invalid, try again");
            }

            return login;
        }

        private static ulong GetAppId()
        {
            string appId = null;

            while (true)
            {
                Console.WriteLine("Enter your application id:");
                Console.Write("> ");
                appId = Console.ReadLine();

                if (appId?.ToCharArray().All(c => char.IsDigit(c)) == true)
                {
                    break;
                }
                Console.WriteLine("The application id is invalid, try again");
            }

            return ulong.Parse(appId);
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