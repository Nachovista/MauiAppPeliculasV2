// /Models/LoginRequest.cs
namespace MauiAppPeliculas.Models
{
    public class LoginRequest
    {
        public string UserOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
