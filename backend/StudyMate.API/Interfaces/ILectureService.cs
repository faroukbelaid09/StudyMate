using StudyMate.API.Models;

namespace StudyMate.API.Interfaces;

public interface ILectureService
{
    Task<(Lecture lecture, string filePath)>
        GetLectureForDownloadAsync(int lectureId, int userId);

    Task UploadLectureAsync(int userId, string title, IFormFile file);

    Task DeleteLectureAsync(int lectureId, int userId);

    Task<Lecture> GetLectureAsync(int lectureId, int userId);
}