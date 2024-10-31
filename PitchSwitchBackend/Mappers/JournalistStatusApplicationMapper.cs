using PitchSwitchBackend.Dtos.JournalistStatusApplication.Requests;
using PitchSwitchBackend.Dtos.JournalistStatusApplication.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Mappers
{
    public static class JournalistStatusApplicationMapper
    {
        public static JournalistStatusApplication FromAddJournalistStatusApplicationDtoToModel(this AddJournalistStatusApplicationDto dto)
        {
            return new JournalistStatusApplication
            {
                Motivation = dto.Motivation
            };
        }

        public static NewJournalistStatusApplicationDto FromModelToNewJournalistStatusApplicationDto(this JournalistStatusApplication model)
        {
            return new NewJournalistStatusApplicationDto
            {
                Id = model.Id,
                Motivation = model.Motivation,
                CreatedOn = model.CreatedOn,
                IsAccepted = model.IsAccepted,
                IsReviewed = model.IsReviewed,
                ReviewedOn = model.ReviewedOn,
                RejectionReason = model.RejectionReason,
                SubmittedByUser = model.SubmittedByUser.FromModelToMinimalUserDto()
            };
        }

        public static JournalistStatusApplicationDto FromModelToJournalistStatusApplicationDto(this JournalistStatusApplication model)
        {
            return new JournalistStatusApplicationDto
            {
                Id = model.Id,
                Motivation = model.Motivation,
                CreatedOn = model.CreatedOn,
                IsAccepted = model.IsAccepted,
                IsReviewed = model.IsReviewed,
                ReviewedOn = model.ReviewedOn,
                RejectionReason = model.RejectionReason,
                SubmittedByUser = model.SubmittedByUser.FromModelToMinimalUserDto()
            };
        }
    }
}
