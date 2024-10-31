using Microsoft.EntityFrameworkCore;
using PitchSwitchBackend.Data;
using PitchSwitchBackend.Dtos.JournalistStatusApplication.Requests;
using PitchSwitchBackend.Dtos.JournalistStatusApplication.Responses;
using PitchSwitchBackend.Helpers;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Models;
using PitchSwitchBackend.Services.AuthService;

namespace PitchSwitchBackend.Services.JournalistStatusApplicationService
{
    public class JournalistStatusApplicationService : IJournalistStatusApplicationService
    {
        private readonly ApplicationDBContext _context;
        private readonly IAuthService _authService;
        public JournalistStatusApplicationService(ApplicationDBContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<bool> CheckIfUserHasOpenApplication(string userId)
        {
            return await _context.JournalistStatusApplications.AnyAsync(jsa => jsa.SubmittedByUserId == userId && !jsa.IsReviewed);
        }

        public async Task<NewJournalistStatusApplicationDto?> AddApplication(AddJournalistStatusApplicationDto applicationDto, string userId)
        {
            var application = applicationDto.FromAddJournalistStatusApplicationDtoToModel();
            application.IsAccepted = false;
            application.IsReviewed = false;
            application.SubmittedByUserId = userId;
            application.CreatedOn = DateTime.UtcNow;

            var result = await _context.JournalistStatusApplications.AddAsync(application);
            await _context.SaveChangesAsync();
            var addedApplication = await GetApplicationByIdWithData(result.Entity.Id);

            return addedApplication?.FromModelToNewJournalistStatusApplicationDto();
        }

        public async Task<JournalistStatusApplication?> GetApplicationById(int applicationId)
        {
            return await _context.JournalistStatusApplications.FirstOrDefaultAsync(jsa => jsa.Id == applicationId);
        }

        public async Task<JournalistStatusApplication?> GetApplicationByIdWithData(int applicationId)
        {
            return await _context.JournalistStatusApplications.Where(jsa => jsa.Id == applicationId)
                .Include(jsu => jsu.SubmittedByUser)
                .FirstOrDefaultAsync();
        }

        public async Task<List<JournalistStatusApplicationDto>> GetAllApplications(JournalistStatusApplicationQueryObject applicationQuery)
        {
            var applications = _context.JournalistStatusApplications.AsQueryable();

            applications = FilterApplications(applications, applicationQuery);

            applications = SortApplications(applications, applicationQuery);

            var skipNumber = (applicationQuery.PageNumber - 1) * applicationQuery.PageSize;

            applications = applications.Include(jsu => jsu.SubmittedByUser);

            var filteredApplications = await applications.Skip(skipNumber).Take(applicationQuery.PageSize).ToListAsync();

            return filteredApplications.Select(jsa => jsa.FromModelToJournalistStatusApplicationDto()).ToList();
        }

        public async Task<JournalistStatusApplicationDto?> UpdateApplication(JournalistStatusApplication application, UpdateJournalistStatusApplicationDto updateDto)
        {
            if (!string.IsNullOrWhiteSpace(updateDto.Motivation))
                application.Motivation = updateDto.Motivation;

            await _context.SaveChangesAsync();

            var updatedApplication = await GetApplicationByIdWithData(application.Id);

            return updatedApplication?.FromModelToJournalistStatusApplicationDto();
        }

        public async Task<JournalistStatusApplicationDto?> ReviewApplication(JournalistStatusApplication application, ReviewJournalistStatusApplicationDto reviewDto)
        {
            if (!reviewDto.IsAccepted)
            {
                if (!string.IsNullOrWhiteSpace(reviewDto.RejectionReason))
                {
                    application.RejectionReason = reviewDto.RejectionReason;
                }
            }
            else
            {
                await _authService.AddUserToRole(application.SubmittedByUser, "Journalist");
            }
            
            application.IsAccepted = reviewDto.IsAccepted;
            application.IsReviewed = true;
            application.ReviewedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            var updatedApplication = await GetApplicationByIdWithData(application.Id);

            return updatedApplication?.FromModelToJournalistStatusApplicationDto();
        }

        public async Task DeleteApplication(JournalistStatusApplication application)
        {
            _context.JournalistStatusApplications.Remove(application);
            await _context.SaveChangesAsync();
        }

        private IQueryable<JournalistStatusApplication> FilterApplications(
            IQueryable<JournalistStatusApplication> applications, JournalistStatusApplicationQueryObject applicationQuery)
        {
            if (!string.IsNullOrWhiteSpace(applicationQuery.Motivation))
                applications = applications.Where(jsa => jsa.Motivation.ToLower().Contains(applicationQuery.Motivation.ToLower()));

            if (!string.IsNullOrWhiteSpace(applicationQuery.RejectionReason))
                applications = applications.Where(jsa => !string.IsNullOrWhiteSpace(jsa.RejectionReason) && 
                                                jsa.RejectionReason.ToLower().Contains(applicationQuery.RejectionReason.ToLower()));

            if (!string.IsNullOrWhiteSpace(applicationQuery.SubmittedByUserId))
                applications = applications.Where(jsa => jsa.SubmittedByUserId.ToLower().Equals(applicationQuery.SubmittedByUserId.ToLower()));

            if (applicationQuery.IsReviewed.HasValue)
                applications = applications.Where(jsa => jsa.IsReviewed == applicationQuery.IsReviewed.Value);

            if (applicationQuery.IsAccepted.HasValue)
                applications = applications.Where(jsa => jsa.IsAccepted == applicationQuery.IsAccepted.Value);

            if (applicationQuery.CreatedOn != null)
            {
                applications = applicationQuery.CreatedOnComparison switch
                {
                    NumberComparisonTypes.Equal => applications.Where(jsa => jsa.CreatedOn == applicationQuery.CreatedOn),
                    NumberComparisonTypes.Less => applications.Where(jsa => jsa.CreatedOn < applicationQuery.CreatedOn),
                    NumberComparisonTypes.LessEqual => applications.Where(jsa => jsa.CreatedOn <= applicationQuery.CreatedOn),
                    NumberComparisonTypes.Greater => applications.Where(jsa => jsa.CreatedOn > applicationQuery.CreatedOn),
                    NumberComparisonTypes.GreaterEqual => applications.Where(jsa => jsa.CreatedOn >= applicationQuery.CreatedOn),
                    _ => applications
                };
            }

            return applications;
        }

        private IQueryable<JournalistStatusApplication> SortApplications(
            IQueryable<JournalistStatusApplication> applications, JournalistStatusApplicationQueryObject applicationQuery)
        {
            if (!string.IsNullOrWhiteSpace(applicationQuery.SortBy))
            {
                if (applicationQuery.SortBy.Equals(nameof(JournalistStatusApplicationQueryObject.Motivation), StringComparison.OrdinalIgnoreCase))
                    applications = applicationQuery.IsDescending ? applications.OrderByDescending(jsa => jsa.Motivation) : applications.OrderBy(jsa => jsa.Motivation);
                else if (applicationQuery.SortBy.Equals(nameof(JournalistStatusApplicationQueryObject.CreatedOn), StringComparison.OrdinalIgnoreCase))
                    applications = applicationQuery.IsDescending ? applications.OrderByDescending(jsa => jsa.CreatedOn) : applications.OrderBy(jsa => jsa.CreatedOn);
                else if (applicationQuery.SortBy.Equals(nameof(JournalistStatusApplicationQueryObject.IsAccepted), StringComparison.OrdinalIgnoreCase))
                    applications = applicationQuery.IsDescending ? applications.OrderByDescending(jsa => jsa.IsAccepted) : applications.OrderBy(jsa => jsa.IsAccepted);
                else if (applicationQuery.SortBy.Equals(nameof(JournalistStatusApplicationQueryObject.IsReviewed), StringComparison.OrdinalIgnoreCase))
                    applications = applicationQuery.IsDescending ? applications.OrderByDescending(jsa => jsa.IsReviewed) : applications.OrderBy(jsa => jsa.IsReviewed);
            }

            return applications;
        }
    }
}
