using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PitchSwitchBackend.Dtos.Transfer.Requests;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Services.TransferService;

namespace PitchSwitchBackend.Controllers
{
    [Route("api/transfers")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly ITransferService _transferService;
        public TransferController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        [HttpPost("addtransfer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTransfer([FromBody] AddTransferDto addTransferDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newTransfer = await _transferService.AddTransfer(addTransferDto);

            if (newTransfer == null)
            {
                return BadRequest(ModelState);
            }

            return CreatedAtAction(nameof(GetTransfer), new { transferId = newTransfer.TransferId }, newTransfer);
        }

        [HttpGet("gettransfers")]
        [Authorize]
        public async Task<IActionResult> GetTransfers([FromQuery] TransferQueryObject transferQuery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transfers = await _transferService.GetTransfers(transferQuery);

            if (transfers == null || transfers.Count == 0)
            {
                return NotFound("There are no transfers matching the criteria");
            }

            return Ok(transfers);
        }

        [HttpGet("gettransfer/{transferId:int}")]
        [Authorize]
        public async Task<IActionResult> GetTransfer([FromRoute] int transferId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transfer = await _transferService.GetTransferWithDataById(transferId);

            if (transfer == null)
            {
                return NotFound("There is no such transfer");
            }

            return Ok(transfer.FromModelToTransferDto());
        }

        [HttpPut("updatetransfer/{transferId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTransfer([FromRoute] int transferId, [FromBody] UpdateTransferDto updateTransferDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transfer = await _transferService.GetTransferById(transferId);
            if (transfer == null)
            {
                return NotFound("There is no such transfer");
            }

            var updatedTransfer = await _transferService.UpdateTransfer(transfer, updateTransferDto);

            return Ok(updatedTransfer);
        }


        [HttpDelete("deletetransfer/{transferId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTransfer([FromRoute] int transferId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transfer = await _transferService.GetTransferWithDataById(transferId);

            if (transfer == null)
            {
                return NotFound("There is no such transfer");
            }

            await _transferService.DeleteTransfer(transfer);

            return NoContent();
        }
    }
}
