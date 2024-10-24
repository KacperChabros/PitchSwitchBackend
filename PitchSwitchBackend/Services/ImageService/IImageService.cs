namespace PitchSwitchBackend.Services.ImageService
{
    public interface IImageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName);
        void DeleteFile(string filePath);
    }
}
