using System.IO;
using System.Threading.Tasks;

namespace EMS.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName);
        Task DeleteFileAsync(string fileUrl);
    }
}
