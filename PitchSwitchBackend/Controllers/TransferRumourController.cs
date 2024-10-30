using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PitchSwitchBackend.Dtos.TransferRumour.Requests;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Services.TransferRumourService;
using System.Security.Claims;

namespace PitchSwitchBackend.Controllers
{
    [Route("api/transferrumours")]
    [ApiController]
    public class TransferRumourController : ControllerBase
    {
        private readonly ITransferRumourService _transferRumourService;
        public TransferRumourController(ITransferRumourService transferRumourService)
        {
            _transferRumourService = transferRumourService;
        }

        [HttpPost("addtransferrumour")]
        [Authorize(Roles = "Admin, Journalist")]
        public async Task<IActionResult> AddTransfer([FromBody] AddTransferRumourDto addTransferRumourDto)
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

            var newTransferRumour = await _transferRumourService.AddTransferRumour(addTransferRumourDto, userId);

            if (newTransferRumour == null)
            {
                return BadRequest(ModelState);
            }

            return CreatedAtAction(nameof(GetTransferRumour), new { transferRumourId = newTransferRumour.TransferRumourId }, newTransferRumour);
        }

        [HttpGet("gettransferrumours")]
        [Authorize]
        public async Task<IActionResult> GetTransferRumours([FromQuery] TransferRumourQueryObject transferRumourQuery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transfersRumours = await _transferRumourService.GetTransferRumours(transferRumourQuery);

            if (transfersRumours == null || transfersRumours.Count == 0)
            {
                return NotFound("There are no transfer rumours matching the criteria");
            }

            return Ok(transfersRumours);
        }

        [HttpGet("gettransferrumour/{transferRumourId:int}")]
        [Authorize]
        public async Task<IActionResult> GetTransferRumour([FromRoute] int transferRumourId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transferRumour = await _transferRumourService.GetTransferRumourWithDataById(transferRumourId);

            if (transferRumour == null)
            {
                return NotFound("There is no such transfer rumour");
            }

            return Ok(transferRumour.FromModelToTransferRumourDto());
        }

        [HttpPut("updatetransferrumour/{transferRumourId:int}")]
        [Authorize(Roles ="Admin, Journalist")]
        public async Task<IActionResult> UpdateTransferRumour([FromRoute] int transferRumourId, [FromBody] UpdateTransferRumourDto updateTransferRumourDto)
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

            var transferRumour = await _transferRumourService.GetTransferRumourById(transferRumourId);
            if (transferRumour == null)
            {
                return NotFound("There is no such transfer rumour");
            }
            if (!transferRumour.CreatedByUserId.Equals(userId) && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var updatedTransferRumour = await _transferRumourService.UpdateTransferRumour(transferRumour, updateTransferRumourDto);

            return Ok(updatedTransferRumour);
        }


        [HttpPut("archivetransferrumour/{transferRumourId:int}")]
        [Authorize(Roles = "Admin, Journalist")]
        public async Task<IActionResult> ArchiveTransferRumour([FromRoute] int transferRumourId, [FromQuery] bool isConfirmed = false)
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
            var transferRumour = await _transferRumourService.GetTransferRumourById(transferRumourId);
            if (transferRumour == null)
            {
                return NotFound("There is no such transfer rumour");
            }

            if (!transferRumour.CreatedByUserId.Equals(userId) && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            var result = await _transferRumourService.ArchiveTransferRumour(transferRumour, isConfirmed);

            if (!result)
            {
                return NotFound("There is no such transfer rumour");
            }

            return Ok();
        }

        [HttpDelete("deletetransferrumour/{transferRumourId:int}")]
        [Authorize(Roles = "Admin, Journalist")]
        public async Task<IActionResult> DeleteTransferRumour([FromRoute] int transferRumourId)
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

            var transferRumour = await _transferRumourService.GetTransferRumourWithDataById(transferRumourId);

            if (transferRumour == null)
            {
                return NotFound("There is no such transfer rumour");
            }

            if (!transferRumour.CreatedByUserId.Equals(userId) && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            await _transferRumourService.DeleteTransferRumour(transferRumour);

            return NoContent();
        }
    }
}
