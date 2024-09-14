namespace Fantasy.Backend.Helpers;

public interface IFileStorage
{
    Task<string> SaveFileAsync(byte[] content, string extention, string containerName);

    Task RemoveFileAsync(string path, string containerName);
}