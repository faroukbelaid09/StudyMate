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

    public async Task UploadLectureAsync(int userId, string title, IFormFile file)
    {
        if (file.Length == 0)
            throw new Exception("Empty file.");

        //////////////////////////////////////////////////
        // Save File
        //////////////////////////////////////////////////

        var uploadsFolder =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "uploads");

        Directory.CreateDirectory(uploadsFolder);

        var fileName =
            $"{Guid.NewGuid()}.pdf";

        var filePath =
            Path.Combine(uploadsFolder, fileName);

        using var stream =
            System.IO.File.Create(filePath);

        await file.CopyToAsync(stream);

        //////////////////////////////////////////////////
        // Save Database
        //////////////////////////////////////////////////

        var lecture = new Lecture
        {
            UserId = userId,
            Title = title,
            FilePath = fileName,
            UploadedAt = DateTime.UtcNow
        };

        _db.Lectures.Add(lecture);

        await _db.SaveChangesAsync();
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