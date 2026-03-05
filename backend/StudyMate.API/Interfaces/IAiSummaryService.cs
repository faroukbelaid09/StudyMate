namespace StudyMate.API.Interfaces;

public interface IAiSummaryService
{
    Task<string> GenerateSummaryAsync(string text);
}