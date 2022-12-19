namespace MonumentService.Model
{
    using LiteDB;
    using System.Text.Json.Serialization;

    public class MonumentBase
    {
        [BsonId]
        [JsonPropertyOrder(order : 1)]
        public int Id { get; set; }

        [JsonPropertyOrder(order : 3)]
        public string? Nombre { get; set; }

        [JsonPropertyOrder(order : 5)]
        public bool HasImage { get; set; } = false;
    }

    public class Monument : MonumentBase
    {
        [JsonPropertyOrder(order : 2)]
        public int? IdBienCultural { get; set; }

        [JsonPropertyOrder(order : 4)]
        public string? Descripcion { get; set; }

        [JsonPropertyOrder(order : 6)]
        public string? Calle { get; set; }

        [JsonPropertyOrder(order : 7)]
        public string? CodigoPostal { get; set; }

        [JsonPropertyOrder(order : 8)]
        public string? Localidad { get; set; }

        [JsonPropertyOrder(order : 9)]
        public string? Municipio { get; set; }

        [JsonPropertyOrder(order : 10)]
        public string? Provincia { get; set; }

        [JsonPropertyOrder(order : 11)]
        public Location? Localizacion { get; set; }

        [JsonPropertyOrder(order : 12)]
        public string? TipoMonumento { get; set; }

        [JsonPropertyOrder(order: 13)]
        public IEnumerable<string> TiposConstruccion { get; set; } = new List<string>();

        [JsonPropertyOrder(order : 14)]
        public string? Clasificacion { get; set; }

        [JsonPropertyOrder(order : 15)]
        public IEnumerable<string> PeriodosHistoricos { get; set; } = new List<string>();
    }
}
