namespace StudyMate.API.Models;

public class Flashcard
{
    public int Id { get; set; }

    public int LectureId { get; set; }

    public string Question { get; set; } = default!;

    public string Answer { get; set; } = default!;

    public Lecture Lecture { get; set; } = default!;
}