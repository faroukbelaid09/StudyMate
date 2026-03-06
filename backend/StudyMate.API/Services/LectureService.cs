using Microsoft.EntityFrameworkCore;
using StudyMate.API.Data;
using StudyMate.API.Interfaces;
using StudyMate.API.Models;
using UglyToad.PdfPig;
using System.Text;

namespace StudyMate.API.Services;

public class LectureService : ILectureService
{
    private readonly AppDbContext _db;
    private readonly IAiSummaryService _aiSummaryService;

    public LectureService(
        AppDbContext db,
        IAiSummaryService aiSummaryService)
    {
        _db = db;
        _aiSummaryService = aiSummaryService;
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
            UploadedAt = DateTime.UtcNow,
            Status = LectureStatus.Uploaded
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

    public async Task DeleteLectureAsync(int lectureId, int userId)
    {
        var lecture = await _db.Lectures
            .FirstOrDefaultAsync(l =>
                l.Id == lectureId &&
                l.UserId == userId);

        if (lecture is null)
            throw new Exception("Lecture not found.");

        ////////////////////////////////////////////////
        // Delete file from disk
        ////////////////////////////////////////////////

        var uploadsFolder =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "uploads");

        var filePath =
            Path.Combine(uploadsFolder, lecture.FilePath);

        if (File.Exists(filePath))
            File.Delete(filePath);

        ////////////////////////////////////////////////
        // Remove from database
        ////////////////////////////////////////////////

        _db.Lectures.Remove(lecture);

        await _db.SaveChangesAsync();
    }

    public async Task<Lecture> GetLectureAsync(int lectureId, int userId)
    {
        var lecture = await _db.Lectures
            .FirstOrDefaultAsync(l =>
                l.Id == lectureId &&
                l.UserId == userId);

        if (lecture is null)
            throw new Exception("Lecture not found.");

        return lecture;
    }

    public async Task ExtractTextAsync(int lectureId, int userId)
    {
        //////////////////////////////////////////////////
        // Find lecture
        //////////////////////////////////////////////////

        var lecture = await _db.Lectures
            .FirstOrDefaultAsync(l =>
                l.Id == lectureId &&
                l.UserId == userId);

        if (lecture is null)
            throw new Exception("Lecture not found.");

        //////////////////////////////////////////////////
        // Build file path
        //////////////////////////////////////////////////

        var uploadsFolder =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "uploads");

        var filePath =
            Path.Combine(uploadsFolder, lecture.FilePath);

        if (!File.Exists(filePath))
            throw new Exception("PDF file not found.");

        //////////////////////////////////////////////////
        // Extract PDF text
        //////////////////////////////////////////////////

        var textBuilder = new StringBuilder();

        using (var document = PdfDocument.Open(filePath))
        {
            foreach (var page in document.GetPages())
            {
                textBuilder.AppendLine(page.Text);
            }
        }

        var extractedText = textBuilder.ToString();

        //////////////////////////////////////////////////
        // Save extracted text
        //////////////////////////////////////////////////

        lecture.ExtractedText = extractedText;

        lecture.Status = LectureStatus.TextExtracted;

        await _db.SaveChangesAsync();
    }

    public async Task GenerateSummaryAsync(int lectureId, int userId)
    {
        var lecture = await _db.Lectures
            .FirstOrDefaultAsync(l =>
                l.Id == lectureId &&
                l.UserId == userId);

        if (lecture is null)
            throw new Exception("Lecture not found.");

        if (lecture.ExtractedText is null)
            throw new Exception("Extract text first.");

        //////////////////////////////////////////////////
        // TEMPORARY AI PLACEHOLDER
        //////////////////////////////////////////////////

        var summary =
            $"[AI Placeholder]\n\n" +
            $"This is a temporary summary for lecture: {lecture.Title}.\n\n" +
            $"Text length: {lecture.ExtractedText.Length} characters.\n\n" +
            $"Later this will be generated using an AI model.";

        lecture.Summary = summary;

        lecture.Status = LectureStatus.SummaryGenerated;

        await _db.SaveChangesAsync();
    }

    public async Task<string> GetSummaryAsync(int lectureId, int userId)
    {
        var lecture = await _db.Lectures
            .FirstOrDefaultAsync(l =>
                l.Id == lectureId &&
                l.UserId == userId);

        if (lecture is null)
            throw new Exception("Lecture not found.");

        if (lecture.Summary is null)
            throw new Exception("Summary not generated yet.");

        return lecture.Summary;
    }

    public async Task GenerateFlashcardsAsync(int lectureId, int userId)
    {
        var lecture = await _db.Lectures
            .FirstOrDefaultAsync(l =>
                l.Id == lectureId &&
                l.UserId == userId);

        if (lecture is null)
            throw new Exception("Lecture not found.");

        if (lecture.ExtractedText is null)
            throw new Exception("Extract text first.");

        //////////////////////////////////////////////////
        // TEMPORARY FLASHCARD PLACEHOLDER
        //////////////////////////////////////////////////

        var flashcards = new List<Flashcard>
    {
        new Flashcard
        {
            LectureId = lectureId,
            Question = "What is the main topic of this lecture?",
            Answer = lecture.Title
        },
        new Flashcard
        {
            LectureId = lectureId,
            Question = "How many characters were extracted?",
            Answer = lecture.ExtractedText.Length.ToString()
        }
    };

        _db.Flashcards.AddRange(flashcards);

        await _db.SaveChangesAsync();
    }

    public async Task<List<Flashcard>> GetFlashcardsAsync(int lectureId, int userId)
    {
        var lecture = await _db.Lectures
            .FirstOrDefaultAsync(l =>
                l.Id == lectureId &&
                l.UserId == userId);

        if (lecture is null)
            throw new Exception("Lecture not found.");

        return await _db.Flashcards
            .Where(f => f.LectureId == lectureId)
            .ToListAsync();
    }

    public async Task GenerateQuizAsync(int lectureId, int userId)
    {
        var lecture = await _db.Lectures
            .FirstOrDefaultAsync(l =>
                l.Id == lectureId &&
                l.UserId == userId);

        if (lecture is null)
            throw new Exception("Lecture not found.");

        if (lecture.ExtractedText is null)
            throw new Exception("Extract text first.");

        //////////////////////////////////////////////////
        // TEMPORARY QUIZ PLACEHOLDER
        //////////////////////////////////////////////////

        var quiz = new List<QuizQuestion>
    {
        new QuizQuestion
        {
            LectureId = lectureId,
            Question = "What is the title of this lecture?",
            OptionA = lecture.Title,
            OptionB = "Biology",
            OptionC = "Chemistry",
            OptionD = "Physics",
            CorrectAnswer = "A"
        },
        new QuizQuestion
        {
            LectureId = lectureId,
            Question = "What was extracted from the PDF?",
            OptionA = "Images",
            OptionB = "Text",
            OptionC = "Audio",
            OptionD = "Video",
            CorrectAnswer = "B"
        }
    };

        _db.QuizQuestions.AddRange(quiz);

        await _db.SaveChangesAsync();
    }

    public async Task<List<QuizQuestion>> GetQuizAsync(int lectureId, int userId)
    {
        var lecture = await _db.Lectures
            .FirstOrDefaultAsync(l =>
                l.Id == lectureId &&
                l.UserId == userId);

        if (lecture is null)
            throw new Exception("Lecture not found.");

        return await _db.QuizQuestions
            .Where(q => q.LectureId == lectureId)
            .ToListAsync();
    }
}