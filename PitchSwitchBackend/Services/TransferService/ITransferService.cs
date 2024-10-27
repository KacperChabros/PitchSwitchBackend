using PitchSwitchBackend.Dtos.Transfer.Requests;
using PitchSwitchBackend.Dtos.Transfer.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Services.TransferService
{
    public interface ITransferService
    {
        Task<NewTransferDto?> AddTransfer(AddTransferDto addTransferDto);
        Task<List<TransferDto>> GetTransfers(TransferQueryObject transferQuery);
        Task<Transfer?> GetTransferById(int transferId);
        Task<Transfer?> GetTransferWithDataById(int transferId);
        Task<TransferDto?> UpdateTransfer(Transfer transfer, UpdateTransferDto updateTransferDto);
        Task DeleteTransfer(Transfer transfer);
    }
}
