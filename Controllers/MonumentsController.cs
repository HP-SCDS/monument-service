namespace MonumentService.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MonumentService.Model;

    [ApiController]
    [Route("[controller]")]
    public class MonumentsController : Controller
    {
        private readonly ILogger<MonumentsController> m_logger;

        public MonumentsController(ILogger<MonumentsController> logger)
        {
            m_logger = logger;
        }

        [HttpGet(Name = "")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetAll()
        {
            return new List<Monument>();
        }
    }
}
