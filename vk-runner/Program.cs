using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using VkNet.Model.RequestParams;

namespace vk_runner
{
	internal static class Program
	{
	    private const string LogPath = @"..\..\log.txt";
	    private static readonly IAuthorizer Authorizer = new AccessTokenAuthorizer(new ConsoleCredentialsProvider(), new AccessTokenFileStorage());

		static void Main(string[] args)
		{
			var api = Authorizer.Authorize(logger: InitLogger());

			var response = api.NewsFeed.Get(new NewsFeedGetParams()
			{
				Count = 10
			});

			Console.ReadKey();
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
	}
}
