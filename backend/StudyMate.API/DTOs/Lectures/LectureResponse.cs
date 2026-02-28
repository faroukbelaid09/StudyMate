namespace StudyMate.API.DTOs.Lectures;

public record LectureResponse(
    int Id,
    string Title,
    DateTime UploadedAt
);