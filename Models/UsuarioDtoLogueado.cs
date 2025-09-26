// /Models/UsuarioDtoLogueado.cs
using System;
using System.Text.Json.Serialization;

namespace MauiAppPeliculas.Models
{
    /// <summary>
    /// Representa el estado de autenticación actual en el cliente:
    /// datos del usuario + token JWT + caducidad.
    /// </summary>
    public class UsuarioDtoLogueado
    {
        [JsonPropertyName("usuario")]
        public UsuarioDto? Usuario { get; set; }

        // Token JWT devuelto por la API en el login
        [JsonPropertyName("token")]
        public string? Token { get; set; }

        // (Opcional) Fecha/hora de expiración del token, si la API la provee
        [JsonPropertyName("token_expires_at")]
        public DateTimeOffset? TokenExpiresAt { get; set; }

        // Helper: ¿hay sesión válida?
        [JsonIgnore]
        public bool IsAuthenticated =>
            Usuario is not null &&
            !string.IsNullOrWhiteSpace(Token) &&
            (TokenExpiresAt is null || TokenExpiresAt > DateTimeOffset.UtcNow);

        // Helper: ¿tiene el rol indicado?
        public bool HasRole(string role) =>
            Usuario is not null &&
            !string.IsNullOrWhiteSpace(role) &&
            string.Equals(Usuario.Rol, role, StringComparison.OrdinalIgnoreCase);

        // Fábrica cómoda desde piezas sueltas
        public static UsuarioDtoLogueado From(
            UsuarioDto usuario,
            string? token,
            DateTimeOffset? tokenExpiresAt = null) => new()
            {
                Usuario = usuario,
                Token = token,
                TokenExpiresAt = tokenExpiresAt
            };
    }
}
