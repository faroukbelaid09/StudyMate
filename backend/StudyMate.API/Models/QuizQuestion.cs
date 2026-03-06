namespace StudyMate.API.Models;

public class QuizQuestion
{
    public int Id { get; set; }

    public int LectureId { get; set; }

    public string Question { get; set; } = default!;

    public string OptionA { get; set; } = default!;

    public string OptionB { get; set; } = default!;

    public string OptionC { get; set; } = default!;

    public string OptionD { get; set; } = default!;

    public string CorrectAnswer { get; set; } = default!;

    public Lecture Lecture { get; set; } = default!;
}