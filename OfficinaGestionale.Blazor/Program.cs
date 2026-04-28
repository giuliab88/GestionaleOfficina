using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OfficinaGestionale.Blazor.Components;
using OfficinaGestionale.Blazor.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

string apiBaseUrl = builder.Configuration["Api:BaseUrl"]!;
builder.Services.AddHttpClient("Api", c => c.BaseAddress = new Uri(apiBaseUrl));

builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<ApiClient>();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapPost("/account/login", async (HttpContext ctx, IHttpClientFactory httpFactory) =>
{
    IFormCollection form = await ctx.Request.ReadFormAsync();
    string email = form["email"].ToString().Trim();
    string password = form["password"].ToString();

    var (jwt, nome, ruolo) = await LoginApiAsync(httpFactory, email, password);
    if (jwt is null)
        return Results.Redirect("/login?errore=1");

    // salvo nel cookie i dati utente e il token ricevuto dall'API
    List<Claim> claims =
    [
        new(ClaimTypes.Name,  nome  ?? email),
        new(ClaimTypes.Email, email),
        new(ClaimTypes.Role,  ruolo ?? "Operatore"),
        new Claim("jwt", jwt),
    ];

    ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    return Results.Redirect("/");
}).DisableAntiforgery();

app.MapPost("/account/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).DisableAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static async Task<(string? Jwt, string? Nome, string? Ruolo)> LoginApiAsync(
    IHttpClientFactory httpFactory, string email, string password)
{
    try
    {
        HttpClient http = httpFactory.CreateClient("Api");

        // delego il controllo credenziali all'API
        HttpResponseMessage r = await http.PostAsJsonAsync(
            "api/auth/login", new { Email = email, Password = password });

        if (!r.IsSuccessStatusCode) return (null, null, null);

        using JsonDocument doc = await r.Content.ReadFromJsonAsync<JsonDocument>()
            ?? throw new InvalidOperationException();

        string? jwt = doc.RootElement.GetProperty("token").GetString();
        string? nome = doc.RootElement.GetProperty("nome").GetString();
        string? ruolo = doc.RootElement.GetProperty("ruolo").GetString();

        return (jwt, nome, ruolo);
    }
    catch
    {
        return (null, null, null);
    }
}