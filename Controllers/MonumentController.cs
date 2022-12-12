namespace MonumentService.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MonumentService.Model;
    using MonumentService.Repository;

    [ApiController]
    [Route("monuments")]
    public class MonumentController : Controller
    {
        private readonly ILogger m_logger;
        private readonly IMonumentRepository m_repository;

        public MonumentController(ILogger<MonumentController> logger, IMonumentRepository repository)
        {
            m_logger = logger;
            m_repository = repository;
        }

        [HttpGet(Name = "")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetAll()
        {
            return m_repository.GetAll();
        }
    }
}
