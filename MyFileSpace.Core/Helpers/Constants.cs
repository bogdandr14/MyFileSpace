namespace MyFileSpace.Core.Helpers
{
    internal static class Constants
    {
        internal const string ROOT_DIRECTORY = "$USER_ROOT";
        public const string USER_ROLE_CLAIM = "user_role";

        //1GB of storage
        internal const long MAX_ALLOWED_USER_STORAGE = 1073741824;

        internal const string WELCOME_EMAIL_TITLE_EN = "Welcome to MyFileSpace";
        internal const string WELCOME_EMAIL_TEMPLATE_EN = "welcome_template.html";

        internal const string WELCOME_EMAIL_TITLE_RO = "Bun venit la MyFileSpace";
        internal const string WELCOME_EMAIL_TEMPLATE_RO = "welcome_template_ro.html";

        internal const string CONFIRMATION_EMAIL_TITLE_EN = "Confirm your MyFileSpace account";
        internal const string CONFIRMATION_EMAIL_TEMPLATE_EN = "confirmation_template.html";

        internal const string CONFIRMATION_EMAIL_TITLE_RO = "Confirmă-ţi contul MyFileSpace";
        internal const string CONFIRMATION_EMAIL_TEMPLATE_RO = "confirmation_template_ro.html";

        internal const string RESET_PASSWORD_EMAIL_TITLE_EN = "Reset your MyFileSpace account password";
        internal const string RESET_PASSWORD_EMAIL_TEMPLATE_EN = "reset_password_template.html";

        internal const string RESET_PASSWORD_EMAIL_TITLE_RO = "Resetează parola contului MyFileSpace";
        internal const string RESET_PASSWORD_EMAIL_TEMPLATE_RO = "reset_password_template_ro.html";
    }
}
