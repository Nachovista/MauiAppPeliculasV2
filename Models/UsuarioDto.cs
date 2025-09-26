using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MauiAppPeliculas.Models
{
    public class UsuarioDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("imagen")]
        public string? Imagen { get; set; }

        [JsonPropertyName("rol")]
        public string Rol { get; set; } = string.Empty;
    }
}
