using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StudyMate.API.Data;
using StudyMate.API.DTOs.Lectures;
using StudyMate.API.Models;
using Microsoft.EntityFrameworkCore;
using StudyMate.API.Interfaces;
using StudyMate.API.DTOs.Quiz;
using StudyMate.API.DTOs.Flashcards;
namespace StudyMate.API.Controllers;

[ApiController]
[Route("lectures")]
[Authorize]
public class LecturesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILectureService _lectureService;

    public LecturesController(
        AppDbContext db,
        ILectureService lectureService)
    {
        _db = db;
        _lectureService = lectureService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(
        [FromForm] UploadLectureRequest req)
    {
        var userId =
            int.Parse(User.FindFirstValue("uid")!);

        try
        {
            await _lectureService.UploadLectureAsync(
                userId,
                req.Title,
                req.File);

            return Ok("Lecture uploaded.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("mine")]
    public async Task<ActionResult<List<LectureResponse>>> GetMine()
    {
        var userId =
            int.Parse(User.FindFirstValue("uid")!);

        var lectures = await _db.Lectures
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.UploadedAt)
            .Select(l => new LectureResponse(
                l.Id,
                l.Title,
                l.UploadedAt,
                l.Status
            ))
            .ToListAsync();

        return Ok(lectures);
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var userId =
            int.Parse(User.FindFirstValue("uid")!);

        try
        {
            var (lecture, filePath) =
                await _lectureService
                    .GetLectureForDownloadAsync(id, userId);

            return PhysicalFile(
                filePath,
                "application/pdf",
                $"{lecture.Title}.pdf");
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId =
            int.Parse(User.FindFirstValue("uid")!);

        try
        {
            await _lectureService.DeleteLectureAsync(id, userId);

            return Ok("Lecture deleted.");
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LectureResponse>> GetLecture(int id)
    {
        var userId =
            int.Parse(User.FindFirstValue("uid")!);

        try
        {
            var lecture =
                await _lectureService.GetLectureAsync(id, userId);

            var response = new LectureResponse(
    lecture.Id,
    lecture.Title,
    lecture.UploadedAt,
    lecture.Status
);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{id}/extract-text")]
    public async Task<IActionResult> ExtractText(int id)
    {
        var userId =
            int.Parse(User.FindFirstValue("uid")!);

        try
        {
            await _lectureService.ExtractTextAsync(id, userId);

            return Ok("Text extracted successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/generate-summary")]
    public async Task<IActionResult> GenerateSummary(int id)
    {
        var userId =
            int.Parse(User.FindFirstValue("uid")!);

        try
        {
            await _lectureService.GenerateSummaryAsync(id, userId);

            return Ok("Summary generated.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/summary")]
    public async Task<ActionResult<LectureSummaryResponse>> GetSummary(int id)
    {
        var userId =
            int.Parse(User.FindFirstValue("uid")!);

        try
        {
            var summary =
                await _lectureService.GetSummaryAsync(id, userId);

            var response =
                new LectureSummaryResponse(id, summary);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/generate-flashcards")]
    public async Task<IActionResult> GenerateFlashcards(int id)
    {
        var userId =
            int.Parse(User.FindFirstValue("uid")!);

        try
        {
            await _lectureService.GenerateFlashcardsAsync(id, userId);

            return Ok("Flashcards generated.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/flashcards")]
    public async Task<ActionResult<List<FlashcardResponse>>> GetFlashcards(int id)
    {
        var userId =
            int.Parse(User.FindFirstValue("uid")!);

        try
        {
            var flashcards =
                await _lectureService.GetFlashcardsAsync(id, userId);

            var response =
                flashcards.Select(f =>
                    new FlashcardResponse(
                        f.Id,
                        f.Question,
                        f.Answer
                    )).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/generate-quiz")]
    public async Task<IActionResult> GenerateQuiz(int id)
    {
        var userId =
            int.Parse(User.FindFirstValue("uid")!);

        try
        {
            await _lectureService.GenerateQuizAsync(id, userId);

            return Ok("Quiz generated.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/quiz")]
    public async Task<ActionResult<List<QuizQuestionResponse>>> GetQuiz(int id)
    {
        var userId =
            int.Parse(User.FindFirstValue("uid")!);

        try
        {
            var quiz =
                await _lectureService.GetQuizAsync(id, userId);

            var response =
                quiz.Select(q =>
                    new QuizQuestionResponse(
                        q.Id,
                        q.Question,
                        q.OptionA,
                        q.OptionB,
                        q.OptionC,
                        q.OptionD,
                        q.CorrectAnswer
                    )).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/quiz/submit")]
    public async Task<ActionResult<QuizResultResponse>> SubmitQuiz(
    int id,
    SubmitQuizRequest req)
    {
        var userId =
            int.Parse(User.FindFirstValue("uid")!);

        try
        {
            var result =
                await _lectureService.SubmitQuizAsync(
                    id,
                    userId,
                    req.Answers);

            var response =
                new QuizResultResponse(
                    result.score,
                    result.total);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}