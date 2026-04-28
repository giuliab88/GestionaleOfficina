namespace OfficinaGestionale.Blazor.Services;

// tiene il JWT in memoria così ApiClient non lo rilegge ogni volta dall'auth state
public class JwtTokenService
{
    public string? Token { get; set; }
}
