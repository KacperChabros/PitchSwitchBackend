﻿namespace PitchSwitchBackend.Dtos.Account.Responses
{
    public class MinimalUserDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
