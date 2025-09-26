// /Services/PeliculasDtoServiceHttp.cs
using System.Net.Http.Json;
using System.Text.Json;
using MauiAppPeliculas.Models;
using MauiAppPeliculas.Models.MauiAppPeliculas.Models;

namespace MauiAppPeliculas.Services
{
    public class PeliculasDtoServiceHttp
    {
        private readonly HttpClient _http;

        // ¡Coincide exacto con tu controlador: [Route("api/[controller]")] => api/Peliculas
        private const string Resource = "api/Peliculas";

        public PeliculasDtoServiceHttp(HttpClient http) => _http = http;

        // ---------- GET ----------
        public async Task<List<PeliculaDto>> GetAllAsync(CancellationToken ct = default)
        {
            using var resp = await _http.GetAsync(Resource, ct);
            var raw = await resp.Content.ReadAsStringAsync(ct);
            System.Diagnostics.Debug.WriteLine($"Peliculas GET {(int)resp.StatusCode}: {raw}");
            resp.EnsureSuccessStatusCode();

            return JsonSerializer.Deserialize<List<PeliculaDto>>(
                       raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new();
        }

        public Task<PeliculaDto?> GetByIdAsync(int id, CancellationToken ct = default)
            => _http.GetFromJsonAsync<PeliculaDto>($"{Resource}/{id}", ct);

        // ---------- POST ----------
        public async Task<PeliculaDto> CreateAsync(PeliculaDto dto, CancellationToken ct = default)
        {
            dto = Normalize(dto);

            using var resp = await _http.PostAsJsonAsync(Resource, dto, ct);
            var raw = await resp.Content.ReadAsStringAsync(ct);
            System.Diagnostics.Debug.WriteLine($"Peliculas POST {(int)resp.StatusCode}: {raw}");
            resp.EnsureSuccessStatusCode();

            return JsonSerializer.Deserialize<PeliculaDto>(
                       raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        // ---------- PUT ----------
        public async Task UpdateAsync(PeliculaDto dto, CancellationToken ct = default)
        {
            dto = Normalize(dto);

            using var resp = await _http.PutAsJsonAsync($"{Resource}/{dto.Id}", dto, ct);
            var raw = await resp.Content.ReadAsStringAsync(ct);
            System.Diagnostics.Debug.WriteLine($"Peliculas PUT {(int)resp.StatusCode}: {raw}");
            resp.EnsureSuccessStatusCode();
        }

        // ---------- DELETE ----------
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            using var resp = await _http.DeleteAsync($"{Resource}/{id}", ct);
            var raw = await resp.Content.ReadAsStringAsync(ct);
            System.Diagnostics.Debug.WriteLine($"Peliculas DELETE {(int)resp.StatusCode}: {raw}");
            resp.EnsureSuccessStatusCode();
        }

        // ---------- (opcional) búsqueda ----------
        public async Task<List<PeliculaDto>> SearchAsync(string? genero = null, string? titulo = null, CancellationToken ct = default)
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(genero)) qs.Add($"genero={Uri.EscapeDataString(genero)}");
            if (!string.IsNullOrWhiteSpace(titulo)) qs.Add($"titulo={Uri.EscapeDataString(titulo)}");

            var url = qs.Count == 0 ? Resource : $"{Resource}?{string.Join("&", qs)}";
            return await _http.GetFromJsonAsync<List<PeliculaDto>>(url, ct) ?? new();
        }

        // ---------- helpers ----------
        private static PeliculaDto Normalize(PeliculaDto p)
        {
            p.Imagen = NormalizeImagen(p.Imagen);
            return p;
        }

        /// Normaliza la imagen para que funcione con wwwroot/Imagenes ó URL http/https
        private static string? NormalizeImagen(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "Imagenes/avatar1.png";
            s = s.Trim();

            // URL absoluta ok
            if (Uri.TryCreate(s, UriKind.Absolute, out var abs) &&
                (abs.Scheme == Uri.UriSchemeHttp || abs.Scheme == Uri.UriSchemeHttps))
                return s;

            // Ruta Windows -> sólo nombre
            if (s.Contains('\\') || s.Contains(':'))
                return $"Imagenes/{System.IO.Path.GetFileName(s)}";

            // Ya empieza con Imagenes/
            if (s.StartsWith("Imagenes/", StringComparison.OrdinalIgnoreCase))
                return s;

            // Nombre suelto
            return $"Imagenes/{s}";
        }
    }
}
