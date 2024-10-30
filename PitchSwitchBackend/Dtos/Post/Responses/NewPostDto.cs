﻿using PitchSwitchBackend.Dtos.Account.Responses;
using PitchSwitchBackend.Dtos.Transfer.Responses;
using PitchSwitchBackend.Dtos.TransferRumour.Responses;

namespace PitchSwitchBackend.Dtos.Post.Responses
{
    public class NewPostDto
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsEdited { get; set; } = false;
        public MinimalUserDto CreatedByUser { get; set; }
        public MinimalTransferDto? Transfer { get; set; }
        public MinimalTransferRumourDto? TransferRumour { get; set; }
    }
}
