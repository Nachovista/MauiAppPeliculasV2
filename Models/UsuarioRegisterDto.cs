using System.Text.Json.Serialization;

namespace MauiAppPeliculas.Models
{
    public class UsuarioRegisterDto
    {
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("imagen")]
        public string? Imagen { get; set; }

        [JsonPropertyName("rol")]
        public string Rol { get; set; } = "usuario"; // por defecto en minúscula

        [JsonPropertyName("contraseña")]
        public string Contrasena { get; set; } = string.Empty;
    }
}
