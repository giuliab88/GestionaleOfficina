using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficinaGestionale.Api.DTOs;
using OfficinaGestionale.Api.Services;

namespace OfficinaGestionale.Api.Controllers;

// gestione interventi (richiede autenticazione)
[ApiController]
[Route("api/interventi")]
[Authorize]
public class InterventoController : ControllerBase
{
    private readonly IService<InterventoDto> _service;

    public InterventoController(IService<InterventoDto> service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IEnumerable<InterventoDto>> ListaInterventi()
    {
        return Ok(_service.CercaTutti());
    }

    // cerca intervento per codice
    [HttpGet("{codice}")]
    public ActionResult<InterventoDto> CercaIntervento(string codice)
    {
        InterventoDto? risultato = _service.CercaPerCodice(codice);
        return risultato is not null ? Ok(risultato) : NotFound();
    }

    [HttpPost]
    public ActionResult<InterventoDto> InserisciIntervento([FromBody] InterventoDto intDto)
    {
        ServiceResult result = _service.Inserisci(intDto);
        if (!result.Success) return Errore(result);

        return CreatedAtAction(nameof(CercaIntervento), new { codice = intDto.Codice }, intDto);
    }

    [HttpPut("{codice}")]
    public IActionResult AggiornaIntervento(string codice, [FromBody] InterventoDto intDto)
    {
        // uso il codice della route per evitare mismatch con il body
        intDto.Codice = codice;

        ServiceResult result = _service.Aggiorna(intDto);
        if (!result.Success) return Errore(result);

        return NoContent();
    }

    [HttpDelete("{codice}")]
    public IActionResult EliminaIntervento(string codice)
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
