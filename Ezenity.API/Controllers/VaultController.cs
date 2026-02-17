using Asp.Versioning;
using Ezenity.Domain.Entities.Vault;
using Ezenity.Contracts.Models;
using Ezenity.Contracts.Models.Vault;
using Ezenity.Infrastructure.Attributes;
using Ezenity.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ezenity.Contracts;

namespace Ezenity.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/vault")]
[ApiVersion("1.0")]
[Produces("application/vnd.api+json")]
public class VaultController : ControllerBase
{
    private readonly DataContext _db;

    public VaultController(DataContext db) => _db = db;

    // Rider: list missions
    [AuthorizeV2] // logged in
    [HttpGet("missions")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VaultMissionResponse>>>> GetMissions()
    {
        var accountId = ((Ezenity.Domain.Entities.Accounts.Account)HttpContext.Items["Account"]).Id;

        var missions = await _db.VaultMissions
            .Include(m => m.Emblem)
            .Where(m => m.IsActive)
            .OrderByDescending(m => m.CreatedUtc)
            .Select(m => new VaultMissionResponse
            {
                Id = m.Id,
                Slug = m.Slug,
                Title = m.Title,
                Summary = m.Summary,
                Description = m.Description,
                RepReward = m.RepReward,
                CoinReward = m.CoinReward,
                EmblemName = m.Emblem != null ? m.Emblem.Name : null,
                MyStatus = _db.VaultSubmissions
                    .Where(s => s.MissionId == m.Id && s.AccountId == accountId)
                    .OrderByDescending(s => s.SubmittedUtc)
                    .Select(s => s.Status.ToString())
                    .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(new ApiResponse<IEnumerable<VaultMissionResponse>>
        {
            StatusCode = 200,
            IsSuccess = true,
            Message = "Vault missions fetched successfully.",
            Data = missions
        });
    }

    // Rider: submit proof
    [AuthorizeV2]
    [HttpPost("missions/{missionId:int}/submissions")]
    public async Task<ActionResult<ApiResponse<object>>> Submit(int missionId, [FromBody] CreateVaultSubmissionRequest req)
    {
        var account = (Ezenity.Core.Entities.Accounts.Account)HttpContext.Items["Account"];

        var mission = await _db.VaultMissions.FirstOrDefaultAsync(x => x.Id == missionId && x.IsActive);
        if (mission == null)
            return NotFound(new ApiResponse<object> { StatusCode = 404, IsSuccess = false, Message = "Mission not found." });

        var submission = new VaultSubmission
        {
            MissionId = missionId,
            AccountId = account.Id,
            ProofYoutubeUrl = req.ProofYoutubeUrl,
            ProofInstagramUrl = req.ProofInstagramUrl,
            ProofTiktokUrl = req.ProofTiktokUrl,
            ProofFacebookUrl = req.ProofFacebookUrl,
            Notes = req.Notes,
            Status = VaultSubmissionStatus.Pending,
        };

        foreach (var fileId in req.FileIds.Distinct())
        {
            submission.Media.Add(new VaultMissionSubmissionMedia
            {
                FileId = fileId,
                MediaType = VaultMediaType.Image // you can infer from content-type later
            });
        }

        _db.VaultSubmissions.Add(submission);
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            StatusCode = 200,
            IsSuccess = true,
            Message = "Submission created (Pending review).",
            Data = new { submission.Id, submission.Status }
        });
    }

    // Admin: list pending submissions
    [AuthorizeV2("Admin")]
    [HttpGet("admin/submissions")]
    public async Task<ActionResult<ApiResponse<object>>> Pending()
    {
        var pending = await _db.VaultSubmissions
            .Include(s => s.Mission)
            .Where(s => s.Status == VaultSubmissionStatus.Pending)
            .OrderBy(s => s.SubmittedUtc)
            .Select(s => new
            {
                s.Id,
                s.MissionId,
                MissionTitle = s.Mission.Title,
                s.AccountId,
                s.SubmittedUtc,
                s.ProofYoutubeUrl,
                s.ProofInstagramUrl,
                s.ProofTiktokUrl,
                s.ProofFacebookUrl
            })
            .ToListAsync();

        return Ok(new ApiResponse<object>
        {
            StatusCode = 200,
            IsSuccess = true,
            Message = "Pending submissions fetched.",
            Data = pending
        });
    }

    // Admin: approve
    [AuthorizeV2("Admin")]
    [HttpPost("admin/submissions/{submissionId:int}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> Approve(int submissionId, [FromBody] string? note)
    {
        var admin = (Ezenity.Core.Entities.Accounts.Account)HttpContext.Items["Account"];

        var sub = await _db.VaultSubmissions
            .Include(s => s.Mission)
            .FirstOrDefaultAsync(s => s.Id == submissionId);

        if (sub == null)
            return NotFound(new ApiResponse<object> { StatusCode = 404, IsSuccess = false, Message = "Submission not found." });

        sub.Status = VaultSubmissionStatus.Approved;
        sub.ReviewedByAccountId = admin.Id;
        sub.ReviewedUtc = DateTime.UtcNow;
        sub.ReviewNote = note;

        // MVP: award Rep/Coins here (we can upgrade to a ledger later)
        // Example if a Account has Rep/Coin columns:
        // var rider = await _db.Accounts.FindAsync(sub.AccountId);
        // rider.Rep += sub.Mission.RepReward;
        // rider.Coins += sub.Mission.CoinReward;

        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            StatusCode = 200,
            IsSuccess = true,
            Message = "Submission approved.",
            Data = new { sub.Id, sub.Status }
        });
    }
}
