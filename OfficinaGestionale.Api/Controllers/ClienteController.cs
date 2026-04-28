using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficinaGestionale.Api.DTOs;
using OfficinaGestionale.Api.Services;

namespace OfficinaGestionale.Api.Controllers;

// controller per gestione clienti (richiede autenticazione)
[ApiController]
[Route("api/clienti")]
[Authorize]
public class ClienteController : ControllerBase
{
    private readonly IService<ClienteDto> _service;

    public ClienteController(IService<ClienteDto> service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ClienteDto>> ListaClienti()
    {
        return Ok(_service.CercaTutti());
    }

    // cerca cliente per codice
    [HttpGet("{codice}")]
    public ActionResult<ClienteDto> VisualizzaCliente(string codice)
    {
        ClienteDto? risultato = _service.CercaPerCodice(codice);
        return risultato is not null ? Ok(risultato) : NotFound();
    }

    // inserimento nuovo cliente
    [HttpPost]
    public ActionResult<ClienteDto> InserisciCliente([FromBody] ClienteDto cliDto)
    {
        ServiceResult result = _service.Inserisci(cliDto);
        if (!result.Success) return Errore(result);

        return CreatedAtAction(nameof(VisualizzaCliente), new { codice = cliDto.Codice }, cliDto);
    }

    [HttpPut("{codice}")]
    public IActionResult AggiornaCliente(string codice, [FromBody] ClienteDto cliDto)
    {
        // uso il codice della route per evitare mismatch con il body
        cliDto.Codice = codice;

        ServiceResult result = _service.Aggiorna(cliDto);
        if (!result.Success) return Errore(result);

        return NoContent();
    }

    [HttpDelete("{codice}")]
    public IActionResult EliminaCliente(string codice)
    {
        ServiceResult result = _service.Elimina(codice);
        if (!result.Success) return Errore(result);

        return NoContent();
    }

    // mappa gli errori del service in status HTTP
    private ObjectResult Errore(ServiceResult result) => result.StatusCode switch
    {
        404 => NotFound(result.ErrorMessage),
        409 => Conflict(result.ErrorMessage),
        500 => StatusCode(500, result.ErrorMessage),
        _ => BadRequest(result.ErrorMessage)
    };
}