using Microsoft.Extensions.Logging;
using VkNet;
using VkNet.Enums.Filters;

namespace vk_runner
{
	public interface IAuthorizer
	{
		VkApi Authorize(Settings settings = null, ILogger<VkApi> logger = null);
	}
}