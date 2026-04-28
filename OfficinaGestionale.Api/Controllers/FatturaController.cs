using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficinaGestionale.Api.DTOs;
using OfficinaGestionale.Api.Services;

namespace OfficinaGestionale.Api.Controllers;

// controller fatture: CRUD + creazione da preventivo
[ApiController]
[Route("api/fatture")]
[Authorize]
public class FatturaController : ControllerBase
{
    private readonly IService<FatturaDto> _service;
    private readonly FatturaService _fatturaService;

    public FatturaController(IService<FatturaDto> service, FatturaService fatturaService)
    {
        _service = service;
        _fatturaService = fatturaService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<FatturaDto>> Lista()
    {
        return Ok(_service.CercaTutti());
    }

    [HttpGet("{codice}")]
    public ActionResult<FatturaDto> Cerca(string codice)
    {
        FatturaDto? risultato = _service.CercaPerCodice(codice);
        return risultato is not null ? Ok(risultato) : NotFound();
    }

    [HttpPost]
    public ActionResult<FatturaDto> Inserisci([FromBody] FatturaDto dto)
    {
        ServiceResult result = _service.Inserisci(dto);
        if (!result.Success) return Errore(result);

        return CreatedAtAction(nameof(Cerca), new { codice = dto.Codice }, dto);
    }

    // endpoint dedicato alla conversione preventivo -> fattura
    [HttpPost("da-preventivo/{codicePreventivo}")]
    public ActionResult<FatturaDto> DaPreventivo(string codicePreventivo)
    {
        ServiceResult<FatturaDto> result = _fatturaService.ConvertitoDaPreventivo(codicePreventivo);
        if (!result.Success) return Errore(result);

        return CreatedAtAction(nameof(Cerca), new { codice = result.Data!.Codice }, result.Data);
    }

    [HttpPut("{codice}")]
    public IActionResult Aggiorna(string codice, [FromBody] FatturaDto dto)
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