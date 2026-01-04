using System.Text;
using MauiApp3.Models;

namespace MauiApp3.Services;

public class ExportService : IExportService
{
    private readonly IJournalService _journalService;

    public ExportService(IJournalService journalService)
    {
        _journalService = journalService;
    }

    public async Task<string> ExportToPdfAsync(DateOnly startDate, DateOnly endDate)
    {
        // For now, export as HTML that can be saved/printed as PDF
        var entries = await _journalService.GetEntriesByDateRangeAsync(startDate, endDate);
        
        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html><head>");
        html.AppendLine("<meta charset='UTF-8'>");
        html.AppendLine("<title>Journal Export</title>");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: sans-serif; max-width: 800px; margin: 0 auto; padding: 20px; }");
        html.AppendLine(".entry { border-bottom: 1px solid #ccc; padding: 20px 0; }");
        html.AppendLine(".entry-header { display: flex; justify-content: space-between; align-items: center; }");
        html.AppendLine(".entry-date { color: #666; font-size: 0.9em; }");
        html.AppendLine(".entry-mood { font-size: 1.5em; }");
        html.AppendLine(".entry-title { font-size: 1.2em; font-weight: bold; margin: 10px 0; }");
        html.AppendLine(".entry-content { line-height: 1.6; white-space: pre-wrap; }");
        html.AppendLine(".tags { margin-top: 10px; }");
        html.AppendLine(".tag { background: #eee; padding: 2px 8px; border-radius: 4px; font-size: 0.8em; margin-right: 5px; }");
        html.AppendLine("</style>");
        html.AppendLine("</head><body>");
        html.AppendLine($"<h1>Journal Entries</h1>");
        html.AppendLine($"<p>{startDate:MMM dd, yyyy} - {endDate:MMM dd, yyyy}</p>");

        foreach (var entry in entries.OrderBy(e => e.EntryDate))
        {
            html.AppendLine("<div class='entry'>");
            html.AppendLine("<div class='entry-header'>");
            html.AppendLine($"<span class='entry-date'>{entry.EntryDate:dddd, MMMM dd, yyyy}</span>");
            html.AppendLine($"<span class='entry-mood'>{GetMoodEmoji(entry.Mood)}</span>");
            html.AppendLine("</div>");
            html.AppendLine($"<div class='entry-title'>{System.Net.WebUtility.HtmlEncode(entry.Title)}</div>");
            html.AppendLine($"<div class='entry-content'>{System.Net.WebUtility.HtmlEncode(entry.Content)}</div>");
            
            if (entry.JournalEntryTags?.Any() == true)
            {
                html.AppendLine("<div class='tags'>");
                foreach (var jt in entry.JournalEntryTags)
                {
                    html.AppendLine($"<span class='tag'>{System.Net.WebUtility.HtmlEncode(jt.Tag?.Name ?? "")}</span>");
                }
                html.AppendLine("</div>");
            }
            html.AppendLine("</div>");
        }

        html.AppendLine("</body></html>");

        // Save to file
        var fileName = $"journal_export_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.html";
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName);
        await File.WriteAllTextAsync(filePath, html.ToString());

        return filePath;
    }

    public async Task<string> ExportToTextAsync(DateOnly startDate, DateOnly endDate)
    {
        var entries = await _journalService.GetEntriesByDateRangeAsync(startDate, endDate);
        
        var text = new StringBuilder();
        text.AppendLine("JOURNAL ENTRIES");
        text.AppendLine($"{startDate:MMM dd, yyyy} - {endDate:MMM dd, yyyy}");
        text.AppendLine(new string('=', 50));
        text.AppendLine();

        foreach (var entry in entries.OrderBy(e => e.EntryDate))
        {
            text.AppendLine($"Date: {entry.EntryDate:dddd, MMMM dd, yyyy}");
            text.AppendLine($"Mood: {GetMoodEmoji(entry.Mood)} {entry.Mood}");
            text.AppendLine($"Title: {entry.Title}");
            text.AppendLine();
            text.AppendLine(entry.Content);
            text.AppendLine();
            text.AppendLine(new string('-', 50));
            text.AppendLine();
        }

        // Save to file
        var fileName = $"journal_export_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.txt";
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName);
        await File.WriteAllTextAsync(filePath, text.ToString());

        return filePath;
    }

    private static string GetMoodEmoji(MoodType mood) => mood switch
    {
        MoodType.Happy => "😊",
        MoodType.Sad => "😢",
        MoodType.Neutral => "😐",
        MoodType.Anxious => "😰",
        MoodType.Excited => "🤩",
        MoodType.Grateful => "🙏",
        MoodType.Stressed => "😫",
        _ => "😐"
    };
}
