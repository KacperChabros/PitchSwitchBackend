using PitchSwitchBackend.Dtos;
using PitchSwitchBackend.Dtos.Club.Requests;
using PitchSwitchBackend.Dtos.Club.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Services.ClubService
{
    public interface IClubService
    {
        Task<bool> ClubExists(int clubId);
        Task<bool> ClubExistsAndNotArchived(int clubId);
        Task<NewClubDto?> AddClub(AddClubDto addClubDto);
        Task<PaginatedListDto<ClubDto>> GetAllClubs(ClubQueryObject clubQuery);
        Task<List<MinimalClubDto>> GetAllMinimalClubs();
        Task<Club?> GetClubById(int clubId);
        Task<ClubDto?> UpdateClub(Club club, UpdateClubDto updateClubDto);
        Task<bool> ArchiveClub(int clubId);
        Task<ClubDto?> RestoreClub(int clubId);
    }
}
