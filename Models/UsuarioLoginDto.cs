using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MauiAppPeliculas.Models
{
    public class UsuarioLoginDto
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        // Tu API usa "Contraseña" con ñ. Mapeamos correctamente el JSON:
        [JsonPropertyName("contraseña")]
        public string Contrasena { get; set; } = string.Empty;
    }
}
