namespace StudyMate.API.DTOs.Quiz;

public class SubmitQuizRequest
{
    public Dictionary<int, string> Answers { get; set; } = new();
}