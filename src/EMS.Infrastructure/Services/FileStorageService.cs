using System;
using System.IO;
using System.Threading.Tasks;
using EMS.Application.Interfaces;

namespace EMS.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName)
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            
            // Assume the web application runs from the Web project root folder
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderName);
            
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileToWrite = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileToWrite);
            }

            // Return relative path for web access
            return $"/uploads/{folderName}/{uniqueFileName}";
        }

        public Task DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return Task.CompletedTask;

            // Extract relative filename and convert to local file system path
            var relativePath = fileUrl.TrimStart('/');
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }
    }
}
