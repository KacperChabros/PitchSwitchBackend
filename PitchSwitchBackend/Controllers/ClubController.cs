using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PitchSwitchBackend.Dtos.Club.Requests;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Services.ClubService;

namespace PitchSwitchBackend.Controllers
{
    [Route("api/club")]
    [ApiController]
    public class ClubController : ControllerBase
    {
        private readonly IClubService _clubService;
        public ClubController(
            IClubService clubService)
        {
            _clubService = clubService;
        }

        [HttpPost("addclub")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddClub([FromBody] AddClubDto addClubDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newClub = await _clubService.AddClub(addClubDto);

            if (newClub == null)
            {
                return BadRequest(ModelState);
            }

            return CreatedAtAction(nameof(GetClub), new { clubId = newClub.ClubId }, newClub);
        }

        [HttpGet("getallclubs")]
        [Authorize]
        public async Task<IActionResult> GetAllClubs([FromQuery] ClubQueryObject clubQuery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var clubs = await _clubService.GetAllClubs(clubQuery);

            if (clubs == null)
            {
                return NotFound("There are no clubs");
            }

            return Ok(clubs);
        }

        [HttpGet("getclub/{clubId:int}")]
        [Authorize]
        public async Task<IActionResult> GetClub([FromRoute] int clubId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var club = await _clubService.GetClubById(clubId);

            if (club == null)
            {
                return NotFound("There is no such club");
            }

            return Ok(club.FromModelToClubDto());
        }

        [HttpPut("updateclub/{clubId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateClub([FromRoute] int clubId, [FromBody] UpdateClubDto updateClubDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var club = await _clubService.GetClubById(clubId);

            if (club == null)
            {
                return NotFound("There is no such club");
            }

            var updatedClub = await _clubService.UpdateClub(club, updateClubDto);

            return Ok(updatedClub);
        }

        [HttpPut("archiveclub/{clubId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArchiveClub([FromRoute] int clubId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _clubService.ArchiveClub(clubId);

            if (!result)
            {
                return NotFound("There is no such club");
            }

            return Ok();
        }

        [HttpPut("restoreclub/{clubId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreClub([FromRoute] int clubId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _clubService.RestoreClub(clubId);

            if (!result)
            {
                return NotFound("There is no such club");
            }

            return Ok();
        }
    }
}
