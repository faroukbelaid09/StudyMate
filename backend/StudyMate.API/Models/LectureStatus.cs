namespace StudyMate.API.Models;

public enum LectureStatus
{
    Uploaded = 0,
    TextExtracted = 1,
    SummaryGenerated = 2,
    FlashcardsGenerated = 3,
    QuizGenerated = 4,
    Failed = 5
}