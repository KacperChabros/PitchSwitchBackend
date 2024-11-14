using PitchSwitchBackend.Dtos;
using PitchSwitchBackend.Dtos.Player.Requests;
using PitchSwitchBackend.Dtos.Player.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Services.PlayerService
{
    public interface IPlayerService
    {
        Task<NewPlayerDto?> AddPlayer(AddPlayerDto addPlayerDto);
        Task<PaginatedListDto<PlayerDto>> GetPlayers(PlayerQueryObject playerQuery);
        Task<List<MinimalPlayerDto>> GetAllMinimalPlayers();
        Task<Player?> GetPlayerById(int playerId);
        Task<Player?> GetPlayerWithClubById(int playerId);
        Task<Player?> GetPlayerByIdWithAllData(int playerId);
        Task<PlayerDto?> UpdatePlayer(Player player, UpdatePlayerDto updatePlayerDto);
        Task DeletePlayer(Player player);
        Task<bool> PlayerExists(int playerId);
        Task<int?> GetPlayerClubIdById(int playerId);
    }
}
