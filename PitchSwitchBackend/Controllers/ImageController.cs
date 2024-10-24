using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PitchSwitchBackend.Dtos.Images.Requests;
using PitchSwitchBackend.Helpers;
using PitchSwitchBackend.Services.ImageService;

namespace PitchSwitchBackend.Controllers
{
    [Route("api/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("uploadimage")]
        [Authorize]
        public async Task<IActionResult> UploadImage([FromForm] ImageUploadDto imageUploadDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!CanUploadImage(imageUploadDto.EntityType))
            {
                return Forbid();
            }

            if (imageUploadDto.FormFile == null || imageUploadDto.FormFile.Length == 0)
                return BadRequest("No file provided.");

            string folderName = GetUploadFolder(imageUploadDto.EntityType);
            if (string.IsNullOrEmpty(folderName))
            {
                return BadRequest("Invalid entity");
            }

            string fileUrl = await _imageService.UploadFileAsync(imageUploadDto.FormFile, folderName);
            return Ok(new { fileUrl });
        }

        [HttpDelete("deleteimage")]
        [Authorize]
        public async Task<IActionResult> DeleteImage([FromBody] DeleteImageDto deleteImageDto)
        {
            if (!CanDeleteImage(deleteImageDto.ImageUrl))
            {
                return Forbid();
            }

            _imageService.DeleteFile(deleteImageDto.ImageUrl);
            return NoContent();
        }

        private bool CanUploadImage(string entityType)
        {
            return entityType.ToLower() switch
            {
                UploadFolders.ClubEntity => User.IsInRole("Admin"),
                UploadFolders.PlayerEntity => User.IsInRole("Admin"),
                UploadFolders.PostEntity => User.IsInRole("Journalist") || User.IsInRole("Admin"),
                UploadFolders.UserEntity => User.IsInRole("User") || User.IsInRole("Journalist") || User.IsInRole("Admin"),
                _ => false,
            };
        }

        private bool CanDeleteImage(string imgUrl)
        {
            if (imgUrl.StartsWith(UploadFolders.ClubsDir))
                return User.IsInRole("Admin");

            if (imgUrl.StartsWith(UploadFolders.PlayersDir) )
                return User.IsInRole("Admin");

            if (imgUrl.StartsWith(UploadFolders.PostsDir))
                return User.IsInRole("Admin") || User.IsInRole("Journalist");

            if (imgUrl.StartsWith(UploadFolders.UsersDir))
                return User.IsInRole("Admin") || User.IsInRole("Journalist") || User.IsInRole("User");

            return false;
        }

        private string GetUploadFolder(string entityType)
        {
            return entityType.ToLower() switch
            {
                UploadFolders.ClubEntity => UploadFolders.ClubsDir,
                UploadFolders.UserEntity => UploadFolders.UsersDir,
                UploadFolders.PlayerEntity => UploadFolders.PlayersDir,
                UploadFolders.PostEntity => UploadFolders.PostsDir,
                _ => string.Empty,
            };
        }
    }
}
