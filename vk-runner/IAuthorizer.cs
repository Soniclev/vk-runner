using VkNet;

namespace vk_runner
{
	public interface IAuthorizer
	{
		VkApi Authorize();
	}
}