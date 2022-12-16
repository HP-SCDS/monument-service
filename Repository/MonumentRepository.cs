namespace MonumentService.Repository
{
    using LiteDB;
    using MonumentService.Model;
    using MonumentService.Util;
    using System.Reflection;

    public class MonumentRepository : IMonumentRepository, IDisposable
    {
        private static readonly string MonumentsDatabase = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? ".", "monuments.db");
        private const string CollectionName = "monuments";

        // SpinLocks can't be readonly, check https://stackoverflow.com/a/11225279 for details
        private SpinLock m_slock = new();
        private IList<Monument> m_monumentsCache = new List<Monument>();

        private readonly ILogger m_logger;
        private readonly LiteDatabase m_database;

        public MonumentRepository(ILogger<MonumentRepository> logger)
        {
            m_logger = logger;
            m_database = new LiteDatabase(new ConnectionString { Filename = MonumentsDatabase });

            // warmup the cache
            UpdateCache();

            m_logger.LogInformation($"Monuments database initialized at {MonumentsDatabase}");
        }

        public IEnumerable<Monument> GetAll()
        {
            return Get(_ => true);
        }

        public IEnumerable<Monument> Get(Func<Monument, bool> filter)
        {
            return SyncHelper.OperationWithLock(ref m_slock, () => m_monumentsCache.Where(filter));
        }

        public int AddOrUpdate(params Monument[] itemsToUpdate)
        {
            ILiteCollection<Monument> collection = m_database.GetCollection<Monument>(CollectionName);
            int result = collection.Upsert(itemsToUpdate);

            UpdateCache();

            return result;
        }

        private void UpdateCache()
        {
            SyncHelper.OperationWithLock(ref m_slock, () =>
            {
                ILiteCollection<Monument> collection = m_database.GetCollection<Monument>(CollectionName);
                IList<Monument> monuments = collection.FindAll().ToList();
                if (monuments.Any())
                {
                    m_monumentsCache = monuments;
                    m_logger.LogInformation($"Monuments cache updated with {m_monumentsCache.Count} monuments");
                }
            });
        }

        #region IDisposable

        private bool m_disposed;

        public void Dispose()
        {
            if (m_disposed)
            {
                return;
            }

            m_database.Dispose();

            m_logger.LogInformation("Monuments repository finalized");

            GC.SuppressFinalize(this);

            m_disposed = true;
        }

        #endregion IDisposable
    }
}
