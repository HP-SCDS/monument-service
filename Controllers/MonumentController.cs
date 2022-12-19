namespace MonumentService.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MonumentService.Images;
    using MonumentService.Model;
    using MonumentService.Repository;
    using MonumentService.Util;
    using Swashbuckle.AspNetCore.Annotations;
    using System.Net;

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
        [SwaggerOperation(Description = "Obtiene todos los monumentos disponibles. Esta operación puede llevar un tiempo significativo. **Se recomienda el uso de otras operaciones de obtención de monumentos**.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetAll()
        {
            return m_repository.GetAll();
        }

        [HttpGet("base")]
        [SwaggerOperation(Description = "Obtiene todos los monumentos disponibles, pero sólo con los datos básicos. Esta operación es más rápida que la operación de obtención de todos los monumentos.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<MonumentBase> GetAllBase()
        {
            return GetAll().ToBase();
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Description = "Obtiene un monumento con un ID concreto. Si no se encuentra, se devuelve un código 404.")]
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

        [HttpGet("{id}/image")]
        [SwaggerOperation(Description = "Devuelve la imagen para un monumento con un ID concreto. Si no se encuentra el monumento o su imagen, se devuelve un código 404.")]
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

        [HttpGet("search/{query}")]
        [SwaggerOperation(Description = "Busca monumentos, contemplando la inclusión de la cadena de búsqueda en nombre o descripción. No distingue entre mayúsculas y minúsculas.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> Search(string? query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return m_repository.GetAll();
            }

            return m_repository.Get(m => StringComparer.ContainsInvariantIgnoreCase(m.Nombre, query) || StringComparer.ContainsInvariantIgnoreCase(m.Descripcion, query));
        }

        [HttpGet("search/{query}/base")]
        [SwaggerOperation(Description = "Busca monumentos, contemplando la inclusión de la cadena de búsqueda en nombre o descripción, pero sólo con los datos básicos. No distingue entre mayúsculas y minúsculas.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<MonumentBase> SearchBase(string? query)
        {
            return Search(query).ToBase();
        }

        [HttpGet("provincia/{provincia}")]
        [SwaggerOperation(Description = "Obtiene monumentos de una provincia concreta. Se puede utilizar el endpoint de *facets* para obtener las provincias. No distingue entre mayúsculas y minúsculas.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetByProvincia(string provincia)
        {
            return m_repository.Get(m => StringComparer.CompareInvariantIgnoreCase(m.Provincia, provincia));
        }

        [HttpGet("provincia/{provincia}/base")]
        [SwaggerOperation(Description = "Obtiene monumentos de una provincia concreta, pero sólo con los datos básicos. Se puede utilizar el endpoint de *facets* para obtener las provincias. No distingue entre mayúsculas y minúsculas.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<MonumentBase> GetByProvinciaBase(string provincia)
        {
            return GetByProvincia(provincia).ToBase();
        }

        [HttpGet("municipio/{municipio}")]
        [SwaggerOperation(Description = "Obtiene monumentos de un municipio concreto. No distingue entre mayúsculas y minúsculas.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetByMunicipio(string municipio)
        {
            return m_repository.Get(m => StringComparer.CompareInvariantIgnoreCase(m.Municipio, municipio));
        }

        [HttpGet("municipio/{municipio}/base")]
        [SwaggerOperation(Description = "Obtiene monumentos de un municipio concreto, pero sólo con los datos básicos. No distingue entre mayúsculas y minúsculas.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<MonumentBase> GetByMunicipioBase(string municipio)
        {
            return GetByMunicipio(municipio).ToBase();
        }

        [HttpGet("localidad/{localidad}")]
        [SwaggerOperation(Description = "Obtiene monumentos de una localidad concreta. No distingue entre mayúsculas y minúsculas.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetByLocalidad(string localidad)
        {
            return m_repository.Get(m => StringComparer.CompareInvariantIgnoreCase(m.Localidad, localidad));
        }

        [HttpGet("localidad/{localidad}/base")]
        [SwaggerOperation(Description = "Obtiene monumentos de una localidad concreta, pero sólo con los datos básicos. No distingue entre mayúsculas y minúsculas.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<MonumentBase> GetByLocalidadBase(string localidad)
        {
            return GetByLocalidad(localidad).ToBase();
        }

        [HttpGet("codigo-postal/{cp}")]
        [SwaggerOperation(Description = "Obtiene monumentos en un código postal concreto.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetByCodigoPostal(string cp)
        {
            return m_repository.Get(m => m.CodigoPostal == cp);
        }

        [HttpGet("codigo-postal/{cp}/base")]
        [SwaggerOperation(Description = "Obtiene monumentos en un código postal concreto, pero sólo con los datos básicos.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<MonumentBase> GetByCodigoPostalBase(string cp)
        {
            return GetByCodigoPostal(cp).ToBase();
        }

        [HttpGet("tipo-monumento/{tipo}")]
        [SwaggerOperation(Description = "Obtiene monumentos de un tipo concreto. Se puede utilizar el endpoint de *facets* para obtener los tipos. No distingue entre mayúsculas y minúsculas.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetByTipoMonumento(string tipo)
        {
            return m_repository.Get(m => StringComparer.CompareInvariantIgnoreCase(m.TipoMonumento, tipo));
        }

        [HttpGet("tipo-monumento/{tipo}/base")]
        [SwaggerOperation(Description = "Obtiene monumentos de un tipo concreto, pero sólo con los datos básicos. Se puede utilizar el endpoint de *facets* para obtener los tipos. No distingue entre mayúsculas y minúsculas.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<MonumentBase> GetByTipoMonumentoBase(string tipo)
        {
            return GetByTipoMonumento(tipo).ToBase();
        }

        [HttpGet("tipo-construccion/{tipo}")]
        [SwaggerOperation(Description = "Obtiene monumentos con un tipo de construcción concreto. Se puede utilizar el endpoint de *facets* para obtener los tipos. No distingue entre mayúsculas y minúsculas.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetByTipoConstruccion(string tipo)
        {
            return m_repository.Get(m => StringComparer.ContainsInvariantIgnoreCase(m.TiposConstruccion, tipo));
        }

        [HttpGet("tipo-construccion/{tipo}/base")]
        [SwaggerOperation(Description = "Obtiene monumentos con un tipo de construcción concreto, pero sólo con los datos básicos. Se puede utilizar el endpoint de *facets* para obtener los tipos. No distingue entre mayúsculas y minúsculas.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<MonumentBase> GetByTipoConstruccionBase(string tipo)
        {
            return GetByTipoConstruccion(tipo).ToBase();
        }

        [HttpGet("clasificacion/{clasificacion}")]
        [SwaggerOperation(Description = "Obtiene monumentos de una clasificación concreta. Se puede utilizar el endpoint de *facets* para obtener las clasificaciones. No distingue entre mayúsculas y minúsculas.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetByClasificacion(string clasificacion)
        {
            return m_repository.Get(m => StringComparer.CompareInvariantIgnoreCase(m.Clasificacion, clasificacion));
        }

        [HttpGet("clasificacion/{clasificacion}/base")]
        [SwaggerOperation(Description = "Obtiene monumentos de una clasificación concreta, pero sólo con los datos básicos. Se puede utilizar el endpoint de *facets* para obtener las clasificaciones. No distingue entre mayúsculas y minúsculas.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<MonumentBase> GetByClasificacionBase(string clasificacion)
        {
            return GetByClasificacion(clasificacion).ToBase();
        }

        [HttpGet("periodo-historico/{periodo}")]
        [SwaggerOperation(Description = "Obtiene monumentos enmarcados en un periodo histórico concreto. Se puede utilizar el endpoint de *facets* para obtener los periodos históricos. No distingue entre mayúsculas y minúsculas.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetByPeriodoHistorico(string periodo)
        {
            return m_repository.Get(m => StringComparer.ContainsInvariantIgnoreCase(m.PeriodosHistoricos, periodo));
        }

        [HttpGet("periodo-historico/{periodo}/base")]
        [SwaggerOperation(Description = "Obtiene monumentos enmarcados en un periodo histórico concreto, pero sólo con los datos básicos. Se puede utilizar el endpoint de *facets* para obtener los periodos históricos. No distingue entre mayúsculas y minúsculas.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<MonumentBase> GetByPeriodoHistoricoBase(string periodo)
        {
            return GetByPeriodoHistorico(periodo).ToBase();
        }

        [HttpGet("nearby")]
        [SwaggerOperation(Description = "Obtiene monumentos cercanos a un punto concreto. Debe especificarse latitud y longitud y la distancia. La distancia se expresa en **kilómetros**.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Monument> GetNearby(double latitude, double longitude, double distance)
        {
            // this is not optimal, we have to get them all and then filter because the database doesn't support such a complex query
            return m_repository.GetAll().Where(m => m.Localizacion != null
                && LocationHelper.CalculateDistance(latitude, longitude, m.Localizacion.Latitud, m.Localizacion.Longitud) <= distance);
        }

        [HttpGet("nearby/base")]
        [SwaggerOperation(Description = "Obtiene monumentos cercanos a un punto concreto, pero sólo con los datos básicos. Debe especificarse latitud y longitud y la distancia. La distancia se expresa en **kilómetros**.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<MonumentBase> GetNearbyBase(double latitude, double longitude, double distance)
        {
            return GetNearby(latitude, longitude, distance).ToBase();
        }
    }
}
