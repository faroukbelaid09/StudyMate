using StudyMate.API.Models;

namespace StudyMate.API.DTOs.Lectures;

public record LectureResponse(
    int Id,
    string Title,
    DateTime UploadedAt,
    LectureStatus Status
);