using StudyMate.API.Models;

namespace StudyMate.API.Interfaces;

public interface ILectureService
{
    Task<(Lecture lecture, string filePath)>
        GetLectureForDownloadAsync(int lectureId, int userId);
}