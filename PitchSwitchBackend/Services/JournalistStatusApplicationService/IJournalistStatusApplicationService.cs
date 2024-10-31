using PitchSwitchBackend.Dtos.JournalistStatusApplication.Requests;
using PitchSwitchBackend.Dtos.JournalistStatusApplication.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Services.JournalistStatusApplicationService
{
    public interface IJournalistStatusApplicationService
    {
        Task<bool> CheckIfUserHasOpenApplication(string userId);
        Task<NewJournalistStatusApplicationDto?> AddApplication(AddJournalistStatusApplicationDto applicationDto, string userId);
        Task<JournalistStatusApplication?> GetApplicationById(int applicationId);
        Task<JournalistStatusApplication?> GetApplicationByIdWithData(int applicationId);
        Task<List<JournalistStatusApplicationDto>> GetAllApplications(JournalistStatusApplicationQueryObject applicationQuery);
        Task<JournalistStatusApplicationDto?> UpdateApplication(JournalistStatusApplication application, UpdateJournalistStatusApplicationDto updateDto);
        Task<JournalistStatusApplicationDto?> ReviewApplication(JournalistStatusApplication application, ReviewJournalistStatusApplicationDto reviewDto);
        Task DeleteApplication(JournalistStatusApplication application);
    }
}
