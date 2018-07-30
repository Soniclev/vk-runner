using System;
using System.IO;
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
					return api;
				}
				catch (AccessTokenInvalidException e)
				{
					Console.WriteLine(e);
				}
			}
			Relogin();
			return Authorize();
		}

		private string TwoFactorImplementation()
		{
			Console.WriteLine("Enter code from SMS or VK administrators message for completing the two factor authorize:");
			Console.Write("> ");
			return Console.ReadLine();
		}

		private void Relogin()
		{
			Console.WriteLine("Enter your application id:");
			Console.Write("> ");
			var appId = ulong.Parse(Console.ReadLine());

			Console.WriteLine("Enter your phone number or email:");
			Console.Write("> ");
			var login = Console.ReadLine();

			Console.WriteLine("Enter your password:");
			Console.Write("> ");
			var password = Console.ReadLine();

			using (VkApi api = new VkApi())
			{
				ApiAuthParams authParams = new ApiAuthParams()
				{
					ApplicationId = appId,
					Login = login,
					Password = password,
					Settings = Settings.All,
					TwoFactorAuthorization = TwoFactorImplementation
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