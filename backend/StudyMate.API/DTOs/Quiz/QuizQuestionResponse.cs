namespace StudyMate.API.DTOs.Quiz;

public record QuizQuestionResponse(
    int Id,
    string Question,
    string OptionA,
    string OptionB,
    string OptionC,
    string OptionD,
    string CorrectAnswer
);