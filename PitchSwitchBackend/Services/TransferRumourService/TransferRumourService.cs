using Microsoft.EntityFrameworkCore;
using PitchSwitchBackend.Data;
using PitchSwitchBackend.Dtos.TransferRumour.Requests;
using PitchSwitchBackend.Dtos.TransferRumour.Responses;
using PitchSwitchBackend.Helpers;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Models;
using PitchSwitchBackend.Services.ClubService;
using PitchSwitchBackend.Services.PlayerService;

namespace PitchSwitchBackend.Services.TransferRumourService
{
    public class TransferRumourService : ITransferRumourService
    {
        private readonly ApplicationDBContext _context;
        private readonly IClubService _clubService;
        private readonly IPlayerService _playerService;
        public TransferRumourService(ApplicationDBContext context,
            IClubService clubService,
            IPlayerService playerService)
        {
            _context = context;
            _clubService = clubService;
            _playerService = playerService;
        }

        public async Task<NewTransferRumourDto?> AddTransferRumour(AddTransferRumourDto addTransferRumourDto)
        {
            await ValidateExistenceOfData(addTransferRumourDto.PlayerId, addTransferRumourDto.BuyingClubId);
            
            var player = await _playerService.GetPlayerWithClubById(addTransferRumourDto.PlayerId);
            
            ValidateNewTransferRumourData(player?.ClubId, addTransferRumourDto.BuyingClubId);

            var transferRumourModel = addTransferRumourDto.FromAddTransferRumourDtoToModel();
            transferRumourModel.SellingClubId = player?.ClubId;

            var result = await _context.TransferRumours.AddAsync(transferRumourModel);
            await _context.SaveChangesAsync();
            var transferRumour = result.Entity;

            if (transferRumour != null)
            {
                if (transferRumour.SellingClubId.HasValue)
                {
                    transferRumour.SellingClub = player?.Club;
                }
                if (transferRumour.BuyingClubId.HasValue)
                {
                    transferRumour.BuyingClub = await _clubService.GetClubById(transferRumour.BuyingClubId.GetValueOrDefault());
                }
                transferRumour.Player = player;
            }

            return transferRumour?.FromModelToNewTransferRumourDto();
        }

        public async Task<List<TransferRumourDto>> GetTransferRumours(TransferRumourQueryObject transferRumourQuery)
        {
            var transferRumours = _context.TransferRumours.AsQueryable().Where(tr => !tr.IsArchived);

            transferRumours = FilterTransferRumours(transferRumours, transferRumourQuery);

            transferRumours = SortTransferRumours(transferRumours, transferRumourQuery);

            var skipNumber = (transferRumourQuery.PageNumber - 1) * transferRumourQuery.PageSize;

            var filteredTransfers = await transferRumours.Skip(skipNumber).Take(transferRumourQuery.PageSize)
                .Include(tr => tr.Player).Include(tr => tr.SellingClub).Include(tr => tr.BuyingClub).ToListAsync();

            return filteredTransfers.Select(tr => tr.FromModelToTransferRumourDto()).ToList();
        }

        public async Task<TransferRumour?> GetTransferRumourById(int transferRumourId)
        {
            return await _context.TransferRumours.FirstOrDefaultAsync(tr => tr.TransferRumourId == transferRumourId);
        }

        public async Task<TransferRumour?> GetTransferRumourWithDataById(int transferRumourId)
        {
            return await _context.TransferRumours
                .Include(tr => tr.Player).Include(tr => tr.SellingClub).Include(tr => tr.BuyingClub).FirstOrDefaultAsync(tr => tr.TransferRumourId == transferRumourId);
        }

        public async Task<TransferRumourDto?> UpdateTransferRumour(TransferRumour transferRumour, UpdateTransferRumourDto updateTransferRumourDto)
        {
            await ValidateExistenceOfData(updateTransferRumourDto.PlayerId, updateTransferRumourDto.BuyingClubId);
            if (updateTransferRumourDto.PlayerId.HasValue)
            {
                transferRumour.PlayerId = updateTransferRumourDto.PlayerId.Value;
                var player = await _playerService.GetPlayerById(transferRumour.PlayerId);
                transferRumour.SellingClubId = player?.ClubId;
            }

            ValidateUpdateTransferData(transferRumour, updateTransferRumourDto);

            if (updateTransferRumourDto.TransferType.HasValue)
                transferRumour.TransferType = updateTransferRumourDto.TransferType.Value;

            if (updateTransferRumourDto.RumouredFee.HasValue)
                transferRumour.RumouredFee = updateTransferRumourDto.RumouredFee.Value;

            if (updateTransferRumourDto.ConfidenceLevel.HasValue)
                transferRumour.ConfidenceLevel = updateTransferRumourDto.ConfidenceLevel.Value;

            if (updateTransferRumourDto.IsBuyingClubDeleted)
                transferRumour.BuyingClubId = null;
            else if (updateTransferRumourDto.BuyingClubId.HasValue)
                transferRumour.BuyingClubId = updateTransferRumourDto.BuyingClubId.Value;

 
                
            await _context.SaveChangesAsync();

            await _context.Entry(transferRumour).Reference(tr => tr.Player).LoadAsync();
            await _context.Entry(transferRumour).Reference(tr => tr.SellingClub).LoadAsync();
            await _context.Entry(transferRumour).Reference(tr => tr.BuyingClub).LoadAsync();

            return transferRumour.FromModelToTransferRumourDto();
        }

        public async Task<bool> ArchiveTransferRumour(int transferRumourId, bool isConfirmed)
        {
            var transferRumour = await _context.TransferRumours
                .FirstOrDefaultAsync(tr => tr.TransferRumourId == transferRumourId);

            if (transferRumour == null)
                return false;

            transferRumour.IsConfirmed = isConfirmed;
            transferRumour.IsArchived = true;
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task ValidateExistenceOfData(int? playerId, int? buyingClubId)
        {
            if (!await ValidatePlayerExists(playerId))
                throw new ArgumentException("Player does not exist");

            if (!await ValidateClubExists(buyingClubId))
                throw new ArgumentException("Given club does not exist");
        }

        private async Task<bool> ValidatePlayerExists(int? playerId)
        {
            return playerId == null || await _playerService.PlayerExists(playerId.Value);
        }
        private async Task<bool> ValidateClubExists(int? clubId)
        {
            return clubId == null || await _clubService.ClubExistsAndNotArchived(clubId.Value);
        }

        private void ValidateNewTransferRumourData(int? sellingClubId, int? buyingClubId)
        {
            if (sellingClubId.HasValue && buyingClubId.HasValue &&
                sellingClubId == buyingClubId)
            {
                throw new ArgumentException("SellingClub cannot be the same as BuyingClub.");
            }
            else if (!sellingClubId.HasValue && !buyingClubId.HasValue)
            {
                throw new ArgumentException("Both clubs cannot be null");
            }
        }

        private void ValidateUpdateTransferData(TransferRumour transferRumour, UpdateTransferRumourDto updateTransferRumourDto)
        {
            int? newSellingClubId = transferRumour.SellingClubId;
            int? newBuyingClubId = updateTransferRumourDto.IsBuyingClubDeleted ? null : updateTransferRumourDto.BuyingClubId ?? transferRumour.BuyingClubId;

            if (newSellingClubId == null && newBuyingClubId == null)
            {
                throw new ArgumentException("SellingClub and BuyingClub cannot be null at once");
            }

            if (newSellingClubId.HasValue && newBuyingClubId.HasValue && newSellingClubId == newBuyingClubId)
            {
                throw new ArgumentException("SellingClub and BuyingClub cannot have the same value.");
            }
        }

        private IQueryable<TransferRumour> FilterTransferRumours(IQueryable<TransferRumour> transferRumours, TransferRumourQueryObject transferRumourQuery)
        {
            if (transferRumourQuery.TransferType.HasValue)
                transferRumours = transferRumours.Where(tr => tr.TransferType == transferRumourQuery.TransferType);

            if (transferRumourQuery.RumouredFee.HasValue)
            {
                transferRumours = transferRumourQuery.RumouredFeeComparison switch
                {
                    NumberComparisonTypes.Equal => transferRumours.Where(tr => tr.RumouredFee == transferRumourQuery.RumouredFee),
                    NumberComparisonTypes.Less => transferRumours.Where(tr => tr.RumouredFee < transferRumourQuery.RumouredFee),
                    NumberComparisonTypes.LessEqual => transferRumours.Where(tr => tr.RumouredFee <= transferRumourQuery.RumouredFee),
                    NumberComparisonTypes.Greater => transferRumours.Where(tr => tr.RumouredFee > transferRumourQuery.RumouredFee),
                    NumberComparisonTypes.GreaterEqual => transferRumours.Where(tr => tr.RumouredFee >= transferRumourQuery.RumouredFee),
                    _ => transferRumours
                };
            }

            if (transferRumourQuery.ConfidenceLevel.HasValue)
            {
                transferRumours = transferRumourQuery.ConfidenceLevelComparison switch
                {
                    NumberComparisonTypes.Equal => transferRumours.Where(tr => tr.ConfidenceLevel == transferRumourQuery.ConfidenceLevel),
                    NumberComparisonTypes.Less => transferRumours.Where(tr => tr.ConfidenceLevel < transferRumourQuery.ConfidenceLevel),
                    NumberComparisonTypes.LessEqual => transferRumours.Where(tr => tr.ConfidenceLevel <= transferRumourQuery.ConfidenceLevel),
                    NumberComparisonTypes.Greater => transferRumours.Where(tr => tr.ConfidenceLevel > transferRumourQuery.ConfidenceLevel),
                    NumberComparisonTypes.GreaterEqual => transferRumours.Where(tr => tr.ConfidenceLevel >= transferRumourQuery.ConfidenceLevel),
                    _ => transferRumours
                };
            }


            if (transferRumourQuery.PlayerId.HasValue)
                transferRumours = transferRumours.Where(t => t.PlayerId == transferRumourQuery.PlayerId);

            if (transferRumourQuery.SellingClubId.HasValue)
            {
                transferRumours = transferRumours.Where(t => t.SellingClubId == transferRumourQuery.SellingClubId);
            }
            else if (transferRumourQuery.FilterForEmptySellingClubIfEmpty)
            {
                transferRumours = transferRumours.Where(t => t.SellingClubId == null);
            }

            if (transferRumourQuery.BuyingClubId.HasValue)
            {
                transferRumours = transferRumours.Where(t => t.BuyingClubId == transferRumourQuery.BuyingClubId);
            }
            else if (transferRumourQuery.FilterForEmptyBuyingClubIfEmpty)
            {
                transferRumours = transferRumours.Where(t => t.BuyingClubId == null);
            }


            return transferRumours;
        }

        private IQueryable<TransferRumour> SortTransferRumours(IQueryable<TransferRumour> transferRumours, TransferRumourQueryObject transferRumourQuery)
        {
            if (!string.IsNullOrWhiteSpace(transferRumourQuery.SortBy))
            {
                if (transferRumourQuery.SortBy.Equals(nameof(TransferRumourQueryObject.RumouredFee), StringComparison.OrdinalIgnoreCase))
                    transferRumours = transferRumourQuery.IsDescending ? transferRumours.OrderByDescending(tr => tr.RumouredFee) : transferRumours.OrderBy(tr => tr.RumouredFee);
                else if (transferRumourQuery.SortBy.Equals(nameof(TransferRumourQueryObject.ConfidenceLevel), StringComparison.OrdinalIgnoreCase))
                    transferRumours = transferRumourQuery.IsDescending ? transferRumours.OrderByDescending(tr => tr.RumouredFee) : transferRumours.OrderBy(tr => tr.RumouredFee);
            }

            return transferRumours;
        }
    }
}
