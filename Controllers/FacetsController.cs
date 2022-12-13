namespace MonumentService.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MonumentService.Repository;

    [ApiController]
    [Route("facets")]
    public class FacetsController : Controller
    {
        private readonly ILogger m_logger;
        private readonly IFacetsRepository m_repository;

        public FacetsController(ILogger<FacetsController> logger, IFacetsRepository repository)
        {
            m_logger = logger;
            m_repository = repository;
        }

        [HttpGet("provincias")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<string> GetProvincias()
        {
            return m_repository.GetAllProvincias();
        }

        [HttpGet("tipos-monumento")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<string> GetTiposMonumento()
        {
            return m_repository.GetAllTiposMonumento();
        }

        [HttpGet("tipos-construccion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<string> GetTiposConstruccion()
        {
            return m_repository.GetAllTiposConstruccion();
        }

        [HttpGet("clasificaciones")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<string> GetClasificaciones()
        {
            return m_repository.GetAllClasificaciones();
        }

        [HttpGet("periodos-historicos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<string> GetPeriodosHistoricos()
        {
            return m_repository.GetAllPeriodosHistoricos();
        }
    }
}
