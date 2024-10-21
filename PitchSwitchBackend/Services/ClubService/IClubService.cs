using PitchSwitchBackend.Dtos.Club.Requests;
using PitchSwitchBackend.Dtos.Club.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Services.ClubService
{
    public interface IClubService
    {
        Task<bool> ClubExists(int clubId);
        Task<NewClubDto?> AddClub(AddClubDto addClubDto);
        Task<List<ClubDto>> GetAllClubs(ClubQueryObject clubQuery);
        Task<Club?> GetClubById(int clubId);
        Task<ClubDto?> UpdateClub(Club club, UpdateClubDto updateClubDto);
        Task DeleteClub(Club club);
    }
}
