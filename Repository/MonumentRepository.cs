namespace MonumentService.Repository
{
    using LiteDB;
    using MonumentService.Model;
    using System.Linq.Expressions;
    using System.Reflection;

    public class MonumentRepository : IMonumentRepository, IDisposable
    {
        private static readonly string MonumentsDatabase = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? ".", "monuments.db");
        private const string CollectionName = "monuments";

        // SpinLocks can't be read-only (see https://stackoverflow.com/a/11225279)
        // This is needed because of bugs in LiteDB that cause 500 on multi-threading
        private SpinLock m_slock = new();

        private readonly ILogger m_logger;
        private readonly LiteDatabase m_database;

        public MonumentRepository(ILogger<MonumentRepository> logger)
        {
            m_logger = logger;
            m_database = new LiteDatabase(new ConnectionString { Filename = MonumentsDatabase });

            m_logger.LogInformation($"Monuments database initialized at {MonumentsDatabase}");
        }

        public IEnumerable<Monument> GetAll()
        {
            return Get(_ => true);
        }

        public IEnumerable<Monument> Get(Expression<Func<Monument, bool>> filter)
        {
            ILiteCollection<Monument> collection = m_database.GetCollection<Monument>(CollectionName);
            return SyncHelper.OperationWithSpinLock(ref m_slock, () => collection.Find(filter));
        }

        public int Add(params Monument[] itemsToAdd)
        {
            ILiteCollection<Monument> collection = m_database.GetCollection<Monument>(CollectionName);
            return SyncHelper.OperationWithSpinLock(ref m_slock, () => collection.Insert(itemsToAdd));
        }

        public int Update(params Monument[] itemsToUpdate)
        {
            ILiteCollection<Monument> collection = m_database.GetCollection<Monument>(CollectionName);
            return SyncHelper.OperationWithSpinLock(ref m_slock, () => collection.Update(itemsToUpdate));
        }

        public int AddOrUpdate(params Monument[] itemsToUpdate)
        {
            ILiteCollection<Monument> collection = m_database.GetCollection<Monument>(CollectionName);
            return SyncHelper.OperationWithSpinLock(ref m_slock, () => collection.Upsert(itemsToUpdate));
        }

        public int Delete(params Monument[] itemsToDelete)
        {
            return Delete(item => itemsToDelete.Contains(item));
        }

        public int Delete(Expression<Func<Monument, bool>> filter)
        {
            ILiteCollection<Monument> collection = m_database.GetCollection<Monument>(CollectionName);
            return SyncHelper.OperationWithSpinLock(ref m_slock, () => collection.DeleteMany(filter));
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
