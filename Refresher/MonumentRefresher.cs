namespace MonumentService.Refresher
{
    using MonumentService.Images;
    using MonumentService.Model;
    using MonumentService.Repository;
    using Newtonsoft.Json.Linq;
    using System.Timers;
    using Timer = System.Timers.Timer;

    public class MonumentRefresher : IMonumentRefresher, IDisposable
    {
        private const string MonumentsUrl = "https://analisis.datosabiertos.jcyl.es/api/records/1.0/search/?dataset=relacion-monumentos&q=&rows=-1&facet=tipomonumento&facet=clasificacion&facet=tipoconstruccion&facet=periodohistorico&facet=poblacion_provincia";
        private const int RefreshMinutes = 720; // 12 hours

        private readonly Timer m_timer = new Timer(RefreshMinutes * 60 * 1000);
        private readonly ManualResetEvent m_refreshEvent = new ManualResetEvent(true);
        private bool m_refreshStopped;
        private readonly ElapsedEventHandler m_timerEventHandler;

        private readonly HttpClient m_client = new HttpClient();

        private readonly ILogger m_logger;
        private readonly IMonumentRepository m_monumentRepository;
        private readonly IFacetsRepository m_facetsRepository;
        private readonly IImageManager m_imageManager;

        public MonumentRefresher(ILogger<MonumentRefresher> logger, IMonumentRepository monumentRepository, IFacetsRepository facetsRepository, IImageManager imageManager)
        {
            m_logger = logger;
            m_monumentRepository = monumentRepository;
            m_facetsRepository = facetsRepository;
            m_imageManager = imageManager;

            m_timerEventHandler = async (_, _) => await RefreshMonuments();

            m_logger.LogInformation("Monument refresher initialized");
        }

        public void Start()
        {
            m_logger.LogInformation("Monument refresher started");

            // force a refresh if no monuments
            int monumentCount = m_monumentRepository.GetAll().Count();
            if (monumentCount <= 0)
            {
                m_logger.LogInformation("No monuments found in database, refresh forced");
                RefreshMonuments().Wait();
            }
            else
            {
                m_logger.LogInformation($"Found {monumentCount} monuments in database, no refresh needed");
            }

            m_timer.Elapsed += m_timerEventHandler;
            m_timer.Start();
        }

        private async Task RefreshMonuments()
        {
            lock (m_refreshEvent)
            {
                m_refreshEvent.WaitOne();
                m_refreshEvent.Reset();
            }

            if (m_refreshStopped)
            {
                return;
            }

            m_logger.LogInformation("Refreshing monuments...");

            bool success = false;

            try
            {
                HttpResponseMessage response = await m_client.GetAsync(MonumentsUrl);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        JToken? records = JObject.Parse(content).SelectToken("records");
                        if (records != null)
                        {
                            IList<MonumentJCyL> monumentsJCyL = records.Select(record => record.SelectToken("fields")?.ToObject<MonumentJCyL>()).Where(m => m != null).Cast<MonumentJCyL>().ToList();
                            if (monumentsJCyL != null && monumentsJCyL.Any())
                            {
                                m_logger.LogInformation($"Found {monumentsJCyL.Count} monuments at API");

                                List<Monument> monuments = monumentsJCyL.Select(ConvertMonument).Where(m => m != null).Cast<Monument>().ToList();
                                m_logger.LogInformation($"{monuments.Count} monuments parsed correctly from API");

                                SaveFacets(monuments);
                                await SaveImages(monuments);

                                // update monuments with image existence before saving them
                                monuments.ForEach(m => m.HasImage = m_imageManager.MonumentHasImage(m.Id));

                                SaveMonuments(monuments);

                                success = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, "Error refreshing monuments");
            }

            if (success)
            {
                m_logger.LogInformation("Monuments refreshed correctly");
            }
            else
            {
                m_logger.LogError("Monuments not refreshed correctly!");
            }

            m_refreshEvent.Set();
        }

        private Monument? ConvertMonument(MonumentJCyL source)
        {
            if (source == null || source.Identificador == null)
            {
                m_logger.LogWarning($"Monument ${source} is not valid, skipping conversion");
                return null;
            }

            Location? location = null;
            if (source.Coordenadas_Longitud != null && source.Coordenadas_Latitud != null)
            {
                location = new Location
                {
                    Latitud = source.Coordenadas_Latitud.Value,
                    Longitud = source.Coordenadas_Longitud.Value
                };
            }

            List<string> periodos = new List<string>();
            if (source.PeriodoHistorico != null)
            {
                string[] splitPeriodos = source.PeriodoHistorico.Split(';');
                periodos.AddRange(splitPeriodos);
            }

            int? bienInteresCultural = null;
            if (int.TryParse(source.IdentificadorBienInteresCultural, out int bienInteresCulturalParsed))
            {
                bienInteresCultural = bienInteresCulturalParsed;
            }

            return new Monument
            {
                Id = source.Identificador.Value,
                IdBienCultural = bienInteresCultural,
                Nombre = source.Nombre,
                Descripcion = source.Descripcion,
                Calle = source.Calle,
                CodigoPostal = source.CodigoPostal,
                Localidad = source.Poblacion_Localidad,
                Municipio = source.Poblacion_Municipio,
                Provincia = source.Poblacion_Provincia,
                Localizacion = location,
                TipoMonumento = source.TipoMonumento,
                TipoConstruccion = source.TipoConstruccion,
                Clasificacion = source.Clasificacion,
                PeriodosHistoricos = periodos
            };
        }
        
        private void SaveMonuments(IList<Monument> monuments)
        {
            int monumentsInserted = m_monumentRepository.AddOrUpdate(monuments.ToArray());
            m_logger.LogInformation($"{monumentsInserted} monuments added or updated to repository");
        }

        private void SaveFacets(IList<Monument> monuments)
        {
            int provinciasInserted = m_facetsRepository.AddOrUpdateProvincias(monuments
                .Select(m => m.Provincia).Where(p => p != null).Cast<string>().Distinct().ToArray());
            m_logger.LogInformation($"{provinciasInserted} provincias added or updated to repository");

            int tiposMonumentoInserted = m_facetsRepository.AddOrUpdateTiposMonumento(monuments
                .Select(m => m.TipoMonumento).Where(t => t != null).Cast<string>().Distinct().ToArray());
            m_logger.LogInformation($"{tiposMonumentoInserted} tipos monumento added or updated to repository");

            int tiposConstruccionInserted = m_facetsRepository.AddOrUpdateTiposConstruccion(monuments
                .Select(m => m.TipoConstruccion).Where(t => t != null).Cast<string>().Distinct().ToArray());
            m_logger.LogInformation($"{tiposConstruccionInserted} tipos construccion added or updated to repository");

            int clasificacionesInserted = m_facetsRepository.AddOrUpdateClasificaciones(monuments
                .Select(m => m.Clasificacion).Where(c => c != null).Cast<string>().Distinct().ToArray());
            m_logger.LogInformation($"{clasificacionesInserted} clasificaciones added or updated to repository");

            int periodosHistoricosInserted = m_facetsRepository.AddOrUpdatePeriodosHistoricos(monuments
                .Where(m => m.PeriodosHistoricos != null).SelectMany(m => m.PeriodosHistoricos).Where(p => p != null).Cast<string>().Distinct().ToArray());
            m_logger.LogInformation($"{periodosHistoricosInserted} periodos historicos added or updated to repository");
        }

        private async Task SaveImages(IList<Monument> monuments)
        {
            int imagesSaved = (await Task.WhenAll(monuments.Select(async m => await m_imageManager.SaveImageForMonument(m)))).Where(r => r).Count();
            m_logger.LogInformation($"{imagesSaved} images saved for {monuments.Count} monuments");
        }

        #region IDisposable

        private bool m_disposed;

        public void Dispose()
        {
            if (m_disposed)
            {
                return;
            }

            m_timer.Elapsed -= m_timerEventHandler;
            m_timer.Stop();

            lock (m_refreshEvent)
            {
                m_refreshEvent.WaitOne();
                m_refreshStopped = true;
            }

            m_timer.Dispose();
            m_refreshEvent.Dispose();

            m_logger.LogInformation("Monument refresher finalized");

            GC.SuppressFinalize(this);

            m_disposed = true;
        }

        #endregion IDisposable
    }
}
