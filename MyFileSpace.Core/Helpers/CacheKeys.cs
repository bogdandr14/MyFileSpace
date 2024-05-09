namespace MyFileSpace.Core.Helpers
{
    internal static class CacheKeys
    {
        public static string FileCacheKeyPrefix(this Guid fileId)
        {
            return $"File_{fileId}_";
        }

        public static string FileCacheKey(this Guid fileId, Session session, string? accessKey)
        {
            return $"File_{fileId}_{session.UserId}_{accessKey}";
        }

        public static string DirectoryCacheKeyPrefix(this Guid directoryId)
        {
            return $"Directory_{directoryId}_";
        }

        public static string DirectoryCacheKey(this Guid directoryId, Session session, string accessKey)
        {
            return $"Directory_{directoryId}_{session.UserId}_{accessKey}";
        }
    }
}
