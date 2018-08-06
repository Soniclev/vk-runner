using NLog;
using VkNet;
using VkNet.Enums.Filters;

namespace vk_runner
{
	public interface IAuthorizer
	{
		VkApi Authorize(Settings settings = null, ILogger logger = null);
	}
}