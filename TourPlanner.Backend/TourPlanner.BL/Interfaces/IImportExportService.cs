using TourPlanner.BL.Models;

namespace TourPlanner.BL.Interfaces;

public interface IImportExportService
{
    Task<TourExportDto> ExportAsync(Guid userId);
    Task<int> ImportAsync(Guid userId, TourExportDto data);
}