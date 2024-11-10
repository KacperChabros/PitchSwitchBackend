
using Microsoft.EntityFrameworkCore;
using PitchSwitchBackend.Data;
using PitchSwitchBackend.Dtos.Club.Requests;
using PitchSwitchBackend.Dtos.Club.Responses;
using PitchSwitchBackend.Helpers;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Models;
using PitchSwitchBackend.Services.ImageService;

namespace PitchSwitchBackend.Services.ClubService
{
    public class ClubService : IClubService
    {
        private readonly ApplicationDBContext _context;
        private readonly IImageService _imageService;
        public ClubService(ApplicationDBContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<bool> ClubExists(int clubId)
        {
            return await _context.Clubs.AnyAsync(c => c.ClubId == clubId);
        }

        public async Task<bool> ClubExistsAndNotArchived(int clubId)
        {
            return await _context.Clubs.AnyAsync(c => c.ClubId == clubId && !c.IsArchived);
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
            if (clubQuery.IncludeArchived.HasValue && !clubQuery.IncludeArchived.Value)
                clubs = clubs.Where(c => !c.IsArchived);

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
            if(updateClubDto.Logo != null)
            {
                var oldLogoUrl = club.LogoUrl;
                var newLogoUrl = await _imageService.UploadFileAsync(updateClubDto.Logo, UploadFolders.ClubsDir);
                club.LogoUrl = newLogoUrl;
                if(!string.IsNullOrEmpty(oldLogoUrl))
                    _imageService.DeleteFile(oldLogoUrl);
            }
            else if(updateClubDto.IsLogoDeleted)
            {
                if (!string.IsNullOrEmpty(club.LogoUrl))
                    _imageService.DeleteFile(club.LogoUrl);
                club.LogoUrl = null;
            }

            
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

            await _context.SaveChangesAsync();

            return club.FromModelToClubDto();
        }

        public async Task<bool> ArchiveClub(int clubId)
        {
            var club = await _context.Clubs
                .Include(c => c.Players)
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.ClubId == clubId);

            if (club == null)
                return false;

            foreach (var player in club.Players)
            {
                player.ClubId = null;
            }

            foreach (var user in club.Users)
            {
                user.FavouriteClubId = null;
            }

            club.IsArchived = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ClubDto?> RestoreClub(int clubId)
        {
            var club = await _context.Clubs.FirstOrDefaultAsync(c => c.ClubId == clubId);
            if (club == null)
                return null;

            club.IsArchived = false;
            await _context.SaveChangesAsync();

            return club.FromModelToClubDto();
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
            }

            return clubs;
        }
    }
}
