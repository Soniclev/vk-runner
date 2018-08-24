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
	    private static readonly IAuthorizer Authorizer = new AccessTokenAuthorizer(new ConsoleCredentialsProvider(), new AccessTokenFileStorage());

		static void Main(string[] args)
		{
			var api = Authorizer.Authorize();

			var response = api.NewsFeed.Get(new NewsFeedGetParams()
			{
				Count = 10
			});

			Console.ReadKey();
		}
	}
}
