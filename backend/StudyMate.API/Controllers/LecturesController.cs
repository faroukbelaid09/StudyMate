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

        if (req.File.Length == 0)
            return BadRequest("Empty file.");

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

        await req.File.CopyToAsync(stream);

        //////////////////////////////////////////////////
        // Save Database
        //////////////////////////////////////////////////

        var lecture = new Lecture
        {
            UserId = userId,
            Title = req.Title,
            FilePath = fileName,
            UploadedAt = DateTime.UtcNow
        };

        _db.Lectures.Add(lecture);

        await _db.SaveChangesAsync();

        return Ok("Lecture uploaded.");
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
}