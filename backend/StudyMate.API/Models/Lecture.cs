namespace StudyMate.API.Models;

public class Lecture
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = default!;

    public string FilePath { get; set; } = default!;

    public DateTime UploadedAt { get; set; }
}