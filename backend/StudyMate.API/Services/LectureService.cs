using Microsoft.EntityFrameworkCore;
using StudyMate.API.Data;
using StudyMate.API.Interfaces;
using StudyMate.API.Models;

namespace StudyMate.API.Services;

public class LectureService : ILectureService
{
    private readonly AppDbContext _db;

    public LectureService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(Lecture lecture, string filePath)>
        GetLectureForDownloadAsync(int lectureId, int userId)
    {
        var lecture = await _db.Lectures
            .FirstOrDefaultAsync(l =>
                l.Id == lectureId &&
                l.UserId == userId);

        if (lecture is null)
            throw new Exception("Lecture not found.");

        var uploadsFolder =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "uploads");

        var filePath =
            Path.Combine(uploadsFolder, lecture.FilePath);

        if (!File.Exists(filePath))
            throw new Exception("File missing on server.");

        return (lecture, filePath);
    }
}