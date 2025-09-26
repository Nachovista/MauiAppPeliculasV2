// /Repositories/PeliculasRepositoryHttp.cs
using MauiAppPeliculas.Models;
using MauiAppPeliculas.Models.MauiAppPeliculas.Models;
using MauiAppPeliculas.Services;

namespace MauiAppPeliculas.Repositories
{
    public class PeliculasRepositoryHttp
    {
        private readonly PeliculasDtoServiceHttp _service;

        // Cache simple
        private List<PeliculaDto>? _cacheLista;
        private DateTime _cacheListaTs;
        private readonly TimeSpan _ttl = TimeSpan.FromMinutes(2);
        private readonly Dictionary<int, PeliculaDto> _cachePorId = new();

        public PeliculasRepositoryHttp(PeliculasDtoServiceHttp service)
        {
            _service = service;
        }

        public async Task<List<PeliculaDto>> GetAllAsync(CancellationToken ct = default)
        {
            if (_cacheLista != null && DateTime.UtcNow - _cacheListaTs < _ttl)
                return _cacheLista;

            var data = await _service.GetAllAsync(ct);
            _cacheLista = data;
            _cacheListaTs = DateTime.UtcNow;

            // Sincronizo cache por id
            _cachePorId.Clear();
            foreach (var p in data)
                _cachePorId[p.Id] = p;

            return data;
        }

        public async Task<PeliculaDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            if (_cachePorId.TryGetValue(id, out var hit))
                return hit;

            var dto = await _service.GetByIdAsync(id, ct);
            if (dto != null) _cachePorId[id] = dto;
            return dto;
        }

        public async Task<PeliculaDto?> CreateAsync(PeliculaDto pelicula, CancellationToken ct = default)
        {
            var created = await _service.CreateAsync(pelicula, ct);
            InvalidateCache();
            return created;
        }

        public async Task UpdateAsync(PeliculaDto pelicula, CancellationToken ct = default)
        {
            await _service.UpdateAsync(pelicula, ct);
            InvalidateCache();
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await _service.DeleteAsync(id, ct);
            InvalidateCache();
        }

        public async Task<List<PeliculaDto>> SearchAsync(string? genero = null, string? titulo = null, CancellationToken ct = default)
        {
            // La búsqueda siempre va a servidor (puedes cachearla si querés)
            return await _service.SearchAsync(genero, titulo, ct);
        }

        public void InvalidateCache()
        {
            _cacheLista = null;
            _cachePorId.Clear();
        }
    }
}