using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PitchSwitchBackend.Dtos.Player.Requests;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Services.PlayerService;

namespace PitchSwitchBackend.Controllers
{
    [Route("api/players")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpPost("addplayer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPlayer([FromBody] AddPlayerDto addPlayerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newPlayer = await _playerService.AddPlayer(addPlayerDto);

            if (newPlayer == null)
            {
                return BadRequest(ModelState);
            }

            return CreatedAtAction(nameof(GetPlayer), new { playerId = newPlayer.PlayerId }, newPlayer);
        }

        [HttpGet("getplayers")]
        [Authorize]
        public async Task<IActionResult> GetPlayers([FromQuery] PlayerQueryObject playerQuery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var clubs = await _playerService.GetPlayers(playerQuery);

            if (clubs == null)
            {
                return NotFound("There are no players");
            }

            return Ok(clubs);
        }

        [HttpGet("getplayer/{playerId:int}")]
        [Authorize]
        public async Task<IActionResult> GetPlayer([FromRoute] int playerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var player = await _playerService.GetPlayerWithClubById(playerId);

            if (player == null)
            {
                return NotFound("There is no such player");
            }

            return Ok(player.FromModelToPlayerDto());
        }

        [HttpPut("updateplayer/{playerId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePlayer([FromRoute] int playerId, [FromBody] UpdatePlayerDto updatePlayerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var player = await _playerService.GetPlayerById(playerId);

            if (player == null)
            {
                return NotFound("There is no such player");
            }

            var updatedPlayer = await _playerService.UpdatePlayer(player, updatePlayerDto);

            return Ok(updatedPlayer);
        }

        [HttpDelete("deleteplayer/{playerId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePlayer([FromRoute] int playerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var player = await _playerService.GetPlayerById(playerId);

            if (player == null)
            {
                return NotFound("There is no such player");
            }

            await _playerService.DeletePlayer(player);

            return NoContent();
        }
    }
}
