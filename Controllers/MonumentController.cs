namespace MonumentService.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MonumentService.Images;
    using MonumentService.Model;
    using MonumentService.Repository;

    [ApiController]
    [Route("monuments")]
    public class MonumentController : Controller
    {
        private const double EarthRadiusInKilometers = 6371;

        private readonly ILogger m_logger;
        private readonly IMonumentRepository m_repository;
        private readonly IImageManager m_imageManager;

        public MonumentController(ILogger<MonumentController> logger, IMonumentRepository repository, IImageManager imageManager)
        {
            m_logger = logger;
            m_repository = repository;
            m_imageManager = imageManager;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetAll()
        {
            return m_repository.GetAll();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Monument> GetById(int id)
        {
            Monument? monument = m_repository.Get(m => m.Id == id).FirstOrDefault();
            if (monument == null)
            {
                return NotFound();
            }

            return monument;
        }

        [HttpGet("provincia/{provincia}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetByProvincia(string provincia)
        {
            return m_repository.Get(m => m.Provincia == provincia);
        }

        [HttpGet("tipo-monumento/{tipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetByTipoMonumento(string tipo)
        {
            return m_repository.Get(m => m.TipoMonumento == tipo);
        }

        [HttpGet("tipo-construccion/{tipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetByTipoConstruccion(string tipo)
        {
            return m_repository.Get(m => m.TipoConstruccion == tipo);
        }

        [HttpGet("clasificacion/{clasificacion}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetByClasificacion(string clasificacion)
        {
            return m_repository.Get(m => m.Clasificacion == clasificacion);
        }

        [HttpGet("periodo-historico/{periodo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetByPeriodoHistorico(string periodo)
        {
            return m_repository.Get(m => m.PeriodosHistoricos != null && m.PeriodosHistoricos.Contains(periodo));
        }

        [HttpGet("{id}/image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetImage(int id)
        {
            byte[]? imageData = m_imageManager.GetImageForMonument(id);
            if (imageData == null)
            {
                return NotFound();
            }

            return File(imageData, "image/jpeg");
        }

        [HttpGet("nearby")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetNearby(double latitude, double longitude, double distance)
        {
            // this is not optimal, we have to get them all and then filter because the database doesn't support such a complex query
            return m_repository.GetAll().Where(m => m.Localizacion != null
                && CalculateDistance(latitude, longitude, m.Localizacion.Latitud, m.Localizacion.Longitud) <= distance);
        }

        // TODO: these calculations belong someplace else but leave them here for now
        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // convert latitude and longitude to radians
            lat1 = ConvertToRadians(lat1);
            lon1 = ConvertToRadians(lon1);
            lat2 = ConvertToRadians(lat2);
            lon2 = ConvertToRadians(lon2);

            // calculate differences between the coordinates
            double latDiff = lat2 - lat1;
            double lonDiff = lon2 - lon1;

            // calculate the Haversine formula
            double a = Math.Sin(latDiff / 2) * Math.Sin(latDiff / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(lonDiff / 2) * Math.Sin(lonDiff / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // calculate the distance in kilometers
            double distance = EarthRadiusInKilometers * c;

            return distance;
        }

        private static double ConvertToRadians(double value)
        {
            return Math.PI / 180 * value;
        }
    }
}
