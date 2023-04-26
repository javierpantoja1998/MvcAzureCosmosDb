using Newtonsoft.Json;

namespace MvcAzureCosmosDb.Models
{
    public class Vehiculo
    {
        [JsonProperty(PropertyName="id")]
        public string Id { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Imagen { get; set; }
        public int VelocidadMaxima { get; set; }
        //EL MOTOR SERA OPCIONAL
        public Motor Motor { get; set; }

    }
}
