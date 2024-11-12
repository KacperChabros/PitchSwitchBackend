using PitchSwitchBackend.Dtos;
using PitchSwitchBackend.Dtos.TransferRumour.Requests;
using PitchSwitchBackend.Dtos.TransferRumour.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Services.TransferRumourService
{
    public interface ITransferRumourService
    {
        Task<NewTransferRumourDto?> AddTransferRumour(AddTransferRumourDto addTransferRumourDto, string userId);
        Task<PaginatedListDto<TransferRumourDto>> GetTransferRumours(TransferRumourQueryObject transferRumourQuery);
        Task<TransferRumour?> GetTransferRumourById(int transferRumourId);
        Task<TransferRumour?> GetTransferRumourWithDataById(int transferRumourId);
        Task<TransferRumourDto?> UpdateTransferRumour(TransferRumour transferRumour, UpdateTransferRumourDto updateTransferRumourDto);
        Task<TransferRumourDto?> ArchiveTransferRumour(TransferRumour transferRumour, bool isConfirmed);
        Task DeleteTransferRumour(TransferRumour transferRumour);
        Task<bool> TransferRumourExists(int transferRumourId);
    }
}
