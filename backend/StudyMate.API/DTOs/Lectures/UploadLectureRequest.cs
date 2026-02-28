using Microsoft.AspNetCore.Http;

namespace StudyMate.API.DTOs.Lectures;

public record UploadLectureRequest(
    string Title,
    IFormFile File
);