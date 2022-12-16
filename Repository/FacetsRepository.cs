namespace MonumentService.Repository
{
    using LiteDB;
    using System.Reflection;

    public class FacetsRepository : IFacetsRepository, IDisposable
    {
        private class StringWrapper
        {
            [BsonId]
            public string? Value { get; set; }
        }

        private static readonly string FacetsDatabase = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? ".", "facets.db");
        private const string ProvinciasCollectionName = "provincias";
        private const string TiposMonumentoCollectionName = "tiposMonumento";
        private const string TiposConstruccionCollectionName = "tiposConstruccion";
        private const string ClasificacionesCollectionName = "clasificaciones";
        private const string PeriodosHistoricosCollectionName = "periodosHistoricos";

        private readonly ILogger m_logger;
        private readonly LiteDatabase m_database;

        public FacetsRepository(ILogger<FacetsRepository> logger)
        {
            m_logger = logger;
            m_database = new LiteDatabase(new ConnectionString { Filename = FacetsDatabase });

            m_logger.LogInformation($"Facets database initialized at {FacetsDatabase}");
        }

        public IEnumerable<string> GetAllProvincias()
        {
            ILiteCollection<StringWrapper> collection = m_database.GetCollection<StringWrapper>(ProvinciasCollectionName);
            return collection.FindAll().Select(w => w.Value).Where(v => v != null).Cast<string>();
        }

        public int AddOrUpdateProvincias(params string[] itemsToUpdate)
        {
            ILiteCollection<StringWrapper> collection = m_database.GetCollection<StringWrapper>(ProvinciasCollectionName);
            return collection.Upsert(itemsToUpdate.Select(i => new StringWrapper { Value = i }));
        }

        public IEnumerable<string> GetAllTiposMonumento()
        {
            ILiteCollection<StringWrapper> collection = m_database.GetCollection<StringWrapper>(TiposMonumentoCollectionName);
            return collection.FindAll().Select(w => w.Value).Where(v => v != null).Cast<string>();
        }

        public int AddOrUpdateTiposMonumento(params string[] itemsToUpdate)
        {
            ILiteCollection<StringWrapper> collection = m_database.GetCollection<StringWrapper>(TiposMonumentoCollectionName);
            return collection.Upsert(itemsToUpdate.Select(i => new StringWrapper { Value = i }));
        }

        public IEnumerable<string> GetAllTiposConstruccion()
        {
            ILiteCollection<StringWrapper> collection = m_database.GetCollection<StringWrapper>(TiposConstruccionCollectionName);
            return collection.FindAll().Select(w => w.Value).Where(v => v != null).Cast<string>();
        }

        public int AddOrUpdateTiposConstruccion(params string[] itemsToUpdate)
        {
            ILiteCollection<StringWrapper> collection = m_database.GetCollection<StringWrapper>(TiposConstruccionCollectionName);
            return collection.Upsert(itemsToUpdate.Select(i => new StringWrapper { Value = i }));
        }

        public IEnumerable<string> GetAllClasificaciones()
        {
            ILiteCollection<StringWrapper> collection = m_database.GetCollection<StringWrapper>(ClasificacionesCollectionName);
            return collection.FindAll().Select(w => w.Value).Where(v => v != null).Cast<string>();
        }

        public int AddOrUpdateClasificaciones(params string[] itemsToUpdate)
        {
            ILiteCollection<StringWrapper> collection = m_database.GetCollection<StringWrapper>(ClasificacionesCollectionName);
            return collection.Upsert(itemsToUpdate.Select(i => new StringWrapper { Value = i }));
        }

        public IEnumerable<string> GetAllPeriodosHistoricos()
        {
            ILiteCollection<StringWrapper> collection = m_database.GetCollection<StringWrapper>(PeriodosHistoricosCollectionName);
            return collection.FindAll().Select(w => w.Value).Where(v => v != null).Cast<string>();
        }

        public int AddOrUpdatePeriodosHistoricos(params string[] itemsToUpdate)
        {
            ILiteCollection<StringWrapper> collection = m_database.GetCollection<StringWrapper>(PeriodosHistoricosCollectionName);
            return collection.Upsert(itemsToUpdate.Select(i => new StringWrapper { Value = i }));
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

            m_logger.LogInformation("Facets repository finalized");

            GC.SuppressFinalize(this);

            m_disposed = true;
        }

        #endregion IDisposable
    }
}