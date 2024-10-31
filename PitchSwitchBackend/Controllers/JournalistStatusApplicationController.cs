using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PitchSwitchBackend.Dtos.JournalistStatusApplication.Requests;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Services.JournalistStatusApplicationService;
using System.Security.Claims;

namespace PitchSwitchBackend.Controllers
{
    [Route("api/journaliststatusapplications")]
    [ApiController]
    public class JournalistStatusApplicationController : ControllerBase
    {
        private readonly IJournalistStatusApplicationService _jsaService;
        public JournalistStatusApplicationController(IJournalistStatusApplicationService jsaService)
        {
            _jsaService = jsaService;
        }

        [HttpPost("addapplication")]
        [Authorize]
        public async Task<IActionResult> AddApplication([FromBody] AddJournalistStatusApplicationDto addJournalistStatusApplicationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.IsInRole("Journalist") || User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            if (await _jsaService.CheckIfUserHasOpenApplication(userId))
            {
                return BadRequest("There is an open application already!");
            }

            var newApplication = await _jsaService.AddApplication(addJournalistStatusApplicationDto, userId);

            return CreatedAtAction(nameof(GetApplication), new { applicationId = newApplication.Id }, newApplication);
        }

        [HttpGet("getallapplications")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllApplications([FromQuery] JournalistStatusApplicationQueryObject jsaQuery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var applications = await _jsaService.GetAllApplications(jsaQuery);

            if (applications == null || applications.Count == 0)
            {
                return NotFound("There are no applications matching the criteria");
            }

            return Ok(applications);
        }

        [HttpGet("getapplication/{applicationId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetApplication([FromRoute] int applicationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var application = await _jsaService.GetApplicationByIdWithData(applicationId);

            if (application == null)
            {
                return NotFound("There is no such application");
            }

            return Ok(application.FromModelToJournalistStatusApplicationDto());
        }

        [HttpPut("updateapplication/{applicationId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateApplication([FromRoute] int applicationId, [FromBody] UpdateJournalistStatusApplicationDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var application = await _jsaService.GetApplicationById(applicationId);

            if (application == null)
            {
                return NotFound("There is no such application");
            }

            if (!application.SubmittedByUserId.Equals(userId))
            {
                return Forbid();
            }

            var updatedApplication = await _jsaService.UpdateApplication(application, updateDto);

            return Ok(updatedApplication);
        }

        [HttpPut("reviewapplications/{applicationId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReviewApplication([FromRoute] int applicationId, [FromBody] ReviewJournalistStatusApplicationDto reviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var application = await _jsaService.GetApplicationByIdWithData(applicationId);

            if (application == null)
            {
                return NotFound("There is no such application");
            }

            var reviewedApplication = await _jsaService.ReviewApplication(application, reviewDto);

            return Ok(reviewedApplication);
        }

        [HttpDelete("deleteapplication/{applicationId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteApplication([FromRoute] int applicationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var application = await _jsaService.GetApplicationById(applicationId);
            if (application == null)
            {
                return NotFound("There is no such application");
            }

            await _jsaService.DeleteApplication(application);
            return NoContent();
        }

    }
}
