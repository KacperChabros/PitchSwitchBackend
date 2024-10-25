using Microsoft.EntityFrameworkCore;
using PitchSwitchBackend.Data;
using PitchSwitchBackend.Dtos.Player.Requests;
using PitchSwitchBackend.Dtos.Player.Responses;
using PitchSwitchBackend.Helpers;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Models;
using PitchSwitchBackend.Services.ClubService;

namespace PitchSwitchBackend.Services.PlayerService
{
    public class PlayerService : IPlayerService
    {
        private readonly ApplicationDBContext _context;
        private readonly IClubService _clubService;
        public PlayerService(ApplicationDBContext context,
            IClubService clubService)
        {
            _context = context;
            _clubService = clubService;
        }

        public async Task<NewPlayerDto?> AddPlayer(AddPlayerDto addPlayerDto)
        {
            var result = await _context.Players.AddAsync(addPlayerDto.FromAddNewPlayerDtoToModel());
            await _context.SaveChangesAsync();
            var player = result.Entity;
            if(player?.ClubId != null)
            {
                var club = await _clubService.GetClubById(player.ClubId.GetValueOrDefault());
                player.Club = club;
            }

            return result.Entity?.FromModelToNewPlayerDto();
        }

        public async Task<List<PlayerDto>> GetPlayers(PlayerQueryObject playerQuery)
        {
            var players = _context.Players.AsQueryable();

            players = FilterPlayers(players, playerQuery);

            players = SortPlayers(players, playerQuery);
            
            var skipNumber = (playerQuery.PageNumber - 1) * playerQuery.PageSize;

            var filteredPlayers = await players.Skip(skipNumber).Take(playerQuery.PageSize).Include(p => p.Club).ToListAsync();
            
            return filteredPlayers.Select(p => p.FromModelToPlayerDto()).ToList();
        }

        public async Task<Player?> GetPlayerById(int playerId)
        {
            return await _context.Players.FirstOrDefaultAsync(p => p.PlayerId == playerId);
        }

        public async Task<Player?> GetPlayerWithClubById(int playerId)
        {
            return await _context.Players.Include(p => p.Club).FirstOrDefaultAsync(p => p.PlayerId == playerId);
        }

        public async Task<PlayerDto?> UpdatePlayer(Player player, UpdatePlayerDto updatePlayerDto)
        {
            if (!string.IsNullOrWhiteSpace(updatePlayerDto.FirstName))
                player.FirstName = updatePlayerDto.FirstName;

            if (!string.IsNullOrWhiteSpace(updatePlayerDto.LastName))
                player.LastName = updatePlayerDto.LastName;

            if (!string.IsNullOrWhiteSpace(updatePlayerDto.Nationality))
                player.Nationality = updatePlayerDto.Nationality;

            if (!string.IsNullOrWhiteSpace(updatePlayerDto.Position))
                player.Position = updatePlayerDto.Position;

            if (updatePlayerDto.DateOfBirth != null)
                player.DateOfBirth = (DateTime)updatePlayerDto.DateOfBirth;

            if (updatePlayerDto.Height != null)
                player.Height = (int)updatePlayerDto.Height;

            if (updatePlayerDto.Weight != null)
                player.Weight = (int)updatePlayerDto.Weight;

            if (updatePlayerDto.PreferredFoot != null)
                player.PreferredFoot = (Enums.Foot)updatePlayerDto.PreferredFoot;

            if (updatePlayerDto.MarketValue != null)
                player.MarketValue = (decimal)updatePlayerDto.MarketValue;

            if (updatePlayerDto.IsPhotoUrlDeleted)
                player.PhotoUrl = null;
            else if (!string.IsNullOrWhiteSpace(updatePlayerDto.PhotoUrl))
                player.PhotoUrl = updatePlayerDto.PhotoUrl;

            if (updatePlayerDto.IsClubIdDeleted)
                player.ClubId = null;
            else if (updatePlayerDto.ClubId != null)
                player.ClubId = updatePlayerDto.ClubId;

            await _context.SaveChangesAsync();

            await _context.Entry(player).Reference(p => p.Club).LoadAsync();

            return player.FromModelToPlayerDto();
        }

        public async Task DeletePlayer(Player player)
        {
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
        }

        private IQueryable<Player> FilterPlayers(IQueryable<Player> players, PlayerQueryObject playerQuery)
        {
            if (!string.IsNullOrWhiteSpace(playerQuery.FirstName))
                players = players.Where(p => p.FirstName.ToLower().Contains(playerQuery.FirstName.ToLower()));

            if (!string.IsNullOrWhiteSpace(playerQuery.LastName))
                players = players.Where(p => p.LastName.ToLower().Contains(playerQuery.LastName.ToLower()));

            if (!string.IsNullOrWhiteSpace(playerQuery.Nationality))
                players = players.Where(p => p.Nationality.ToLower().Contains(playerQuery.Nationality.ToLower()));

            if (!string.IsNullOrWhiteSpace(playerQuery.Position))
                players = players.Where(p => p.Position.ToLower().Contains(playerQuery.Position.ToLower()));

            if (playerQuery.PreferredFoot != null)
                players = players.Where(p => p.PreferredFoot == playerQuery.PreferredFoot);

            if (playerQuery.ClubId != null)
            {
                players = players.Where(p => p.ClubId == playerQuery.ClubId);
            }
            else if (playerQuery.IsUnemployed != null)
            {
                if (playerQuery.IsUnemployed == true)
                    players = players.Where(p => p.ClubId == null);
                else
                    players = players.Where(p => p.ClubId != null);
            }

            if (playerQuery.DateOfBirth != null)
            {
                players = playerQuery.DateOfBirthComparison switch
                {
                    NumberComparisonTypes.Equal => players.Where(p => p.DateOfBirth == playerQuery.DateOfBirth),
                    NumberComparisonTypes.Less => players.Where(p => p.DateOfBirth < playerQuery.DateOfBirth),
                    NumberComparisonTypes.LessEqual => players.Where(p => p.DateOfBirth <= playerQuery.DateOfBirth),
                    NumberComparisonTypes.Greater => players.Where(p => p.DateOfBirth > playerQuery.DateOfBirth),
                    NumberComparisonTypes.GreaterEqual => players.Where(p => p.DateOfBirth >= playerQuery.DateOfBirth),
                    _ => players
                };
            }

            if (playerQuery.Height != null)
            {
                players = playerQuery.HeightComparison switch
                {
                    NumberComparisonTypes.Equal => players.Where(p => p.Height == playerQuery.Height),
                    NumberComparisonTypes.Less => players.Where(p => p.Height < playerQuery.Height),
                    NumberComparisonTypes.LessEqual => players.Where(p => p.Height <= playerQuery.Height),
                    NumberComparisonTypes.Greater => players.Where(p => p.Height > playerQuery.Height),
                    NumberComparisonTypes.GreaterEqual => players.Where(p => p.Height >= playerQuery.Height),
                    _ => players
                };
            }

            if (playerQuery.Weight != null)
            {
                players = playerQuery.WeightComparison switch
                {
                    NumberComparisonTypes.Equal => players.Where(p => p.Weight == playerQuery.Weight),
                    NumberComparisonTypes.Less => players.Where(p => p.Weight < playerQuery.Weight),
                    NumberComparisonTypes.LessEqual => players.Where(p => p.Weight <= playerQuery.Weight),
                    NumberComparisonTypes.Greater => players.Where(p => p.Weight > playerQuery.Weight),
                    NumberComparisonTypes.GreaterEqual => players.Where(p => p.Weight >= playerQuery.Weight),
                    _ => players
                };
            }

            if (playerQuery.MarketValue != null)
            {
                players = playerQuery.MarketValueComparison switch
                {
                    NumberComparisonTypes.Equal => players.Where(p => p.MarketValue == playerQuery.MarketValue),
                    NumberComparisonTypes.Less => players.Where(p => p.MarketValue < playerQuery.MarketValue),
                    NumberComparisonTypes.LessEqual => players.Where(p => p.MarketValue <= playerQuery.MarketValue),
                    NumberComparisonTypes.Greater => players.Where(p => p.MarketValue > playerQuery.MarketValue),
                    NumberComparisonTypes.GreaterEqual => players.Where(p => p.MarketValue >= playerQuery.MarketValue),
                    _ => players
                };
            }

            return players;
        }

        private IQueryable<Player> SortPlayers(IQueryable<Player> players, PlayerQueryObject playerQuery)
        {
            if (!string.IsNullOrWhiteSpace(playerQuery.SortBy))
            {
                if (playerQuery.SortBy.Equals(nameof(PlayerQueryObject.FirstName), StringComparison.OrdinalIgnoreCase))
                    players = playerQuery.IsDescending ? players.OrderByDescending(p => p.FirstName) : players.OrderBy(p => p.FirstName);
                else if (playerQuery.SortBy.Equals(nameof(PlayerQueryObject.LastName), StringComparison.OrdinalIgnoreCase))
                    players = playerQuery.IsDescending ? players.OrderByDescending(p => p.LastName) : players.OrderBy(p => p.LastName);
                else if (playerQuery.SortBy.Equals(nameof(PlayerQueryObject.Nationality), StringComparison.OrdinalIgnoreCase))
                    players = playerQuery.IsDescending ? players.OrderByDescending(p => p.Nationality) : players.OrderBy(p => p.Nationality);
                else if (playerQuery.SortBy.Equals(nameof(PlayerQueryObject.Position), StringComparison.OrdinalIgnoreCase))
                    players = playerQuery.IsDescending ? players.OrderByDescending(p => p.Position) : players.OrderBy(p => p.Position);
                else if (playerQuery.SortBy.Equals(nameof(PlayerQueryObject.DateOfBirth), StringComparison.OrdinalIgnoreCase))
                    players = playerQuery.IsDescending ? players.OrderByDescending(p => p.DateOfBirth) : players.OrderBy(p => p.DateOfBirth);
                else if (playerQuery.SortBy.Equals(nameof(PlayerQueryObject.Height), StringComparison.OrdinalIgnoreCase))
                    players = playerQuery.IsDescending ? players.OrderByDescending(p => p.Height) : players.OrderBy(p => p.Height);
                else if (playerQuery.SortBy.Equals(nameof(PlayerQueryObject.Weight), StringComparison.OrdinalIgnoreCase))
                    players = playerQuery.IsDescending ? players.OrderByDescending(p => p.Weight) : players.OrderBy(p => p.Weight);
                else if (playerQuery.SortBy.Equals(nameof(PlayerQueryObject.MarketValue), StringComparison.OrdinalIgnoreCase))
                    players = playerQuery.IsDescending ? players.OrderByDescending(p => p.MarketValue) : players.OrderBy(p => p.MarketValue);
            }

            return players;
        }
    }
}
