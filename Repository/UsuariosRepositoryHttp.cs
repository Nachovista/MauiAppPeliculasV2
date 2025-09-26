using MauiAppPeliculas.Models;
using MauiAppPeliculas.Services;

namespace MauiAppPeliculas.Repositories
{
    public class UsuariosRepositoryHttp
    {
        private readonly UsuarioDtoServiceHttp _service;

        private List<UsuarioDto>? _cacheLista;
        private DateTime _cacheListaTs;
        private readonly TimeSpan _ttl = TimeSpan.FromMinutes(2);
        private readonly Dictionary<int, UsuarioDto> _cachePorId = new();

        public UsuariosRepositoryHttp(UsuarioDtoServiceHttp service)
        {
            _service = service;
        }

        public async Task<List<UsuarioDto>> GetAllAsync(CancellationToken ct = default)
        {
            if (_cacheLista != null && DateTime.UtcNow - _cacheListaTs < _ttl)
                return _cacheLista;

            var data = await _service.GetAllAsync(ct);
            _cacheLista = data;
            _cacheListaTs = DateTime.UtcNow;

            _cachePorId.Clear();
            foreach (var u in data)
                _cachePorId[u.Id] = u;

            return data;
        }

        public async Task<UsuarioDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            if (_cachePorId.TryGetValue(id, out var hit))
                return hit;

            var dto = await _service.GetByIdAsync(id, ct);
            if (dto != null) _cachePorId[id] = dto;
            return dto;
        }

        // Si tu UI usa "Agregar Usuario" con Create clásico:
        public async Task<UsuarioDto?> CreateAsync(UsuarioDto usuario, CancellationToken ct = default)
        {
            var created = await _service.CreateAsync(usuario, ct);
            InvalidateCache();
            return created;
        }

        // Si tu UI usa "Registrar Usuario" con el JSON que envía 'contraseña':
        public async Task<UsuarioDto?> RegisterAsync(UsuarioRegisterDto dto, CancellationToken ct = default)
        {
            var created = await _service.RegisterAsync(dto, ct);
            InvalidateCache();
            return created;
        }

        public async Task UpdateAsync(UsuarioDto usuario, CancellationToken ct = default)
        {
            await _service.UpdateAsync(usuario, ct);
            InvalidateCache();
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await _service.DeleteAsync(id, ct);
            InvalidateCache();
        }

        public async Task<List<UsuarioDto>> SearchAsync(string? email = null, string? rol = null, CancellationToken ct = default)
        {
            return await _service.SearchAsync(email, rol, ct);
        }

        public void InvalidateCache()
        {
            _cacheLista = null;
            _cachePorId.Clear();
        }
    }
}
