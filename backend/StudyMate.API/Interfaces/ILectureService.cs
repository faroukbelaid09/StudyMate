using StudyMate.API.Models;

namespace StudyMate.API.Interfaces;

public interface ILectureService
{
    Task<(Lecture lecture, string filePath)>
        GetLectureForDownloadAsync(int lectureId, int userId);

    Task UploadLectureAsync(int userId, string title, IFormFile file);

    Task DeleteLectureAsync(int lectureId, int userId);

    Task<Lecture> GetLectureAsync(int lectureId, int userId);

    Task ExtractTextAsync(int lectureId, int userId);

    Task GenerateSummaryAsync(int lectureId, int userId);

    Task<string> GetSummaryAsync(int lectureId, int userId);

    Task GenerateFlashcardsAsync(int lectureId, int userId);

    Task<List<Flashcard>> GetFlashcardsAsync(int lectureId, int userId);

    Task GenerateQuizAsync(int lectureId, int userId);

    Task<List<QuizQuestion>> GetQuizAsync(int lectureId, int userId);

    Task<(int score, int total)> SubmitQuizAsync(int lectureId, int userId, Dictionary<int, string> answers);
    

}