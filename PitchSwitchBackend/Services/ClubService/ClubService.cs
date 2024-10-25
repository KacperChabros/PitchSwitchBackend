
using Microsoft.EntityFrameworkCore;
using PitchSwitchBackend.Data;
using PitchSwitchBackend.Dtos.Club.Requests;
using PitchSwitchBackend.Dtos.Club.Responses;
using PitchSwitchBackend.Helpers;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Services.ClubService
{
    public class ClubService : IClubService
    {
        private readonly ApplicationDBContext _context;
        public ClubService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<bool> ClubExists(int clubId)
        {
            return await _context.Clubs.AnyAsync(c => c.ClubId == clubId);
        }

        public async Task<NewClubDto?> AddClub(AddClubDto addClubDto)
        {
            var result = await _context.Clubs.AddAsync(addClubDto.FromAddClubDtoToModel());
            await _context.SaveChangesAsync();

            return result.Entity?.FromModelToNewClubDto();
        }

        public async Task<List<ClubDto>> GetAllClubs(ClubQueryObject clubQuery)
        {
            var clubs = _context.Clubs.AsQueryable();

            clubs = FilterClubs(clubs, clubQuery);

            clubs = SortClubs(clubs, clubQuery);

            var skipNumber = (clubQuery.PageNumber - 1) * clubQuery.PageSize;

            var filteredClubs = await clubs.Skip(skipNumber).Take(clubQuery.PageSize).ToListAsync();

            return filteredClubs.Select(c => c.FromModelToClubDto()).ToList();
        }

        public async Task<Club?> GetClubById(int clubId)
        {
            return await _context.Clubs.FirstOrDefaultAsync(c => c.ClubId == clubId);
        }

        public async Task<ClubDto?> UpdateClub(Club club, UpdateClubDto updateClubDto)
        {
            if (!string.IsNullOrWhiteSpace(updateClubDto.Name))
                club.Name = updateClubDto.Name;

            if (!string.IsNullOrWhiteSpace(updateClubDto.ShortName))
                club.ShortName = updateClubDto.ShortName;

            if (!string.IsNullOrWhiteSpace(updateClubDto.League))
                club.League = updateClubDto.League;

            if (!string.IsNullOrWhiteSpace(updateClubDto.Country))
                club.Country = updateClubDto.Country;

            if (!string.IsNullOrWhiteSpace(updateClubDto.City))
                club.City = updateClubDto.City;

            if (!string.IsNullOrWhiteSpace(updateClubDto.Stadium))
                club.Stadium = updateClubDto.Stadium;

            if (updateClubDto.FoundationYear != null)
                club.FoundationYear = (int)updateClubDto.FoundationYear;

            if (updateClubDto.IsLogoUrlDeleted)
                club.LogoUrl = null;
            else if (!string.IsNullOrWhiteSpace(updateClubDto.LogoUrl))
                club.LogoUrl = updateClubDto.LogoUrl;

            await _context.SaveChangesAsync();

            return club.FromModelToClubDto();
        }

        public async Task DeleteClub(Club club)
        {
            _context.Clubs.Remove(club);
            await _context.SaveChangesAsync();
        }

        private IQueryable<Club> FilterClubs(IQueryable<Club> clubs, ClubQueryObject clubQuery)
        {
            if (!string.IsNullOrWhiteSpace(clubQuery.Name))
                clubs = clubs.Where(c => c.Name.ToLower().Contains(clubQuery.Name.ToLower()));

            if (!string.IsNullOrWhiteSpace(clubQuery.ShortName))
                clubs = clubs.Where(c => c.ShortName.ToLower().Contains(clubQuery.ShortName.ToLower()));

            if (!string.IsNullOrWhiteSpace(clubQuery.League))
                clubs = clubs.Where(c => c.League.ToLower().Contains(clubQuery.League.ToLower()));

            if (!string.IsNullOrWhiteSpace(clubQuery.Country))
                clubs = clubs.Where(c => c.Country.ToLower().Contains(clubQuery.Country.ToLower()));

            if (!string.IsNullOrWhiteSpace(clubQuery.City))
                clubs = clubs.Where(c => c.City.ToLower().Contains(clubQuery.City.ToLower()));

            if (!string.IsNullOrWhiteSpace(clubQuery.Stadium))
                clubs = clubs.Where(c => c.Stadium.ToLower().Contains(clubQuery.Stadium.ToLower()));

            if (clubQuery.FoundationYear != null)
            {
                clubs = clubQuery.FoundationYearComparison switch
                {
                    NumberComparisonTypes.Equal => clubs.Where(c => c.FoundationYear == clubQuery.FoundationYear),
                    NumberComparisonTypes.Less => clubs.Where(c => c.FoundationYear < clubQuery.FoundationYear),
                    NumberComparisonTypes.LessEqual => clubs.Where(c => c.FoundationYear <= clubQuery.FoundationYear),
                    NumberComparisonTypes.Greater => clubs.Where(c => c.FoundationYear > clubQuery.FoundationYear),
                    NumberComparisonTypes.GreaterEqual => clubs.Where(c => c.FoundationYear >= clubQuery.FoundationYear),
                    _ => clubs
                };
            }

            return clubs;
        }

        private IQueryable<Club> SortClubs(IQueryable<Club> clubs, ClubQueryObject clubQuery)
        {
            if (!string.IsNullOrWhiteSpace(clubQuery.SortBy))
            {
                if (clubQuery.SortBy.Equals(nameof(ClubQueryObject.Name), StringComparison.OrdinalIgnoreCase))
                    clubs = clubQuery.IsDescending ? clubs.OrderByDescending(c => c.Name) : clubs.OrderBy(c => c.Name);
                else if (clubQuery.SortBy.Equals(nameof(ClubQueryObject.ShortName), StringComparison.OrdinalIgnoreCase))
                    clubs = clubQuery.IsDescending ? clubs.OrderByDescending(c => c.ShortName) : clubs.OrderBy(c => c.ShortName);
                else if (clubQuery.SortBy.Equals(nameof(ClubQueryObject.League), StringComparison.OrdinalIgnoreCase))
                    clubs = clubQuery.IsDescending ? clubs.OrderByDescending(c => c.League) : clubs.OrderBy(c => c.League);
                else if (clubQuery.SortBy.Equals(nameof(ClubQueryObject.Country), StringComparison.OrdinalIgnoreCase))
                    clubs = clubQuery.IsDescending ? clubs.OrderByDescending(c => c.Country) : clubs.OrderBy(c => c.Country);
                else if (clubQuery.SortBy.Equals(nameof(ClubQueryObject.City), StringComparison.OrdinalIgnoreCase))
                    clubs = clubQuery.IsDescending ? clubs.OrderByDescending(c => c.City) : clubs.OrderBy(c => c.City);
                else if (clubQuery.SortBy.Equals(nameof(ClubQueryObject.FoundationYear), StringComparison.OrdinalIgnoreCase))
                    clubs = clubQuery.IsDescending ? clubs.OrderByDescending(c => c.FoundationYear) : clubs.OrderBy(c => c.FoundationYear);
                else if (clubQuery.SortBy.Equals(nameof(ClubQueryObject.Stadium), StringComparison.OrdinalIgnoreCase))
                    clubs = clubQuery.IsDescending ? clubs.OrderByDescending(c => c.Stadium) : clubs.OrderBy(c => c.Stadium);
            }

            return clubs;
        }
    }
}
