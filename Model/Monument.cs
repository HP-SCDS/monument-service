namespace MonumentService.Model
{
    using LiteDB;

    public class Monument
    {
        [BsonId]
        public int Id {  get; set; }

        public int? IdBienCultural { get; set; }

        public string? Nombre { get; set; }

        public string? Descripcion { get; set; }

        public string? Calle { get; set; }

        public string? CodigoPostal { get; set; }

        public string? Localidad { get; set; }

        public string? Municipio { get; set; }

        public string? Provincia { get; set; }

        public Location? Localizacion { get; set; }

        public string? TipoMonumento { get; set; }

        public string? TipoConstruccion { get; set; }

        public string? Clasificacion { get; set; }

        public IEnumerable<string> PeriodosHistoricos { get; set; } = new List<string>();

        public bool HasImage { get; set; } = false;
    }
}
