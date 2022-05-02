using System.Threading.Tasks;
using Ogma3.Infrastructure.Formatters;

namespace Ogma3.Services.RssService;

public interface IRssService
{
	Task<RssResult> GetStoriesAsync();
	Task<RssResult> GetBlogpostsAsync();
}