namespace MonumentService.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MonumentService.Images;
    using MonumentService.Model;
    using MonumentService.Repository;
    using System.Reflection.Metadata;

    [ApiController]
    [Route("monuments")]
    public class MonumentController : Controller
    {
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
    }
}
