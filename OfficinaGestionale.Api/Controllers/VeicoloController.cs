using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficinaGestionale.Api.DTOs;
using OfficinaGestionale.Api.Services;

namespace OfficinaGestionale.Api.Controllers;

// gestione veicoli (richiede autenticazione)
[ApiController]
[Route("api/veicoli")]
[Authorize]
public class VeicoloController : ControllerBase
{
    private readonly IService<VeicoloDto> _service;

    public VeicoloController(IService<VeicoloDto> service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IEnumerable<VeicoloDto>> ListaVeicoli()
    {
        return Ok(_service.CercaTutti());
    }

    [HttpGet("{codice}")]
    public ActionResult<VeicoloDto> VisualizzaVeicolo(string codice)
    {
        VeicoloDto? risultato = _service.CercaPerCodice(codice);
        return risultato is not null ? Ok(risultato) : NotFound();
    }

    [HttpPost]
    public ActionResult<VeicoloDto> InserisciVeicolo([FromBody] VeicoloDto veicoloDto)
    {
        ServiceResult result = _service.Inserisci(veicoloDto);
        if (!result.Success) return Errore(result);

        return CreatedAtAction(nameof(VisualizzaVeicolo), new { codice = veicoloDto.Codice }, veicoloDto);
    }

    [HttpPut("{codice}")]
    public IActionResult AggiornaVeicolo(string codice, [FromBody] VeicoloDto veicoloDto)
    {
        // uso il codice della route per evitare mismatch con il body
        veicoloDto.Codice = codice;

        ServiceResult result = _service.Aggiorna(veicoloDto);
        if (!result.Success) return Errore(result);

        return NoContent();
    }

    [HttpDelete("{codice}")]
    public IActionResult EliminaVeicolo(string codice)
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