using Microsoft.EntityFrameworkCore;
using PitchSwitchBackend.Data;
using PitchSwitchBackend.Dtos;
using PitchSwitchBackend.Dtos.Player.Requests;
using PitchSwitchBackend.Dtos.Player.Responses;
using PitchSwitchBackend.Helpers;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Models;
using PitchSwitchBackend.Services.ClubService;
using PitchSwitchBackend.Services.ImageService;

namespace PitchSwitchBackend.Services.PlayerService
{
    public class PlayerService : IPlayerService
    {
        private readonly ApplicationDBContext _context;
        private readonly IClubService _clubService;
        private readonly IImageService _imageService;
        public PlayerService(ApplicationDBContext context,
            IClubService clubService,
            IImageService imageService)
        {
            _context = context;
            _clubService = clubService;
            _imageService = imageService;
        }

        public async Task<NewPlayerDto?> AddPlayer(AddPlayerDto addPlayerDto)
        {
            if (!await ValidateClubExists(addPlayerDto.ClubId))
            {
                throw new ArgumentException("Given club does not exist");
            }
            var player = addPlayerDto.FromAddNewPlayerDtoToModel();
            if (addPlayerDto.Photo != null)
            {
                var photoPath = await _imageService.UploadFileAsync(addPlayerDto.Photo, UploadFolders.PlayersDir);
                player.PhotoUrl = photoPath;
            }

            var result = await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();
            var addedPlayer = await GetPlayerByIdWithAllData(result.Entity.PlayerId);

            return addedPlayer?.FromModelToNewPlayerDto();
        }

        public async Task<PaginatedListDto<PlayerDto>> GetPlayers(PlayerQueryObject playerQuery)
        {
            var players = _context.Players.AsQueryable();

            players = FilterPlayers(players, playerQuery);

            players = SortPlayers(players, playerQuery);
            
            var totalCount = players.Count();

            var skipNumber = (playerQuery.PageNumber - 1) * playerQuery.PageSize;

            var filteredPlayers = await players.Skip(skipNumber).Take(playerQuery.PageSize).Include(p => p.Club).ToListAsync();
            var paginatedPlayers = filteredPlayers.Select(p => p.FromModelToPlayerDto()).ToList();
            return new PaginatedListDto<PlayerDto>
            {
                Items = paginatedPlayers,
                TotalCount = totalCount
            };
        }

        public async Task<List<MinimalPlayerDto>> GetAllMinimalPlayers()
        {
            var players = _context.Players.Include(p => p.Club).AsQueryable();
            return await players.Select(p => p.FromModelToMinimalPlayerDto()).ToListAsync();
        }

        public async Task<Player?> GetPlayerById(int playerId)
        {
            return await _context.Players.FirstOrDefaultAsync(p => p.PlayerId == playerId);
        }

        public async Task<Player?> GetPlayerWithClubById(int playerId)
        {
            return await _context.Players.Include(p => p.Club).FirstOrDefaultAsync(p => p.PlayerId == playerId);
        }

        public async Task<Player?> GetPlayerByIdWithAllData(int playerId, bool includePosts=false)
        {
            if(includePosts)
            {
                return await _context.Players.Include(p => p.Club).Include(p => p.Transfers).ThenInclude(t => t.Posts).Include(p => p.TransferRumours).ThenInclude(tr => tr.Posts)
                        .FirstOrDefaultAsync(p => p.PlayerId == playerId);
            }
            return await _context.Players.Include(p => p.Club).Include(p => p.Transfers).Include(p => p.TransferRumours)
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);
        }

        public async Task<PlayerDto?> UpdatePlayer(Player player, UpdatePlayerDto updatePlayerDto)
        {
            if (!await ValidateClubExists(updatePlayerDto.ClubId))
            {
                throw new ArgumentException("Given club does not exist");
            }

            if (updatePlayerDto.Photo != null)
            {
                var oldPhotoUrl = player.PhotoUrl;
                var newPhotoUrl = await _imageService.UploadFileAsync(updatePlayerDto.Photo, UploadFolders.PlayersDir);
                player.PhotoUrl = newPhotoUrl;
                if (!string.IsNullOrEmpty(oldPhotoUrl))
                    _imageService.DeleteFile(oldPhotoUrl);
            }
            else if (updatePlayerDto.IsPhotoDeleted)
            {
                if (!string.IsNullOrEmpty(player.PhotoUrl))
                    _imageService.DeleteFile(player.PhotoUrl);
                player.PhotoUrl = null;
            }

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

            if (updatePlayerDto.IsClubIdDeleted)
                player.ClubId = null;
            else if (updatePlayerDto.ClubId != null)
                player.ClubId = updatePlayerDto.ClubId;

            await _context.SaveChangesAsync();

            var updatedPlayer = await GetPlayerByIdWithAllData(player.PlayerId);

            return updatedPlayer?.FromModelToPlayerDto();
        }

        public async Task DeletePlayer(Player player)
        {
            foreach (var transfer in player.Transfers)
            {
                foreach (var post in transfer.Posts)
                {
                    post.TransferId = null;
                }
            }
            _context.Transfers.RemoveRange(player.Transfers);

            foreach (var transferRumour in player.TransferRumours)
            {
                foreach(var post in transferRumour.Posts)
                {
                    post.TransferRumourId = null;
                }
            }
            _context.TransferRumours.RemoveRange(player.TransferRumours);

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> PlayerExists(int playerId)
        {
            return await _context.Players.AnyAsync(p => p.PlayerId == playerId);
        }

        public async Task<int?> GetPlayerClubIdById(int playerId)
        {
            var player = await GetPlayerById(playerId);
            return player?.ClubId;
        }

        private async Task<bool> ValidateClubExists(int? clubId)
        {
            return clubId == null || await _clubService.ClubExistsAndNotArchived(clubId.Value);
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
            else if (playerQuery.FilterForUnemployedIfClubIsEmpty)
            {
                players = players.Where(p => p.ClubId == null);
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
