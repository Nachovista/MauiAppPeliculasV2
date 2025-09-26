// /Services/UsuarioDtoServiceHttp.cs
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MauiAppPeliculas.Models;

namespace MauiAppPeliculas.Services
{
    public class UsuarioDtoServiceHttp
    {
        private readonly HttpClient _http;

        private const string Resource = "api/Usuario";          // CRUD base
        private const string LoginEndpoint = "api/Usuario/login";    // Login
        private const string RegisterEndpoint = "api/Usuario/register"; // Opcional

        public UsuarioDtoLogueado Sesion { get; private set; } = new();

        public UsuarioDto? UsuarioDtoLogueado => Sesion.Usuario;

        public UsuarioDtoServiceHttp(HttpClient http) => _http = http;

        public async Task<UsuarioDto?> LoginAsync(string userOrEmail, string password, CancellationToken ct = default)
        {
            var req = new LoginRequest { UserOrEmail = userOrEmail, Password = password };

            using var resp = await _http.PostAsJsonAsync(LoginEndpoint, req, ct);
            var raw = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode) return null;

            // Primero intento sesión enriquecida (por si algún día agregás token)
            try
            {
                var sesion = JsonSerializer.Deserialize<UsuarioDtoLogueado>(
                    raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (sesion?.Usuario != null)
                {
                    Sesion.Usuario = sesion.Usuario;
                    Sesion.Token = sesion.Token;
                    Sesion.TokenExpiresAt = sesion.TokenExpiresAt;

                    if (!string.IsNullOrWhiteSpace(Sesion.Token))
                        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sesion.Token);

                    return Sesion.Usuario;
                }
            }
            catch { /* fallback abajo */ }

            // Fallback: UsuarioDto “plano”
            var usuario = JsonSerializer.Deserialize<UsuarioDto>(
                raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (usuario == null) return null;

            Sesion.Usuario = usuario;
            Sesion.Token = null;
            Sesion.TokenExpiresAt = null;

            return usuario;
        }

        public void Logout()
        {
            Sesion = new UsuarioDtoLogueado();
            _http.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<List<UsuarioDto>> GetAllAsync(CancellationToken ct = default)
        {
            using var resp = await _http.GetAsync(Resource, ct);
            if (!resp.IsSuccessStatusCode) return new();

            var raw = await resp.Content.ReadAsStringAsync(ct);
            var data = JsonSerializer.Deserialize<List<UsuarioDto>>(
                raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data ?? new();
        }

        public Task<UsuarioDto?> GetByIdAsync(int id, CancellationToken ct = default)
            => _http.GetFromJsonAsync<UsuarioDto>($"{Resource}/{id}", ct);

        // === Alta (Create) contra POST api/Usuario ===
        public async Task<UsuarioDto?> CreateAsync(UsuarioDto usuario, CancellationToken ct = default)
        {
            using var resp = await _http.PostAsJsonAsync(Resource, usuario, ct);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<UsuarioDto>(cancellationToken: ct);
        }

        // === Alta alternativa (Register) contra POST api/Usuario/register ===
        public async Task<UsuarioDto?> RegisterAsync(UsuarioRegisterDto dto, CancellationToken ct = default)
        {
            using var resp = await _http.PostAsJsonAsync(RegisterEndpoint, dto, ct);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<UsuarioDto>(cancellationToken: ct);
        }

        public async Task UpdateAsync(UsuarioDto usuario, CancellationToken ct = default)
        {
            using var resp = await _http.PutAsJsonAsync($"{Resource}/{usuario.Id}", usuario, ct);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            using var resp = await _http.DeleteAsync($"{Resource}/{id}", ct);
            resp.EnsureSuccessStatusCode();
        }

        public async Task<List<UsuarioDto>> SearchAsync(string? email = null, string? rol = null, CancellationToken ct = default)
        {
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(email)) qs.Add($"email={Uri.EscapeDataString(email)}");
            if (!string.IsNullOrWhiteSpace(rol)) qs.Add($"rol={Uri.EscapeDataString(rol)}");

            var url = qs.Count == 0 ? Resource : $"{Resource}?{string.Join("&", qs)}";
            return await _http.GetFromJsonAsync<List<UsuarioDto>>(url, ct) ?? new();
        }

        // ==== Tipos auxiliares cliente ====
        public class LoginRequest
        {
            public string UserOrEmail { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
