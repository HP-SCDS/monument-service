namespace MonumentService.Refresher
{
    public class MonumentRefresher : IMonumentRefresher, IDisposable
    {
        private readonly ILogger m_logger;

        public MonumentRefresher(ILogger<MonumentRefresher> logger)
        {
            m_logger = logger;
            m_logger.LogInformation("Monument refresher initialized");
        }

        public void Start()
        {
            m_logger.LogInformation("Monument refresher started");
        }

        #region IDisposable

        private bool m_disposed;

        public void Dispose()
        {
            if (m_disposed)
            {
                return;
            }

            m_logger.LogInformation("Monument refresher finalized");

            GC.SuppressFinalize(this);

            m_disposed = true;
        }

        #endregion IDisposable
    }
}
