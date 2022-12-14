namespace MonumentService.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MonumentService.Repository;
    using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(Description = "Obtiene una lista de todas las provincias con monumentos asociados.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<string> GetProvincias()
        {
            return m_repository.GetAllProvincias();
        }

        [HttpGet("tipos-monumento")]
        [SwaggerOperation(Description = "Obtiene una lista de todos los tipos de monumentos.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<string> GetTiposMonumento()
        {
            return m_repository.GetAllTiposMonumento();
        }

        [HttpGet("tipos-construccion")]
        [SwaggerOperation(Description = "Obtiene una lista de todos los tipos de construcción para los monumentos.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<string> GetTiposConstruccion()
        {
            return m_repository.GetAllTiposConstruccion();
        }

        [HttpGet("clasificaciones")]
        [SwaggerOperation(Description = "Obtiene una lista de todas las clasificaciones de los monumentos.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<string> GetClasificaciones()
        {
            return m_repository.GetAllClasificaciones();
        }

        [HttpGet("periodos-historicos")]
        [SwaggerOperation(Description = "Obtiene una lista de todos los periodos históricos con monumentos asociados.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<string> GetPeriodosHistoricos()
        {
            return m_repository.GetAllPeriodosHistoricos();
        }
    }
}
