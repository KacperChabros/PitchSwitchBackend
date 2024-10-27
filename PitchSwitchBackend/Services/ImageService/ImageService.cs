using PitchSwitchBackend.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace PitchSwitchBackend.Services.ImageService
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Dictionary<string, (int width, int height)> _imageDimensions = new()
        {
            { UploadFolders.ClubsDir, (512, 512) },
            { UploadFolders.PlayersDir, (512, 1024) },
            { UploadFolders.UsersDir, (512, 512) },
            { UploadFolders.PostsDir, (1024, 1024) }
        };

        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            CheckFileValidity(file);

            if (!_imageDimensions.TryGetValue(folderName, out var dimensions))
            {
                throw new InvalidOperationException("Invalid folder name or no dimensions set for this folder.");
            }

            using var image = await ProcessImage(file, dimensions.width, dimensions.height);
            
            var uploadsPath = CreateUploadsFolder(folderName);

            var filePath = CreateFullPath(uploadsPath, Path.GetExtension(file.FileName).ToLowerInvariant());

            await image.SaveAsync(filePath);

            var uniqueFileName = Path.GetFileName(filePath);

            return Path.Combine(folderName, uniqueFileName).Replace("\\", "/");
        }

        public void DeleteFile(string filePath)
        {
            var fullPath = Path.GetFullPath(Path.Combine(_webHostEnvironment.WebRootPath, filePath));

            if (!fullPath.StartsWith(_webHostEnvironment.WebRootPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid file path.");
            }

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        private void CheckFileValidity(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file");

            if (file.Length > 3 * 1024 * 1024 || file.Length < 1024) // 3 MB
            {
                throw new ArgumentException("Invalid file size");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("File type is not allowed. Only image files are allowed.");
            }

            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                throw new ArgumentException("Invalid file type. Only image files are allowed.");
            }
        }

        private async Task<SixLabors.ImageSharp.Image> ProcessImage(IFormFile file, int width, int height)
        {
            var image = await SixLabors.ImageSharp.Image.LoadAsync(file.OpenReadStream());

            if (image.Width > 2000 || image.Height > 2000)
            {
                throw new ArgumentException("Image dimensions are too large.");
            }

            image.Metadata.ExifProfile = null;

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = ResizeMode.Max
            }));

            return image;
        }

        private string CreateUploadsFolder(string folderName)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
            var fullFolderPath = Path.GetFullPath(uploadsFolder);
            if (!fullFolderPath.StartsWith(_webHostEnvironment.WebRootPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid folder path.");
            }

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            return uploadsFolder;
        }

        private string CreateFullPath(string uploadsPath, string extension)
        {
            var uniqueFileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            var fullFilePath = Path.GetFullPath(filePath);
            if (!fullFilePath.StartsWith(_webHostEnvironment.WebRootPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid file path.");
            }

            return filePath;
        }
    }
}
