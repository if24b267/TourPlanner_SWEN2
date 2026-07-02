using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourPlanner.BL.Interfaces;
using TourPlanner.BL.Models;

namespace TourPlanner.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImportExportController : ControllerBase
{
    private readonly IImportExportService _importExportService;

    public ImportExportController(IImportExportService importExportService)
    {
        _importExportService = importExportService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var dto = await _importExportService.ExportAsync(CurrentUserId);
        var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
        var bytes = Encoding.UTF8.GetBytes(json);

        return File(bytes, "application/json", $"tourplanner-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json");
    }

    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Keine Datei uebermittelt");

        TourExportDto? dto;
        try
        {
            await using var stream = file.OpenReadStream();
            dto = await JsonSerializer.DeserializeAsync<TourExportDto>(stream);
        }
        catch (JsonException)
        {
            return BadRequest("Ungueltiges JSON-Format");
        }

        if (dto == null || dto.Tours == null)
            return BadRequest("Datei enthaelt keine gueltigen Tour-Daten");

        var count = await _importExportService.ImportAsync(CurrentUserId, dto);
        return Ok(new { importedCount = count });
    }
}