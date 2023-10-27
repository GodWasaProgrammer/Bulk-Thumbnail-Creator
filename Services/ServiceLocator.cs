using Services.Interfaces;

namespace Services
{

    public static class ServiceLocator
    {
        private static ILogService _logService;

        public static void RegisterLogService(ILogService logService)
        {
            _logService = logService;
        }

        public static ILogService GetLogService()
        {
            return _logService;
        }
    }

}
