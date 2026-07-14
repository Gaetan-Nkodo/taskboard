using Microsoft.Extensions.Configuration;

namespace TaskBoard.Application.Common;

public static class FrontendUrlHelper
{
    private const string DefaultFrontendUrl = "https://taskboard-iyec.onrender.com";

    public static string GetNormalizedFrontendUrl(IConfiguration config)
    {
        var raw = config["Frontend:BaseUrl"] ?? DefaultFrontendUrl;
        return raw.TrimEnd('/');
    }
}
