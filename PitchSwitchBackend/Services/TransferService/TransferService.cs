using Microsoft.EntityFrameworkCore;
using PitchSwitchBackend.Data;
using PitchSwitchBackend.Dtos.Player.Requests;
using PitchSwitchBackend.Dtos.Transfer.Requests;
using PitchSwitchBackend.Dtos.Transfer.Responses;
using PitchSwitchBackend.Helpers;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Models;
using PitchSwitchBackend.Services.ClubService;
using PitchSwitchBackend.Services.PlayerService;

namespace PitchSwitchBackend.Services.TransferService
{
    public class TransferService : ITransferService
    {
        private readonly ApplicationDBContext _context;
        private readonly IClubService _clubService;
        private readonly IPlayerService _playerService;
        public TransferService(ApplicationDBContext context,
            IClubService clubService,
            IPlayerService playerService)
        {
            _context = context;
            _clubService = clubService;
            _playerService = playerService;
        }

        public async Task<NewTransferDto?> AddTransfer(AddTransferDto addTransferDto)
        {
            await ValidateExistenceOfData(addTransferDto.PlayerId, addTransferDto.SellingClubId, addTransferDto.BuyingClubId);
            await ValidateNewTransferData(addTransferDto.PlayerId, addTransferDto.SellingClubId, addTransferDto.BuyingClubId, addTransferDto.TransferDate);

            var mapp = addTransferDto.FromAddTransferDtoToModel();
            var result = await _context.Transfers.AddAsync(addTransferDto.FromAddTransferDtoToModel());
            await _context.SaveChangesAsync();
            var transfer = result.Entity;
            if (transfer.TransferDate.Date == DateTime.Today)
            {
                var player = await _playerService.GetPlayerById(transfer.PlayerId);
                var updatePlayerDto = new UpdatePlayerDto { ClubId = transfer.BuyingClubId,  IsClubIdDeleted = transfer.BuyingClubId == null};
                await _playerService.UpdatePlayer(player, updatePlayerDto);
            }

            var addedTransfer = await GetTransferWithDataById(result.Entity.TransferId);
            return addedTransfer?.FromModelToNewTransferDto();
        }

        public async Task<List<TransferDto>> GetTransfers(TransferQueryObject transferQuery)
        {
            var transfers = _context.Transfers.AsQueryable();

            transfers = FilterTransfers(transfers, transferQuery);

            transfers = SortTransfers(transfers, transferQuery);

            var skipNumber = (transferQuery.PageNumber - 1) * transferQuery.PageSize;

            var filteredTransfers = await transfers.Skip(skipNumber).Take(transferQuery.PageSize)
                .Include(t => t.Player).Include(t => t.SellingClub).Include(t => t.BuyingClub).ToListAsync();

            return filteredTransfers.Select(t => t.FromModelToTransferDto()).ToList();
        }

        public async Task<Transfer?> GetTransferWithDataById(int transferId)
        {
            return await _context.Transfers.Include(t => t.Player).ThenInclude(p => p.Club)
                .Include(t => t.SellingClub).Include(t => t.BuyingClub)
                .Include(t => t.Posts)
                .FirstOrDefaultAsync(t => t.TransferId == transferId);
        }

        public async Task<Transfer?> GetTransferById(int transferId)
        {
            return await _context.Transfers.FirstOrDefaultAsync(t => t.TransferId == transferId);
        }

        public async Task<TransferDto?> UpdateTransfer(Transfer transfer, UpdateTransferDto updateTransferDto)
        {
            await ValidateExistenceOfData(updateTransferDto.PlayerId, updateTransferDto.SellingClubId, updateTransferDto.BuyingClubId);
            await ValidateUpdateTransferData(transfer, updateTransferDto);

            if (updateTransferDto.TransferType.HasValue)
                transfer.TransferType = updateTransferDto.TransferType.Value;

            if (updateTransferDto.TransferDate.HasValue)
                transfer.TransferDate = updateTransferDto.TransferDate.Value;

            if (updateTransferDto.TransferFee.HasValue)
                transfer.TransferFee = updateTransferDto.TransferFee.Value;

            if (updateTransferDto.IsSellingClubDeleted)
                transfer.SellingClubId = null;
            else if (updateTransferDto.SellingClubId.HasValue)
                transfer.SellingClubId = updateTransferDto.SellingClubId.Value;

            if (updateTransferDto.IsBuyingClubDeleted)
                transfer.BuyingClubId = null;
            else if (updateTransferDto.BuyingClubId.HasValue)
                transfer.BuyingClubId = updateTransferDto.BuyingClubId.Value;

            if (updateTransferDto.PlayerId.HasValue)
                transfer.PlayerId = updateTransferDto.PlayerId.Value;

            if (transfer.TransferDate.Date == DateTime.Today)
            {
                var player = await _playerService.GetPlayerById(transfer.PlayerId);
                var updatePlayerDto = new UpdatePlayerDto { ClubId = transfer.BuyingClubId, IsClubIdDeleted = transfer.BuyingClubId == null };
                await _playerService.UpdatePlayer(player, updatePlayerDto);
            }

            await _context.SaveChangesAsync();

            var updatedTransfer = await GetTransferWithDataById(transfer.TransferId);

            return updatedTransfer?.FromModelToTransferDto();
        }
        public async Task DeleteTransfer(Transfer transfer)
        {
            foreach (var post in transfer.Posts)
            {
                post.TransferId = null;
            }

            _context.Transfers.Remove(transfer);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TransferExists(int transferId)
        {
            return await _context.Transfers.AnyAsync(t => t.TransferId == transferId);
        }

        private async Task ValidateExistenceOfData(int? playerId, int? sellingClubId, int? buyingClubId)
        {
            if (!await ValidatePlayerExists(playerId))
                throw new ArgumentException("Player does not exist");

            if (!await ValidateClubExists(sellingClubId) ||
                !await ValidateClubExists(buyingClubId))
            {
                throw new ArgumentException("Given club does not exist");
            }
        }

        private async Task ValidateNewTransferData(int playerId, int? sellingClubId, int? buyingClubId, DateTime? transferDate)
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
            
            if(transferDate.HasValue && transferDate.Value.Date == DateTime.Today)
            {
                if (await _playerService.GetPlayerClubIdById(playerId) != sellingClubId)
                    throw new ArgumentException("Selling club cannot be different than player's club when the transfer date is today");
            }
        }

        private async Task ValidateUpdateTransferData(Transfer transfer, UpdateTransferDto updateTransferDto)
        {
            int? newSellingClubId = updateTransferDto.IsSellingClubDeleted ? null : updateTransferDto.SellingClubId ?? transfer.SellingClubId;
            int? newBuyingClubId = updateTransferDto.IsBuyingClubDeleted ? null : updateTransferDto.BuyingClubId ?? transfer.BuyingClubId;
            var newTransferDat = updateTransferDto.TransferDate ?? transfer.TransferDate;
            var newPlayerId = updateTransferDto.PlayerId ?? transfer.PlayerId;

            if (newSellingClubId == null && newBuyingClubId == null)
            {
                throw new ArgumentException("SellingClub and BuyingClub cannot be null at once");
            }

            if (newSellingClubId.HasValue && newBuyingClubId.HasValue && newSellingClubId == newBuyingClubId)
            {
                throw new ArgumentException("SellingClub and BuyingClub cannot have the same value.");
            }

            if(newTransferDat.Date == DateTime.Today)
            {
                if (newSellingClubId != await _playerService.GetPlayerClubIdById(newPlayerId))
                    throw new ArgumentException("SellingClubId cannot be different from player's club for transfers made today");
            }
        }

        private async Task<bool> ValidatePlayerExists(int? playerId)
        {
            return playerId == null || await _playerService.PlayerExists(playerId.Value);
        }
        private async Task<bool> ValidateClubExists(int? clubId)
        {
            return clubId == null || await _clubService.ClubExistsAndNotArchived(clubId.Value);
        }

        private IQueryable<Transfer> FilterTransfers(IQueryable<Transfer> transfers, TransferQueryObject transferQuery)
        {
            if (transferQuery.TransferType.HasValue)
                transfers = transfers.Where(t => t.TransferType == transferQuery.TransferType);

            if (transferQuery.TransferDate.HasValue)
            {
                transfers = transferQuery.TransferDateComparison switch
                {
                    NumberComparisonTypes.Equal => transfers.Where(t => t.TransferDate.Date == transferQuery.TransferDate.Value.Date),
                    NumberComparisonTypes.Less => transfers.Where(t => t.TransferDate.Date < transferQuery.TransferDate.Value.Date),
                    NumberComparisonTypes.LessEqual => transfers.Where(t => t.TransferDate.Date <= transferQuery.TransferDate.Value.Date),
                    NumberComparisonTypes.Greater => transfers.Where(t => t.TransferDate.Date > transferQuery.TransferDate.Value.Date),
                    NumberComparisonTypes.GreaterEqual => transfers.Where(t => t.TransferDate.Date >= transferQuery.TransferDate.Value.Date),
                    _ => transfers
                };
            }

            if (transferQuery.TransferFee.HasValue)
            {
                transfers = transferQuery.TransferFeeComparison switch
                {
                    NumberComparisonTypes.Equal => transfers.Where(t => t.TransferFee == transferQuery.TransferFee),
                    NumberComparisonTypes.Less => transfers.Where(t => t.TransferFee < transferQuery.TransferFee),
                    NumberComparisonTypes.LessEqual => transfers.Where(t => t.TransferFee <= transferQuery.TransferFee),
                    NumberComparisonTypes.Greater => transfers.Where(t => t.TransferFee > transferQuery.TransferFee),
                    NumberComparisonTypes.GreaterEqual => transfers.Where(t => t.TransferFee >= transferQuery.TransferFee),
                    _ => transfers
                };
            }

            if (transferQuery.PlayerId.HasValue)
                transfers = transfers.Where(t => t.PlayerId == transferQuery.PlayerId);

            if (transferQuery.SellingClubId.HasValue)
            {
                transfers = transfers.Where(t => t.SellingClubId == transferQuery.SellingClubId);
            }
            else if (transferQuery.FilterForEmptySellingClubIfEmpty)
            {
                transfers = transfers.Where(t => t.SellingClubId == null);
            }

            if (transferQuery.BuyingClubId.HasValue)
            {
                transfers = transfers.Where(t => t.BuyingClubId == transferQuery.BuyingClubId);
            }
            else if (transferQuery.FilterForEmptyBuyingClubIfEmpty)
            {
                transfers = transfers.Where(t => t.BuyingClubId == null);
            }


            return transfers;
        }

        private IQueryable<Transfer> SortTransfers(IQueryable<Transfer> transfers, TransferQueryObject transferQuery)
        {
            if (!string.IsNullOrWhiteSpace(transferQuery.SortBy))
            {
                if (transferQuery.SortBy.Equals(nameof(TransferQueryObject.TransferDate), StringComparison.OrdinalIgnoreCase))
                    transfers = transferQuery.IsDescending ? transfers.OrderByDescending(t => t.TransferDate) : transfers.OrderBy(t => t.TransferDate);
                else if (transferQuery.SortBy.Equals(nameof(TransferQueryObject.TransferFee), StringComparison.OrdinalIgnoreCase))
                    transfers = transferQuery.IsDescending ? transfers.OrderByDescending(t => t.TransferFee) : transfers.OrderBy(t => t.TransferFee);
            }

            return transfers;
        }
    }
}
