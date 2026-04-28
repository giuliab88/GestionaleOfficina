using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficinaGestionale.Api.DTOs;
using OfficinaGestionale.Api.Services;

namespace OfficinaGestionale.Api.Controllers;

// gestione preventivi (richiede autenticazione)
[ApiController]
[Route("api/preventivi")]
[Authorize]
public class PreventivoController : ControllerBase
{
    private readonly IService<PreventivoDto> _service;

    public PreventivoController(IService<PreventivoDto> service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PreventivoDto>> Lista()
    {
        return Ok(_service.CercaTutti());
    }

    [HttpGet("{codice}")]
    public ActionResult<PreventivoDto> Cerca(string codice)
    {
        PreventivoDto? risultato = _service.CercaPerCodice(codice);
        return risultato is not null ? Ok(risultato) : NotFound();
    }

    [HttpPost]
    public ActionResult<PreventivoDto> Inserisci([FromBody] PreventivoDto dto)
    {
        ServiceResult result = _service.Inserisci(dto);
        if (!result.Success) return Errore(result);

        return CreatedAtAction(nameof(Cerca), new { codice = dto.Codice }, dto);
    }

    [HttpPut("{codice}")]
    public IActionResult Aggiorna(string codice, [FromBody] PreventivoDto dto)
    {
        // uso il codice della route per evitare mismatch con il body
        dto.Codice = codice;

        ServiceResult result = _service.Aggiorna(dto);
        if (!result.Success) return Errore(result);

        return NoContent();
    }

    [HttpDelete("{codice}")]
    public IActionResult Elimina(string codice)
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