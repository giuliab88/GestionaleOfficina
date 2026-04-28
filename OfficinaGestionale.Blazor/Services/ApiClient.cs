using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using OfficinaGestionale.Api.DTOs;

namespace OfficinaGestionale.Blazor.Services;

public class ApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly AuthenticationStateProvider _auth;
    private readonly JwtTokenService _tokenService;

    public ApiClient(IHttpClientFactory factory, AuthenticationStateProvider auth, JwtTokenService tokenService)
    {
        _factory = factory;
        _auth = auth;
        _tokenService = tokenService;
    }

    private async Task<HttpClient> HttpAsync()
    {
        // se ho già il token lo riuso, altrimenti lo leggo dall'auth state
        string? jwt = _tokenService.Token;
        if (jwt is null)
        {
            AuthenticationState state = await _auth.GetAuthenticationStateAsync();
            jwt = state.User.FindFirstValue("jwt");
        }

        HttpClient http = _factory.CreateClient("Api");
        if (jwt is not null)
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwt);

        return http;
    }

    // clienti

    public async Task<List<ClienteDto>> GetClientiAsync()
    {
        HttpClient http = await HttpAsync();
        return await http.GetFromJsonAsync<List<ClienteDto>>("api/clienti") ?? [];
    }

    public async Task<bool> InserisciClienteAsync(ClienteDto dto)
    {
        HttpClient http = await HttpAsync();
        HttpResponseMessage r = await http.PostAsJsonAsync("api/clienti", dto);

        // aggiorno il DTO con il codice generato dall'API
        if (r.IsSuccessStatusCode)
        {
            ClienteDto? result = await r.Content.ReadFromJsonAsync<ClienteDto>();
            if (result is not null) dto.Codice = result.Codice;
        }

        return r.IsSuccessStatusCode;
    }

    public async Task<bool> AggiornaClienteAsync(ClienteDto dto)
    {
        HttpClient http = await HttpAsync();
        HttpResponseMessage r = await http.PutAsJsonAsync($"api/clienti/{dto.Codice}", dto);
        return r.IsSuccessStatusCode;
    }

    public async Task<bool> EliminaClienteAsync(string codice)
    {
        HttpClient http = await HttpAsync();
        HttpResponseMessage r = await http.DeleteAsync($"api/clienti/{Uri.EscapeDataString(codice)}");
        return r.IsSuccessStatusCode;
    }

    // veicoli

    public async Task<List<VeicoloDto>> GetVeicoliAsync()
    {
        HttpClient http = await HttpAsync();
        return await http.GetFromJsonAsync<List<VeicoloDto>>("api/veicoli") ?? [];
    }

    public async Task<bool> InserisciVeicoloAsync(VeicoloDto dto)
    {
        HttpClient http = await HttpAsync();
        HttpResponseMessage r = await http.PostAsJsonAsync("api/veicoli", dto);

        // aggiorno il DTO con il codice generato dall'API
        if (r.IsSuccessStatusCode)
        {
            VeicoloDto? result = await r.Content.ReadFromJsonAsync<VeicoloDto>();
            if (result is not null) dto.Codice = result.Codice;
        }

        return r.IsSuccessStatusCode;
    }

    public async Task<bool> AggiornaVeicoloAsync(VeicoloDto dto)
    {
        HttpClient http = await HttpAsync();
        HttpResponseMessage r = await http.PutAsJsonAsync($"api/veicoli/{dto.Codice}", dto);
        return r.IsSuccessStatusCode;
    }

    public async Task<bool> EliminaVeicoloAsync(string codice)
    {
        HttpClient http = await HttpAsync();
        HttpResponseMessage r = await http.DeleteAsync($"api/veicoli/{Uri.EscapeDataString(codice)}");
        return r.IsSuccessStatusCode;
    }

    // interventi

    public async Task<List<InterventoDto>> GetInterventiAsync()
    {
        HttpClient http = await HttpAsync();
        return await http.GetFromJsonAsync<List<InterventoDto>>("api/interventi") ?? [];
    }

    public async Task<bool> InserisciInterventoAsync(InterventoDto dto)
    {
        HttpClient http = await HttpAsync();
        HttpResponseMessage r = await http.PostAsJsonAsync("api/interventi", dto);

        // aggiorno il DTO con il codice generato dall'API
        if (r.IsSuccessStatusCode)
        {
            InterventoDto? result = await r.Content.ReadFromJsonAsync<InterventoDto>();
            if (result is not null) dto.Codice = result.Codice;
        }

        return r.IsSuccessStatusCode;
    }

    public async Task<bool> AggiornaInterventoAsync(InterventoDto dto)
    {
        HttpClient http = await HttpAsync();
        HttpResponseMessage r = await http.PutAsJsonAsync($"api/interventi/{dto.Codice}", dto);
        return r.IsSuccessStatusCode;
    }

    public async Task<bool> EliminaInterventoAsync(string codice)
    {
        HttpClient http = await HttpAsync();
        HttpResponseMessage r = await http.DeleteAsync($"api/interventi/{Uri.EscapeDataString(codice)}");
        return r.IsSuccessStatusCode;
    }

    // preventivi

    public async Task<List<PreventivoDto>> GetPreventiviAsync()
    {
        HttpClient http = await HttpAsync();
        return await http.GetFromJsonAsync<List<PreventivoDto>>("api/preventivi") ?? [];
    }

    public async Task<bool> InserisciPreventivoAsync(PreventivoDto dto)
    {
        HttpClient http = await HttpAsync();
        HttpResponseMessage r = await http.PostAsJsonAsync("api/preventivi", dto);

        // aggiorno il DTO con il codice generato dall'API
        if (r.IsSuccessStatusCode)
        {
            PreventivoDto? result = await r.Content.ReadFromJsonAsync<PreventivoDto>();
            if (result is not null) dto.Codice = result.Codice;
        }

        return r.IsSuccessStatusCode;
    }

    public async Task<bool> AggiornaPreventivoAsync(PreventivoDto dto)
    {
        HttpClient http = await HttpAsync();
        HttpResponseMessage r = await http.PutAsJsonAsync($"api/preventivi/{dto.Codice}", dto);
        return r.IsSuccessStatusCode;
    }

    public async Task<bool> EliminaPreventivoAsync(string codice)
    {
        HttpClient http = await HttpAsync();
        HttpResponseMessage r = await http.DeleteAsync($"api/preventivi/{Uri.EscapeDataString(codice)}");
        return r.IsSuccessStatusCode;
    }

    // fatture

    public async Task<List<FatturaDto>> GetFattureAsync()
    {
        HttpClient http = await HttpAsync();
        return await http.GetFromJsonAsync<List<FatturaDto>>("api/fatture") ?? [];
    }

    public async Task<bool> InserisciFatturaAsync(FatturaDto dto)
    {
        HttpClient http = await HttpAsync();
        HttpResponseMessage r = await http.PostAsJsonAsync("api/fatture", dto);

        // aggiorno il DTO con il codice generato dall'API
        if (r.IsSuccessStatusCode)
        {
            FatturaDto? result = await r.Content.ReadFromJsonAsync<FatturaDto>();
            if (result is not null) dto.Codice = result.Codice;
        }

        return r.IsSuccessStatusCode;
    }

    public async Task<bool> AggiornaFatturaAsync(FatturaDto dto)
    {
        HttpClient http = await HttpAsync();
        HttpResponseMessage r = await http.PutAsJsonAsync($"api/fatture/{dto.Codice}", dto);
        return r.IsSuccessStatusCode;
    }

    public async Task<bool> EliminaFatturaAsync(string codice)
    {
        HttpClient http = await HttpAsync();
        HttpResponseMessage r = await http.DeleteAsync($"api/fatture/{Uri.EscapeDataString(codice)}");
        return r.IsSuccessStatusCode;
    }

    // crea una fattura partendo da un preventivo esistente
    public async Task<FatturaDto?> ConvertiFatturaAsync(string codicePreventivo)
    {
        HttpClient http = await HttpAsync();
        HttpResponseMessage r = await http.PostAsync($"api/fatture/da-preventivo/{Uri.EscapeDataString(codicePreventivo)}", null);
        if (!r.IsSuccessStatusCode) return null;
        return await r.Content.ReadFromJsonAsync<FatturaDto>();
    }
}
