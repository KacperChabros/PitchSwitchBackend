using PitchSwitchBackend.Dtos.TransferRumour.Requests;
using PitchSwitchBackend.Dtos.TransferRumour.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Services.TransferRumourService
{
    public interface ITransferRumourService
    {
        Task<NewTransferRumourDto?> AddTransferRumour(AddTransferRumourDto addTransferRumourDto);
        Task<List<TransferRumourDto>> GetTransferRumours(TransferRumourQueryObject transferRumourQuery);
        Task<TransferRumour?> GetTransferRumourById(int transferRumourId);
        Task<TransferRumour?> GetTransferRumourWithDataById(int transferRumourId);
        Task<TransferRumourDto?> UpdateTransferRumour(TransferRumour transferRumour, UpdateTransferRumourDto updateTransferRumourDto);
        Task<bool> ArchiveTransferRumour(int transferRumourId, bool isConfirmed);
    }
}
