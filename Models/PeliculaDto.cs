using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


namespace MauiAppPeliculas.Models
{

    namespace MauiAppPeliculas.Models
    {
        public class PeliculaDto
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("titulo")]
            public string Titulo { get; set; } = string.Empty;

            [JsonPropertyName("descripcion")]
            public string? Descripcion { get; set; }

            [JsonPropertyName("imagen")]
            public string? Imagen { get; set; }

            [JsonPropertyName("genero")]
            public string? Genero { get; set; }

            [JsonPropertyName("anio")]
            public int? Anio { get; set; }
        }
    }

}