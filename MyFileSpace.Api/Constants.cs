namespace MyFileSpace.Api
{
    public static class Constants
    {
        public static readonly string TITLE = "Swagger My File Space API";
        public static readonly string APP_ABBR = "AuthSp";
        public static readonly string DESCRIPTION = "An ASP.NET Core Web API for storing and distributing files";

        public static readonly string VERSION = "v1";
        public static readonly string DEVELOPMENT = "Development";

        #region "Headers"
        public const string APP_HEADER = "X-App";
        public const string ENV_HEADER = "X-Env";
        public const string AUTH_R_HEADER = "X-Auth-R";
        public const string AUTH_N_HEADER = "X-Auth-N";
        public const string AUTH_HEADER = "Authorization";

        #endregion

        #region "Config paths"
        #endregion

        #region "Paths"
        public static readonly string ERROR_PATH = "/Home/Error";
        public static readonly string SERVICES_LIST_PATH = "/listservices";
        #endregion

        #region "Swagger"
        public static readonly string SWAGGER_AUTHORIZATION = "Authorization";
        public static readonly string SWAGGER_ENDPOINT = "/swagger/v1/swagger.json";
        public static readonly string SWAGGER_TITLE = "My File Space Swagger V1";
        public static readonly string SWAGGER_SECURITY_NAME = "bearerAuth";
        public static readonly string SWAGGER_SECURITY_DESCRIPTION = "JWT Authorization header using the Bearer scheme.";
        #endregion
    }
}
