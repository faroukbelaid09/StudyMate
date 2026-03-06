namespace StudyMate.API.DTOs.Flashcards;

public record FlashcardResponse(
    int Id,
    string Question,
    string Answer
);