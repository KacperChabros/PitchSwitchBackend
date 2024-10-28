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
            ValidateNewTransferData(addTransferDto.SellingClubId, addTransferDto.BuyingClubId);

            var result = await _context.Transfers.AddAsync(addTransferDto.FromAddTransferDtoToModel());
            await _context.SaveChangesAsync();
            var transfer = result.Entity;
            if (transfer.TransferDate.Date == DateTime.Today)
            {
                var player = await _playerService.GetPlayerById(transfer.PlayerId);
                var updatePlayerDto = new UpdatePlayerDto { ClubId = transfer.BuyingClubId,  IsClubIdDeleted = transfer.BuyingClubId == null};
                await _playerService.UpdatePlayer(player, updatePlayerDto);
            }

            if (transfer != null)
            {
                if (transfer.SellingClubId.HasValue)
                {
                    transfer.SellingClub = await _clubService.GetClubById(transfer.SellingClubId.GetValueOrDefault());
                }
                if (transfer.BuyingClubId.HasValue)
                {
                    transfer.BuyingClub = await _clubService.GetClubById(transfer.BuyingClubId.GetValueOrDefault());
                }
                transfer.Player = await _playerService.GetPlayerById(transfer.PlayerId);
            }

            return transfer?.FromModelToNewTransferDto();
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
            return await _context.Transfers.Include(t => t.Player).Include(t => t.SellingClub).Include(t => t.BuyingClub)
                .FirstOrDefaultAsync(t => t.TransferId == transferId);
        }

        public async Task<Transfer?> GetTransferById(int transferId)
        {
            return await _context.Transfers.FirstOrDefaultAsync(t => t.TransferId == transferId);
        }

        public async Task<TransferDto?> UpdateTransfer(Transfer transfer, UpdateTransferDto updateTransferDto)
        {
            await ValidateExistenceOfData(updateTransferDto.PlayerId, updateTransferDto.SellingClubId, updateTransferDto.BuyingClubId);
            ValidateUpdateTransferData(transfer, updateTransferDto);

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

            await _context.Entry(transfer).Reference(t => t.Player).LoadAsync();
            await _context.Entry(transfer).Reference(t => t.SellingClub).LoadAsync();
            await _context.Entry(transfer).Reference(t => t.BuyingClub).LoadAsync();

            return transfer.FromModelToTransferDto();
        }
        public async Task DeleteTransfer(Transfer transfer)
        {
            _context.Transfers.Remove(transfer);
            await _context.SaveChangesAsync();
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

        private void ValidateNewTransferData(int? sellingClubId, int? buyingClubId)
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

        private void ValidateUpdateTransferData(Transfer transfer, UpdateTransferDto updateTransferDto)
        {
            int? newSellingClubId = updateTransferDto.IsSellingClubDeleted ? null : updateTransferDto.SellingClubId ?? transfer.SellingClubId;
            int? newBuyingClubId = updateTransferDto.IsBuyingClubDeleted ? null : updateTransferDto.BuyingClubId ?? transfer.BuyingClubId;

            if (newSellingClubId == null && newBuyingClubId == null)
            {
                throw new ArgumentException("SellingClub and BuyingClub cannot be null at once");
            }

            if (newSellingClubId.HasValue && newBuyingClubId.HasValue && newSellingClubId == newBuyingClubId)
            {
                throw new ArgumentException("SellingClub and BuyingClub cannot have the same value.");
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
                    NumberComparisonTypes.Equal => transfers.Where(t => t.TransferDate == transferQuery.TransferDate),
                    NumberComparisonTypes.Less => transfers.Where(t => t.TransferDate < transferQuery.TransferDate),
                    NumberComparisonTypes.LessEqual => transfers.Where(t => t.TransferDate <= transferQuery.TransferDate),
                    NumberComparisonTypes.Greater => transfers.Where(t => t.TransferDate > transferQuery.TransferDate),
                    NumberComparisonTypes.GreaterEqual => transfers.Where(t => t.TransferDate >= transferQuery.TransferDate),
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
