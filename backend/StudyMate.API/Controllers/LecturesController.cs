using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StudyMate.API.Data;
using StudyMate.API.DTOs.Lectures;
using StudyMate.API.Models;
using Microsoft.EntityFrameworkCore;
using StudyMate.API.Interfaces;
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
                l.UploadedAt
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
                lecture.UploadedAt
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}