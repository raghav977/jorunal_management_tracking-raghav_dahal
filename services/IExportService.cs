using MauiApp3.Models;

namespace MauiApp3.Services;

public interface IExportService
{
    Task<string> ExportToPdfAsync(DateOnly startDate, DateOnly endDate);
    Task<string> ExportToTextAsync(DateOnly startDate, DateOnly endDate);
}
