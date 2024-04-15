namespace MyFileSpace.SharedKernel.Providers
{
    public static class AppServicesProvider
    {
        private static IServiceProvider _services;

        public static void Initialize(this IServiceProvider services)
        {
            if (_services != null)
            {
                throw new Exception("Can't set once a value has already been set.");
            }
            _services = services;
        }

        /// <summary>
        /// Provides static access to the framework's services provider
        /// </summary>
        public static IServiceProvider Services
        {
            get { return _services; }
        }

        public static T GetService<T>() where T : class
        {

            var s = _services.GetService(typeof(T)) as T;
            return s!;
        }
    }
}
