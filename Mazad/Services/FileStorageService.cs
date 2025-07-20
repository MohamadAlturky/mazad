using Microsoft.AspNetCore.Http;

namespace Mazad.Services;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string directory);
    // Task<(byte[] FileContents, string ContentType)> GetFileAsync(string filePath);
    bool DeleteFile(string filePath);
}

public class FileStorageService : IFileStorageService
{
    private readonly string _baseStoragePath;
    private readonly Dictionary<string, string> _allowedMimeTypes = new()
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".webp", "image/webp" },
    };

    public FileStorageService(IWebHostEnvironment environment)
    {
        _baseStoragePath = Path.Combine(environment.WebRootPath, "uploads");
        EnsureDirectoryExists(_baseStoragePath);
    }

    public async Task<string> SaveFileAsync(IFormFile file, string directory)
    {
        try
        {
            // Validate file
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file was provided");
            }

            // Validate file type
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedMimeTypes.ContainsKey(extension))
            {
                throw new ArgumentException("File type not allowed");
            }

            // Normalize directory path and ensure it's relative
            directory = directory.Replace("\\", "/").TrimStart('/');

            // Create directory if it doesn't exist
            var targetDirectory = Path.Combine(_baseStoragePath, directory);
            EnsureDirectoryExists(targetDirectory);

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{extension}";
            var relativePath = Path.Combine(directory, fileName).Replace("\\", "/");
            var fullPath = Path.Combine(_baseStoragePath, directory, fileName);

            Console.WriteLine($"Saving file to: {fullPath}");
            Console.WriteLine($"Relative path will be: {relativePath}");

            // Save file
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative path
            return relativePath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save file: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    // public async Task<(byte[] FileContents, string ContentType)> GetFileAsync(string filePath)
    // {
    //     try
    //     {
    //         // URL decode the path and normalize slashes
    //         filePath = Uri.UnescapeDataString(filePath).Replace("\\", "/").TrimStart('/');

    //         // Remove any wwwroot/uploads prefix if it exists
    //         filePath = filePath.Replace("wwwroot/uploads/", "").Replace("uploads/", "");

    //         var fullPath = Path.Combine(_baseStoragePath, filePath);
    //         fullPath = Path.GetFullPath(fullPath); // Normalize the path

    //         // Security check - ensure the path is still under _baseStoragePath
    //         if (!fullPath.StartsWith(_baseStoragePath))
    //         {
    //             throw new ArgumentException("Invalid file path");
    //         }

    //         Console.WriteLine($"Attempting to read file from: {fullPath}");

    //         if (!File.Exists(fullPath))
    //         {
    //             Console.WriteLine($"File not found at path: {fullPath}");
    //             throw new FileNotFoundException($"File not found at path: {filePath}");
    //         }

    //         var extension = Path.GetExtension(filePath).ToLowerInvariant();
    //         if (!_allowedMimeTypes.TryGetValue(extension, out var contentType))
    //         {
    //             throw new ArgumentException($"Invalid file type: {extension}");
    //         }

    //         var fileBytes = await File.ReadAllBytesAsync(fullPath);
    //         return (fileBytes, contentType);
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Failed to retrieve file: {ex.Message}");
    //         Console.WriteLine($"Stack trace: {ex.StackTrace}");
    //         throw;
    //     }
    // }

    public bool DeleteFile(string filePath)
    {
        try
        {
            // Normalize file path and ensure it's relative
            filePath = filePath.Replace("\\", "/").TrimStart('/');
            var fullPath = Path.Combine(_baseStoragePath, filePath);

            Console.WriteLine($"Attempting to delete file: {fullPath}");

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                Console.WriteLine($"Successfully deleted file: {fullPath}");
                return true;
            }
            Console.WriteLine($"File not found for deletion: {fullPath}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete file: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    private void EnsureDirectoryExists(string path)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Console.WriteLine($"Created directory: {path}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create directory {path}: {ex.Message}");
            throw;
        }
    }
}
