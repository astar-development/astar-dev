using Microsoft.Identity.Client;

namespace AStar.Dev.OneDrive.Client.Services;

public static class TokenCacheHelper
{
    public static void EnableSerialization(ITokenCache tokenCache, string cacheFilePath)
    {
        tokenCache.SetBeforeAccess(args =>
        {
            if (File.Exists(cacheFilePath)) args.TokenCache.DeserializeMsalV3(File.ReadAllBytes(cacheFilePath));
        });

        tokenCache.SetAfterAccess(args =>
        {
            if (args.HasStateChanged) File.WriteAllBytes(cacheFilePath, args.TokenCache.SerializeMsalV3());
        });
    }
}
