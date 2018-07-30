using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;

namespace vk_runner
{
	class Program
	{
		private static readonly IAuthorizer _authorizer = new AccessTokenAuthorizer();

		static void Main(string[] args)
		{
			VkApi api = _authorizer.Authorize();
			Console.WriteLine(api.Status.Get(api.UserId ?? 1).Text);
			Console.ReadKey();
		}
	}
}
